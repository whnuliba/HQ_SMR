using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extension
{
    public static class PredicateExtensions
    {


        /// <summary>
        /// 添加And条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.AndAlso<T>(second, Expression.AndAlso); //C#中类似于&&
        }
        /// <summary>
        /// 添加Or条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.AndAlso<T>(second, Expression.OrElse);//C#中类似于||
        }
        /// <summary>
        /// 合并表达式以及参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, Func<Expression, Expression, BinaryExpression> func)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(func(left, right), parameter);



        }
        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }

        public static Expression<Func<T, bool>> Or1<T>(this Expression<Func<T, bool>> exprLeft,
           Expression<Func<T, bool>> exprRight)
        {
            var invokedExpr = Expression.Invoke(exprRight, exprLeft.Parameters);

            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(exprLeft.Body, invokedExpr), exprLeft.Parameters);
        }

        public static Expression<Func<T, bool>> PermissionExpression<T>(this Expression<Func<T, bool>> predicate, string value, Func<T, string> fieldFunc)
        {
            if (!string.IsNullOrEmpty(value) && value.Contains(";"))
            {
                string[] items = value.Split(';');

                Expression<Func<T, bool>> predicate1 = o => fieldFunc(o).Contains(items[0]);
                for (int i = 1; i < items.Length; i++)
                {
                    int j = i;
                    predicate1 = predicate1.Or(o => fieldFunc(o).Contains(items[j]));
                }

                predicate = predicate.Or(predicate1);
            }
            else
            {
                predicate = predicate.Or(o => fieldFunc(o).Contains(value));
            }

            return predicate;
        }
        public static Expression<Func<T, bool>> OrContains<T>(this Expression<Func<T, string>> fieldFunc, string value)
        {
            ParameterExpression parameter = fieldFunc.Parameters.First();
            MemberExpression member = fieldFunc.Body as MemberExpression;

            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            Expression exprLeft;
            if (!string.IsNullOrEmpty(value) && value.Contains(";"))
            {
                var items = value.Split(';');

                exprLeft = null;
                foreach (var item in items)
                {
                    ConstantExpression constant = Expression.Constant(item, typeof(string));
                    MethodCallExpression exprRight = Expression.Call(member, method, constant);
                    if (exprLeft == null)
                    {
                        exprLeft = exprRight;
                    }
                    else
                    {
                        exprLeft = Expression.Or(exprLeft, exprRight);
                    }
                }

                if (exprLeft == null)
                {
                    Expression<Func<T, bool>> defaultFilter = o => true;
                    exprLeft = defaultFilter;
                }
            }
            else
            {
                ConstantExpression constant = Expression.Constant(value, typeof(string));
                exprLeft = Expression.Call(member, method, constant);
            }

            Expression<Func<T, bool>> expr = Expression.Lambda<Func<T, bool>>(exprLeft, parameter);

            return expr;
        }

        public static Expression<Func<T, bool>> OrContainsIf<T>(this Expression<Func<T, bool>> expr, Expression<Func<T, string>> fieldFunc, string value)
        {
            if (expr == null)
            {
                expr = OrContains(fieldFunc, value);
            }
            else
            {
                expr = expr.Or(OrContains(fieldFunc, value));
            }

            return expr;
        }
    }
}
