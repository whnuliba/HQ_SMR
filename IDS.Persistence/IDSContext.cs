using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using IDS.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using IDS.Base;
using System.Reflection;
using IDS.Common;
namespace IDS.Persistence;

public partial class IDSContext : DbContext
{

    public IDSContext(DbContextOptions options)
    : base(options)
    {
    }
    public int Insert<T>(T o) where T : class
    {
        //using (new WriterLock(mRw))
        //{
        try
        {
            base.Add(o);
            return SaveChanges();
        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }
        return 0;
        //}

    }

    public int AddOrUpdate<T>(T data, Expression<Func<T, bool>> predicate = null) where T : class
    {
        //using (new WriterLock(mRw))
        //{
        try
        {
            var set = Set<T>();
            var o = set.SingleOrDefault(predicate);
            if (o == null)
                Entry(data).State = EntityState.Added;
            else
            {
                Entry(o).CurrentValues.SetValues(data);
                Entry(o).State = EntityState.Modified;
            }

            return SaveChanges();
        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }

        return 0;
        //}
    }


    public int Save<T>(T data) where T : class
    {
        //using (new WriterLock(mRw))
        //{
        try
        {
            base.Add(data);
            int i = SaveChanges();
            return i;
        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }

        return 0;
        //}
    }

    public int Count<T>(Expression<Func<T, bool>> predicate, bool readUncommit = false) where T : class
    {
        //using (new ReaderLock(mRw))
        //{
        try
        {

            if (!readUncommit)
            {
                if (predicate == null)
                {
                    return Set<T>().AsNoTracking().Count();
                }
                return Set<T>().AsNoTracking().Count(predicate);
            }

            using (this.Database.BeginTransaction())
            {
                return Set<T>().AsNoTracking().Count(predicate);
            }
        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }

        return 0;
        //}
    }

    public bool Any<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        //using (new ReaderLock(mRw))
        //{
        try
        {
            return Set<T>().AsNoTracking().Any(predicate);
        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }

        return false;
        //}
    }

    public bool All<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        //using (new ReaderLock(mRw))
        //{
        try
        {
            return Set<T>().AsNoTracking().All(predicate);
        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }

        return false;
        //}
    }

    public int Delete<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        //using (new WriterLock(mRw))
        //{
        try
        {
            var set = Set<T>();
            var query = set.Where(predicate).ToList();
            query.ForEach(f => set.Remove(f));
            return SaveChanges();
        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }

        return 0;
        //}
    }

    public int Sql(string sql, params IDbDataParameter[] parameters)
    {
        //using (new WriterLock(mRw))
        //{
        try
        {
            return Database.ExecuteSqlRaw(sql, parameters);
        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }

        return 0;
        //}
    }

    //public int Procedure(string procedureName, Dictionary<string, object> inputs)
    //{
    //    //using (new WriterLock(mRw))
    //    //{
    //    using (var cmd = BuildCommand(CommandType.StoredProcedure, procedureName))
    //    {
    //        cmd.CommandTimeout = 600;
    //        Database.OpenConnection();
    //        try
    //        {
    //            SqlCommandBuilder.DeriveParameters(cmd);
    //            foreach (var kv in inputs)
    //            {
    //                var paramName = "@" + kv.Key;
    //                if (cmd.Parameters.Contains(paramName))
    //                    cmd.Parameters[paramName].Value = kv.Value;
    //            }

    //            var result = cmd.ExecuteNonQuery();
    //            foreach (SqlParameter p in cmd.Parameters)
    //            {
    //                var name = p.ParameterName.Replace("@", string.Empty);
    //                if (inputs.ContainsKey(name))
    //                    inputs[name] = p.Value;
    //                else
    //                    inputs.Add(name, p.Value);
    //            }

    //            return result;
    //        }
    //        catch (Exception e)
    //        {
    //            ExceptionManager.ThrowException(e);
    //        }
    //        finally
    //        {
    //            Database.CloseConnection();
    //        }
    //        return -1;

    //    }
    //    //}
    //}

    public IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        //using (new ReaderLock(mRw))
        //{
        try
        {
            return predicate == null ? Set<T>().AsNoTracking() : Set<T>().Where(predicate).AsNoTracking();
        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }

        return null;
        //}
    }

    //public IQueryable<T> QueryNoLock<T>(Expression<Func<T, bool>> predicate) where T : class
    //{
    //    try
    //    {
    //        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
    //        {
    //            return predicate == null ? Set<T>().AsNoTracking() : Set<T>().Where(predicate).AsNoTracking();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //       ExceptionManager.ThrowException(ex);
    //    }

    //    return null;
    //}

    //public DataTable GetTable(string sql)
    //{
    //    //using (new ReaderLock(mRw))
    //    //{
    //    try
    //    {
    //        var table = new DataTable();
    //        var conn = Database.GetDbConnection();
    //        var cmd = conn.CreateCommand();
    //        cmd.CommandText = sql;
    //        var adapter = SqlClientFactory.Instance.CreateDataAdapter();
    //        adapter.SelectCommand = cmd;
    //        adapter.Fill(table);
    //        return table;
    //    }
    //    catch (Exception ex)
    //    {
    //       ExceptionManager.ThrowException(ex);
    //    }

    //    return null;
    //    //}
    //}

    public object ExecuteScalar(string sql)
    {


        var table = new DataTable();
        var conn = Database.GetDbConnection();
        try
        {
            var cmd = conn.CreateCommand();
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            return cmd.ExecuteScalar();
        }
        catch (Exception ex)
        {
            //ExceptionManager.ThrowException(ex);
        }
        finally
        {
            conn.Close();
        }

        return null;

    }



    public IQueryable<T> SqlQuery<T>(string tableName, string where, string orderBy) where T : class
    {
        //using (new ReaderLock(mRw))
        //{
        try
        {
            where = string.IsNullOrEmpty(where) || string.IsNullOrWhiteSpace(where)
                ? string.Empty
                : $"WHERE {where}";
            orderBy = string.IsNullOrEmpty(orderBy) || string.IsNullOrWhiteSpace(orderBy)
                ? string.Empty
                : $"ORDER BY {orderBy}";
            var sql = $"SELECT * FROM {tableName} {where} {orderBy}".Trim();
            return SqlQuery<T>(sql);
            //var set = Set<T>();
            //var sqlQuery = set.SqlQuery(sql).AsNoTracking();
            //return sqlQuery;
        }
        catch (Exception ex)
        {
            //ExceptionManager.ThrowException(ex);
        }

        return null;
        //}
    }

    public IQueryable<T> SqlQuery<T>(string sql) where T : class
    {

        try
        {
            var set = Set<T>();
            return set.FromSqlRaw(sql).AsNoTracking();
        }
        catch (Exception ex)
        {
            //ExceptionManager.ThrowException(ex);
        }

        return null;
        //}
    }

    //public int Update<T>(T o, string[] propertys = null) where T : class
    //{

    //    try
    //    {
    //        var set = Set<T>();
    //        set.Local.Add(o);
    //        Entry(o).CurrentValues.SetValues(o);
    //        Entry(o).State = EntityState.Modified;
    //        if (propertys != null && propertys.Length > 0)
    //        {
    //            foreach (var property in propertys)
    //            {
    //                Entry(o).Property(property).IsModified = true;
    //            }

    //        }
    //        return SaveChanges();
    //    }
    //    catch (Exception ex)
    //    {
    //       ExceptionManager.ThrowException(ex);
    //    }
    //    return 0;

    //}

    public int Update<T>(T o, string[] propertys = null) where T : class
    {

        try
        {
            var set = Set<T>();
            set.Attach(o);
            if (propertys != null && propertys.Length > 0)
            {
                foreach (var property in propertys)
                {
                    Entry(o).Property(property).IsModified = true;
                }
            }
            else
            {
                Entry(o).State = EntityState.Modified;
            }

            return SaveChanges();
        }
        catch (Exception ex)
        {
            //ExceptionManager.ThrowException(ex);
        }
        return 0;

    }

    public int UpdateByPrimaryKeySelective<T>(T record, string[] propertys = null) where T : class
    {

        try
        {
            if (propertys != null && propertys.Length > 0)
            {
                return Update(record, propertys);
            }
            Type type = record.GetType();
            var typeProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var entity = Entry<T>(record);
            List<string> props = new List<string>();
            foreach (var prop in typeProps)
            {
                var isIdsColums = prop.GetCustomAttributes(inherit: true).Any(a => a.GetType().Equals(typeof(IdsColumnAttribute)));
                if (!isIdsColums)
                    continue;
                object val = prop.GetValue(record, null);
                if (val != null)
                {
                    props.Add(prop.Name);
                    entity.Property(prop.Name).IsModified = true;
                }
            }
            if (props.Count() > 0)
            {
                return SaveChanges();
            }
            return 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    public override void AddRange(IEnumerable<object> entities)
    {
        try
        {
            ChangeTracker.Clear();
            base.AddRange(entities);
            SaveChanges();
        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }
    }

    public int InsertRange<T>(List<T> lst) where T : class
    {
        //using (new WriterLock(mRw))
        //{
        try
        {
            if (lst.Count > 0)
            {
                //lst.ForEach(c =>
                //{
                //    Entry(c).State = EntityState.Added;

                //});
                ChangeTracker.Clear();
                base.AddRange(lst);
                return  SaveChanges();


            }

        }
        catch (Exception ex)
        {
            ExceptionManager.ThrowException(ex);
        }
        return 0;
        //}
    }

    public int Update<T>(List<T> lst) where T : class
    {
        //using (new WriterLock(mRw))
        //{
        try
        {

            var set = Set<T>();
            set.UpdateRange(lst);
            //foreach (var o in lst)
            //{
            //    set.Local.Add(o);
            //    Entry(o).CurrentValues.SetValues(o);
            //    Entry(o).State = EntityState.Modified;
            //}

            return SaveChanges();
        }
        catch (Exception ex)
        {
            //ExceptionManager.ThrowException(ex);
        }

        return 0;
        //}
    }

    //public List<T> GetPagedList<T>(Expression<Func<T, bool>> predicate, string orderBy, bool ascending,
    //    int pageIndex, int pageSize, out int totalRecord) where T : class
    //{

    //    totalRecord = 0;
    //    try
    //    {
    //        var query = Query(predicate);
    //        totalRecord = query.Count();
    //        if (totalRecord <= 0) return new List<T>();
    //        if (!string.IsNullOrWhiteSpace(orderBy))
    //        {
    //            query = ascending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
    //        }

    //        query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
    //        return query.ToList();
    //    }
    //    catch (Exception ex)
    //    {
    //        //ExceptionManager.ThrowException(ex);
    //    }

    //    return null;
    //    //}
    //}

    public List<T> GetPagedList<T>(string tableName, string where, string orderBy, int pageIndex, int pageSize,
        out int totalRecord)
        where T : class
    {
        //using (new ReaderLock(mRw))
        //{
        totalRecord = 0;
        try
        {
            var query = SqlQuery<T>(tableName, where, orderBy);
            totalRecord = query.Count();
            if (totalRecord <= 0) return new List<T>();
            var newQuery = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return newQuery.ToList();
        }
        catch (Exception ex)
        {
            //ExceptionManager.ThrowException(ex);
        }
        return null;
        //}
    }


    //private SqlCommand BuildCommand(CommandType cmdType, string cmtText)
    //{
    //    if (!(Database.GetDbConnection().CreateCommand() is SqlCommand cmd)) return null;
    //    cmd.CommandText = cmtText;
    //    cmd.CommandType = cmdType;
    //    return cmd;
    //}

    public int Update(string dbTableName, string set, string where)
    {
        if (dbTableName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(dbTableName));
        if (set.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(set));
        if (where.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(where));
        var sql = $"UPDATE {dbTableName} SET {set} WHERE {where}";
        return Sql(sql);
    }
    public int Delete(string dbTableName, string where)
    {

        if (dbTableName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(dbTableName));
        if (where.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(where));
        var sql = $"DELETE FROM {dbTableName} WHERE {where}";
        return Sql(sql);
    }

    public int Count(string tableName, string where)
    {
        if (tableName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(tableName));
        if (where.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(where));
        var sql = $"SELECT COUNT(1) FROM {tableName} WHERE {where}";
        return (int)ExecuteScalar(sql);
    }

    public override void Dispose()
    {
        // this.Database?.CloseConnection();
        base.Dispose();
        GC.Collect();
    }
}
