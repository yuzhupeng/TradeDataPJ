/*******************************************************
 * 
 * 作者：fishyue
 * 创建日期：20210405
 * 说明：MongoDb帮助类
 * 运行环境：.NET 4.5
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 fishyue 20210405 11:56
 * 
*******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace CoinWin.DataGeneration
{
    /// <summary>
    /// MongoDb帮助类
    /// </summary>
    public class DB
    {
        private static readonly string connStr = "mongodb://127.0.0.1:27017";//GlobalConfig.Settings["mongoConnStr"];

        //private static readonly string dbName = "PERP";//GlobalConfig.Settings["mongoDbName"];

        private IMongoDatabase db = null;



        private static readonly object lockHelper = new object();

        //private DB() { }

        public IMongoDatabase GetDb(string DBS)
        {
            if (db == null)
            {
                lock (lockHelper)
                {
                    if (db == null)
                    {
                        var client = new MongoClient(connStr);
                        db = client.GetDatabase(DBS);
                    }
                }
            }
            return db;
        }
    }

    public class MongoDbHelper<T> where T : BaseEntity
    {
        public IMongoDatabase db = null;

        public IMongoCollection<T> collection = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbs">库</param>
        /// <param name="tablename">表</param>
        public MongoDbHelper(string dbs, string tablename)
        {
            DB dbq = new DB();
            this.db = dbq.GetDb(dbs);
            collection = db.GetCollection<T>(tablename);
        }

        public MongoDbHelper(string dbs)
        {
            DB dbq = new DB();
            this.db = dbq.GetDb(dbs);           
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Insert(T entity)
        {
            collection.InsertOneAsync(entity);
            return entity;
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void Modify(string id, string field, string value)
        {
            var filter = Builders<T>.Filter.Eq("Id", ObjectId.Parse(id));
            var updated = Builders<T>.Update.Set(field, value);
            UpdateResult result = collection.UpdateOneAsync(filter, updated).Result;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity)
        {
            try
            {
                var old = collection.Find(e => e.Id.Equals(entity.Id)).ToList().FirstOrDefault();

                foreach (var prop in entity.GetType().GetProperties())
                {
                    var newValue = prop.GetValue(entity);
                    var oldValue = old.GetType().GetProperty(prop.Name).GetValue(old);
                    if (newValue != null)
                    {
                        if (oldValue == null)
                            oldValue = "";
                        if (!newValue.ToString().Equals(oldValue.ToString()))
                        {
                            old.GetType().GetProperty(prop.Name).SetValue(old, newValue.ToString());
                        }
                    }
                }
                old.State = "n";
                old.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var filter = Builders<T>.Filter.Eq("Id", entity.Id);
                ReplaceOneResult result = collection.ReplaceOneAsync(filter, old).Result;
            }
            catch (Exception ex)
            {
                var aaa = ex.Message + ex.StackTrace;
                throw;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            var filter = Builders<T>.Filter.Eq("Id", entity.Id);
            collection.DeleteOneAsync(filter);
        }
        /// <summary>
        /// 根据id查询一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T QueryOne(string id)
        {
            return collection.Find(a => a.Id == ObjectId.Parse(id)).ToList().FirstOrDefault();
        }
        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public List<T> QueryAll()
        {
            var sq = collection.Count(a => a.State != "");

            return collection.Find(a => a.State != "").ToList();
        }


        /// <summary>
        /// 根据条件查询多条数据
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        /// 
        public List<T> QueryAllExpress(Expression<Func<T, bool>> express)
        {
            return collection.Find(express).ToList();
        }


        /// <summary>
        /// 根据条件查询一条数据
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        /// 
        public T QueryByFirst(Expression<Func<T, bool>> express)
        {
            return collection.Find(express).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="list"></param>
        public void InsertBatch(List<T> list)
        {
            try
            {
                collection.InsertMany(list);
            }
            catch (Exception e)
            {
                Console.WriteLine("Mongodb批量保存数据出错，错误信息" + e.Message.ToString());

            }
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="list"></param>
        public async Task<bool> InsertBatchAsync(List<T> list)
        {
            try
            {
                await collection.InsertManyAsync(list);

            }
            catch (Exception e)
            {

                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据Id批量删除
        /// </summary>
        public void DeleteBatch(List<ObjectId> list)
        {
            var filter = Builders<T>.Filter.In("Id", list);
            collection.DeleteManyAsync(filter);
        }


        /// <summary>
        /// 从指定的库与表中获取指定条件的数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> express)
        {
            //return await collection.Find(express).ToList();
            var collections = await collection.FindAsync(express);
            return collections.ToList();

        }


        /// <summary>
        /// 未添加到索引的数据
        /// </summary>
        /// <returns></returns>
        public List<T> QueryToLucene()
        {
            return collection.Find(a => a.State.Equals("y") || a.State.Equals("n")).ToList();
        }


        /// <summary>
        /// 返回可查询的记录源
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetQueryable()
        {        
            IQueryable<T> query = collection.AsQueryable();
            return query;
        }

        /// <summary>
        /// 获取表的所有记录数量
        /// </summary>
        /// <returns></returns>
        public virtual int GetRecordCount()
        {
            return GetQueryable().Count();
        }

        /// <summary>
        /// 获取表的所有记录数量
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetqueryList()
        {
            return GetQueryable().ToList();
        }

    }

    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            var flag = ObjectId.GenerateNewId();
            this.Id = flag;
            this.State = "y";
            this.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public ObjectId Id { get; set; }

        public string State { get; set; }

        public string CreateTime { get; set; }

        public string UpdateTime { get; set; }
    }
}