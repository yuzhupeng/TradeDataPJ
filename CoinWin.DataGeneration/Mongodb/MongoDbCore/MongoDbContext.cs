using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CoinWin.DataGeneration
{
    /// <summary>
    /// MongoDB对象的上下文
    /// </summary>
    public class MongoDbContext
    {
        /// <summary>
        /// Mongo上下文 
        /// </summary>
        public IMongoDatabase DbContext { get; }
        /// <summary>
        /// 初始化MongoDb数据上下文
        /// 将数据库名传递进来
        /// </summary>
        public MongoDbContext(string dbName)
        {
            //连接字符串，如："mongodb://username:password@host:port/[DatabaseName]?ssl=true"
            //建议放在配置文件中
            //"mongodb://root:a123@192.168.1.6:27017";
            var connectionString = "mongodb://127.0.0.1:27017";
            try
            {
                var mongoClient = new MongoClient(connectionString);
                //数据库如果不存在，会自动创建
                DbContext = mongoClient.GetDatabase(dbName);
            }
            catch (Exception e)
            {
                //Log.WriteLogByError("构建MongoDbContext出错", e);
                throw;
            }
        }

        /// <summary>
        /// 异步获取表（集合）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public async Task<IMongoCollection<TEntity>> GetCollectionAsync<TEntity>(string tableName = "") where TEntity : class
        {


            var dt = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(tableName))
            {

                dt = tableName;
            }

            // 获取集合名称，使用的标准是在实体类型名后添加日期
            var collectionName = dt;

            // 如果集合不存在，那么创建集合
            if (false == await IsCollectionExistsAsync<TEntity>(collectionName))
            {
                await DbContext.CreateCollectionAsync(collectionName);
            }


            return DbContext.GetCollection<TEntity>(collectionName);
        }



        /// <summary>
        /// 异步获取表（集合）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public  IMongoCollection<TEntity> GetCollection<TEntity>(string tableName = "") where TEntity : class
        {


            var dt = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(tableName))
            {

                dt = tableName;
            }

            // 获取集合名称，使用的标准是在实体类型名后添加日期
            var collectionName = dt;

         


            return DbContext.GetCollection<TEntity>(collectionName);
        }




        /// <summary>
        /// 异步获取表（集合）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public IMongoCollection<TEntity> create<TEntity>(string tableName = "") where TEntity : class
        {


            var dt = DateTime.Now.ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(tableName))
            {

                dt = tableName;
            }

            // 获取集合名称，使用的标准是在实体类型名后添加日期
            var collectionName = dt;

            // 如果集合不存在，那么创建集合

            if (IsCollectionExists<TEntity>(collectionName))
            {
                  DbContext.CreateCollection(collectionName);
            }


            return DbContext.GetCollection<TEntity>(collectionName);
        }





        /// <summary>
        /// 集合是否存在
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public async Task<bool> IsCollectionExistsAsync<TEntity>(string name)
        {
            var filter = new BsonDocument("name", name);
            // 通过集合名称过滤
            var collections = await DbContext.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            // 检查是否存在
            return await collections.AnyAsync();
        }

        /// <summary>
        /// 集合是否存在
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public  bool IsCollectionExists<TEntity>(string name)
        {
            var filter = new BsonDocument("name", name);
            // 通过集合名称过滤
            var collections =  DbContext.ListCollections(new ListCollectionsOptions { Filter = filter });
            // 检查是否存在
            return true;
        }


    }
}