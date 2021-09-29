
using Dapper;
using DapperExtensions;
 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class SqlDapperHelper
    {
        static string connStrRead =  ConfigurationManager.AppSettings["Read"];
           
        static string connStrWrite = System.Configuration.ConfigurationManager.AppSettings["Write"];
        //ConfigurationManager.ConnectionStrings["Write"].ConnectionString;

        static int commandTimeout = 30;
        public static IDbConnection GetConnection(bool useWriteConn)
        {
            if (useWriteConn)
                return new SqlConnection(connStrWrite);
            return new SqlConnection(connStrRead);
        }
        public static SqlConnection GetOpenConnection()
        {
            var conn = new SqlConnection(connStrWrite);
            conn.Open();
            return conn;
        }


        /// <summary>
        ///  执行sql返回一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="useWriteConn"></param>
        /// <returns></returns>
        public static T ExecuteReaderReturnT<T>(string sql, object param = null, bool useWriteConn = false, IDbTransaction transaction = null)
        {
            if (transaction == null)
            {
                using (IDbConnection conn = GetConnection(useWriteConn))
                {
                    conn.Open();
                    return conn.QueryFirstOrDefault<T>(sql, param, commandTimeout: commandTimeout);
                }
            }
            else
            {
                var conn = transaction.Connection;
                return conn.QueryFirstOrDefault<T>(sql, param, commandTimeout: commandTimeout, transaction: transaction);
            }

        }
        /// <summary>
        /// 执行sql返回多个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="useWriteConn"></param>
        /// <returns></returns>
        public static List<T> ExecuteReaderReturnList<T>(string sql, object param = null, bool useWriteConn = false, IDbTransaction transaction = null)
        {
            using (IDbConnection conn = GetConnection(useWriteConn))
            {
                conn.Open();

                return conn.Query<T>(sql, param, commandTimeout: commandTimeout, transaction: transaction).ToList();
            }
        }
        ///// <summary>
        ///// 执行sql返回一个对象--异步
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="sql"></param>
        ///// <param name="param"></param>
        ///// <param name="useWriteConn"></param>
        ///// <returns></returns>
        //public static async Task<T> ExecuteReaderRetTAsync<T>(string sql, object param = null, bool useWriteConn = false)
        //{
        //    using (IDbConnection conn = GetConnection(useWriteConn))
        //    {
        //        conn.Open();
        //        return await conn.QueryFirstOrDefaultAsync<T>(sql, param, commandTimeout: commandTimeout).ConfigureAwait(false);
        //    }
        //}
        ///// <summary>
        ///// 执行sql返回多个对象--异步
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="sql"></param>
        ///// <param name="param"></param>
        ///// <param name="useWriteConn"></param>
        ///// <returns></returns>
        //public static async Task<List<T>> ExecuteReaderRetListAsync<T>(string sql, object param = null, bool useWriteConn = false)
        //{
        //    using (IDbConnection conn = GetConnection(useWriteConn))
        //    {
        //        conn.Open();
        //        var list = await conn.QueryAsync<T>(sql, param, commandTimeout: commandTimeout).ConfigureAwait(false);
        //        return list.ToList();
        //    }
        //}
        /// <summary>
        /// 执行sql，返回影响行数 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int ExecuteSqlInt(string sql, object param = null, IDbTransaction transaction = null)
        {
            if (transaction == null)
            {
                using (IDbConnection conn = GetConnection(true))
                {
                    conn.Open();
                    return conn.Execute(sql, param, commandTimeout: commandTimeout, commandType: CommandType.Text);
                }
            }
            else
            {
                var conn = transaction.Connection;
                return conn.Execute(sql, param, transaction: transaction, commandTimeout: commandTimeout, commandType: CommandType.Text);
            }
        }
        ///// <summary>
        ///// 执行sql，返回影响行数--异步
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <param name="param"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //public static async Task<int> ExecuteSqlIntAsync(string sql, object param = null, IDbTransaction transaction = null)
        //{
        //    if (transaction == null)
        //    {
        //        using (IDbConnection conn = GetConnection(true))
        //        {
        //            conn.Open();
        //            return await conn.ExecuteAsync(sql, param, commandTimeout: commandTimeout, commandType: CommandType.Text).ConfigureAwait(false);
        //        }
        //    }
        //    else
        //    {
        //        var conn = transaction.Connection;
        //        return await conn.ExecuteAsync(sql, param, transaction: transaction, commandTimeout: commandTimeout, commandType: CommandType.Text).ConfigureAwait(false);
        //    }
        //}
        /// <summary>
        /// 根据id获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="useWriteConn"></param>
        /// <returns></returns>
        public static T GetById<T>(int id, IDbTransaction transaction = null, bool useWriteConn = false) where T : class
        {
            if (transaction == null)
            {
                using (IDbConnection conn = GetConnection(useWriteConn))
                {
                    conn.Open();
                    return conn.Get<T>(id, commandTimeout: commandTimeout);
                }
            }
            else
            {
                var conn = transaction.Connection;
                return conn.Get<T>(id, transaction: transaction, commandTimeout: commandTimeout);
            }
        }
        /// <summary>
        /// 根据id获取实体--异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="useWriteConn"></param>
        /// <returns></returns>
        //public static async Task<T> GetByIdAsync<T>(int id, IDbTransaction transaction = null, bool useWriteConn = false) where T : class
        //{
        //    if (transaction == null)
        //    {
        //        using (IDbConnection conn = GetConnection(useWriteConn))
        //        {
        //            conn.Open();
        //            return await conn.GetAsync<T>(id, commandTimeout: commandTimeout);
        //        }
        //    }
        //    else
        //    {
        //        var conn = transaction.Connection;
        //        return await conn.GetAsync<T>(id, transaction: transaction, commandTimeout: commandTimeout);
        //    }
        //}

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static void ExecuteInsert<T>(T item, IDbTransaction transaction = null) where T : class
        {
            if (transaction == null)
            {
                using (IDbConnection conn = GetConnection(true))
                {
                    conn.Open();
                    var res = conn.Insert<T>(item, commandTimeout: commandTimeout);
                   
                }
            }
            else
            {
                var conn = transaction.Connection;
                 conn.Insert(item, transaction: transaction, commandTimeout: commandTimeout);
            }
        }

        /// <summary>
        /// 根据sql批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sql"></param>
        /// <param name="transaction"></param>
        public static void Add<T>(IEnumerable<T> list ,string sql, IDbTransaction transaction = null) where T : class
        {
            try
            {
                if (transaction == null)
                {
                    using (IDbConnection conn = GetConnection(true))
                    {
                        conn.Open();
                        var result = conn.Execute(sql, list); //直接传送list对象
                        //conn.Insert<T>(list, commandTimeout: commandTimeout);
                    }
                }
                else
                {
                    var conn = transaction.Connection;
                    var result = conn.Execute(sql, list); //直接传送list对象
                    //conn.Insert(list, transaction: transaction, commandTimeout: commandTimeout);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="transaction"></param>
        public static void ExecuteInsertList<T>(IEnumerable<T> list, IDbTransaction transaction = null) where T : class
        {
            try
            {
                if (transaction == null)
                {
                    using (IDbConnection conn = GetConnection(true))
                    {
                        conn.Open();
                        conn.Insert<T>(list, commandTimeout: commandTimeout);
                    }
                }
                else
                {
                    var conn = transaction.Connection;
                    conn.Insert(list, transaction: transaction, commandTimeout: commandTimeout);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// 更新单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static bool ExecuteUpdate<T>(T item, IDbTransaction transaction = null) where T : class
        {
            if (transaction == null)
            {
                using (IDbConnection conn = GetConnection(true))
                {
                    conn.Open();
                    return conn.Update(item, commandTimeout: commandTimeout);
                }
            }
            else
            {
                var conn = transaction.Connection;
                return conn.Update(item, transaction: transaction, commandTimeout: commandTimeout);
            }
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static bool ExecuteUpdateList<T>(List<T> item, IDbTransaction transaction = null) where T : class
        {
            if (transaction == null)
            {
                using (IDbConnection conn = GetConnection(true))
                {
                    conn.Open();
                    return conn.Update(item, commandTimeout: commandTimeout);
                }
            }
            else
            {
                var conn = transaction.Connection;
                return conn.Update(item, transaction: transaction, commandTimeout: commandTimeout);
            }
        }


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">主sql 不带 order by</param>
        /// <param name="sort">排序内容 id desc，add_time asc</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页多少条</param>
        /// <param name="useWriteConn">是否主库</param>
        /// <returns></returns>
        public static List<T> ExecutePageList<T>(string sql, string sort, int pageIndex, int pageSize, bool useWriteConn = false, object param = null)
        {
            string pageSql = @"SELECT TOP {0} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) _row_number_,*  FROM 
              ({2})temp )temp1 WHERE temp1._row_number_>{3} ORDER BY _row_number_";
            string execSql = string.Format(pageSql, pageSize, sort, sql, pageSize * (pageIndex - 1));
            using (IDbConnection conn = GetConnection(useWriteConn))
            {
                conn.Open();

                return conn.Query<T>(execSql, param, commandTimeout: commandTimeout).ToList();
            }
        }

    }
}