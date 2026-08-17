using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using System.Xml;
using ZstdSharp.Unsafe;
using log4net;

namespace IDS.Persistence
{
    public static class EFCoreExtentions
    {
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ExecuteSqlQuery(this DbContext dbContext, string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] parameters)
        {
            using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open(); //打开连接
                }
                //添加输入参数
                cmd.Parameters.AddRange(parameters);

                //执行命令，读取器读取数据
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        IDictionary<string, object> row = new ExpandoObject(); //实例化一个动态可扩展对象
                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            row.Add(dataReader.GetName(i), dataReader[i]);
                        }
                        yield return row;
                    }
                }
            }
        }

        /// <summary>
        /// 执行增、删、改的命令
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(this DbContext dbContext, string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] parameters)
        {
            //1. 创建连接对象
            using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
            {
                //接下来把异常处理加入
                try
                {
                    cmd.CommandText = cmdText;
                    cmd.CommandType = cmdType;
                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        cmd.Connection.Open(); //打开连接
                    }
                    //处理输入参数
                    cmd.Parameters.AddRange(parameters);

                    //事务
                    //cmd.Transaction = tran; 

                    int result = cmd.ExecuteNonQuery();   //执行增删改命令
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static DataTable SelectDataTable(this DbContext dbContext, string cmdText, SqlParameter[] parameters, CommandType cmdType,SqlTransaction sqlTransaction=null)
        {

            var conn = dbContext.Database.GetDbConnection() as SqlConnection;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open(); //打开连接
            }
            using (SqlCommand cmd = new SqlCommand(cmdText, conn))
            {
                if (cmdType == null)
                    cmdType = CommandType.Text;
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                if(sqlTransaction!=null)
                    cmd.Transaction = sqlTransaction;
                cmd.Parameters.AddRange(parameters);
                DataTable table = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(table);
                return table;
            }
        }

        public static DataSet SelectDataSet(this DbContext dbContext, string cmdText, SqlParameter[] parameters, CommandType cmdType, SqlTransaction sqlTransaction = null)
        {

            var conn = dbContext.Database.GetDbConnection() as SqlConnection;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open(); //打开连接
            }
            using (SqlCommand cmd = new SqlCommand(cmdText, conn))
            {
                if (cmdType == null)
                    cmdType = CommandType.Text;
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                if (sqlTransaction != null)
                    cmd.Transaction = sqlTransaction;
                cmd.Parameters.AddRange(parameters);
                DataSet sd = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(sd);
                return sd;
            }
        }



        public static DataTable SelectDataTable(this DbContext dbContext, string cmdText, SqlParameter[] parameters, CommandType cmdType = CommandType.Text)
        {

            var conn = dbContext.Database.GetDbConnection() as SqlConnection;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open(); //打开连接
            }
            using (SqlCommand cmd = new SqlCommand(cmdText, conn))
            {
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                cmd.Parameters.AddRange(parameters);
                DataTable table = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(table);
                return table;
            }
        }

        public static DataSet SelectDataSet(this DbContext dbContext, string cmdText, SqlParameter[] parameters, CommandType cmdType = CommandType.Text)
        {

            var conn = dbContext.Database.GetDbConnection() as SqlConnection;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open(); //打开连接
            }
            using (SqlCommand cmd = new SqlCommand(cmdText, conn))
            {
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                cmd.Parameters.AddRange(parameters);
                DataSet sd = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(sd);
                return sd;
            }
        }


        public static int BulkInsert(this DbContext dbContext, string tableName, List<Dictionary<string, object>> list, SqlTransaction sqlTransaction = null)
        {
            ILog Log = LogManager.GetLogger(typeof(EFCoreExtentions));

            var conn = dbContext.Database.GetDbConnection() as SqlConnection;
            var dataTable = new DataTable();
            if (list == null || list.Count == 0)
            {
                return 0;
            }
            var map = list[0];
            foreach (var item in map)
            {
                dataTable.Columns.Add(new DataColumn(item.Key));
            } 
            list.ForEach(dictItem =>
            {
                DataRow dataRow = dataTable.NewRow();
                foreach (var item in dictItem)
                {
                    dataRow[item.Key] = item.Value;
                }
                dataTable.Rows.Add(dataRow);
            });
            if (conn.State != ConnectionState.Open)
            {
                conn.Open(); //打开连接
            }

            if (sqlTransaction != null)
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.KeepNulls, sqlTransaction))
                {
                    try
                    {
                        //执行命令，读取器读取数据
                        bulkCopy.BatchSize = list.Count;
                        foreach (var item in map)
                        {
                            bulkCopy.ColumnMappings.Add(item.Key, item.Key);
                        }
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.WriteToServer(dataTable);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("数据批量复制报错|An error was found when you copied table data in batches:" + ex.Message ?? ex.InnerException?.Message);
                        return 0;
                    }

                }
                return list.Count;
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
            {
                try
                {
                    //执行命令，读取器读取数据
                    bulkCopy.BatchSize = list.Count;
                    foreach (var item in map)
                    {
                        bulkCopy.ColumnMappings.Add(item.Key, item.Key);
                    }
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(dataTable);
                }
                catch (Exception ex)
                {
                    Log.Error("数据批量复制报错|An error was found when you copied table data in batches:" + ex.Message ?? ex.InnerException?.Message);
                    return 0;
                }

            }
            return list.Count;
        }


        public static int BulkInsert(this DbContext dbContext, string tableName, DataTable list, SqlTransaction sqlTransaction = null)
        {
            ILog Log = LogManager.GetLogger(typeof(EFCoreExtentions));

            var conn = dbContext.Database.GetDbConnection() as SqlConnection;
            //var dataTable = new DataTable();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open(); //打开连接
            }
            if (sqlTransaction != null) {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.KeepNulls, sqlTransaction))
                {
                    try
                    {
                        //执行命令，读取器读取数据
                        bulkCopy.BatchSize = list.Rows.Count;
                        foreach (DataColumn item in list.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                        }
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.WriteToServer(list);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("数据批量复制报错|An error was found when you copied table data in batches:" + ex.Message ?? ex.InnerException?.Message);
                        return 0;
                    }

                }
                return list.Rows.Count;
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
            {
                try
                {
                    //执行命令，读取器读取数据
                    bulkCopy.BatchSize = list.Rows.Count;
                    foreach (DataColumn item in list.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                    }
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(list);
                }
                catch (Exception ex)
                {
                    Log.Error("数据批量复制报错|An error was found when you copied table data in batches:" + ex.Message ?? ex.InnerException?.Message);
                    return 0;
                }

            }
            return list.Rows.Count;
        }

    }
}
