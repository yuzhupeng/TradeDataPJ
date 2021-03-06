using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;


namespace CoinWin.DataGeneration
{
    /// <summary>
    /// 封装向业务层提供操作MongoDB的操作方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        /// <summary>
        /// 从指定的库与表中获取指定条件的数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string dbName, string tableName)
        {
            var dbContext = new MongoDbContext(dbName);
            var collection = await dbContext.GetCollectionAsync<T>(tableName);
            return collection.AsQueryable<T>().Where(predicate).ToList();
        }


        /// <summary>
        /// 从指定的库与表中获取指定条件的数据
        /// </summary>
        /// <returns></returns>
        public  List<T> GetList( string dbName, string tableName)
        {
            var dbContext = new MongoDbContext(dbName);
            var collection =  dbContext.GetCollection<T>(tableName);
            return collection.AsQueryable<T>().ToList();
        }




        /// <summary>
        /// 对指定的库与表中新增数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddList(List<T> list, string dbName, string tableName = "")
        {
            var dbContext = new MongoDbContext(dbName);
            var collection = await dbContext.GetCollectionAsync<T>(tableName);
            await collection.InsertManyAsync(list);
            return true;
        }


        /// <summary>
        /// 对指定的库与表中新增数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Add(T obj, string dbName, string tableName = "")
        {
            var dbContext = new MongoDbContext(dbName);
            var collection = await dbContext.GetCollectionAsync<T>(tableName);
            collection.InsertOne(obj);
            return true;
        }

      



    }
}