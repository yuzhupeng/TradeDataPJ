using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{


    //1.存储dict集合 2.存储list
    public class MongoDbHelpers
    {



        public void SaveMaxOrder()
        {

            MaxData l = new MaxData();
            //DateTime dt = l.GetLastTime();

            DateTime st = new DateTime(2021, 02, 21);
            DateTime end = DateTime.Now;

            var list = TimeCore.GetDate(st, end);

            string table = CommandEnum.RedisKey.MaxOrder + st.ToString("yyyy");
            foreach (var item in list)
            {
                List<MaxOrders> maxlist = new List<MaxOrders>();

                var tablename = CommandEnum.RedisKey.MaxOrder + item.ToString("yyyy-MM-dd");
                MaxData geter = new MaxData();
                //maxlist = geter.GetMaxData(tablename);
                if (maxlist != null)
                {
                    MongoDbHelper<MaxOrders> max = new MongoDbHelper<MaxOrders>(CommandEnum.RedisKey.MaxOrder, table);
                    max.InsertBatch(maxlist);
                }
            }



        }

        public void GetMaxOrder()
        {
            MaxData l = new MaxData();
            //DateTime dt = l.GetLastTime();

            DateTime st = new DateTime(2021, 02, 21);
            DateTime end = DateTime.Now;

            var list = TimeCore.GetDate(st, end);

            List<MaxOrders> maxlist = new List<MaxOrders>();
            foreach (var item in list)
            {

                var tablename = CommandEnum.RedisKey.MaxOrder + item.ToString("yyyy-MM-dd");
                //MaxData geter = new MaxData();
                //maxlist = geter.GetMaxData(tablename);
                if (maxlist != null)
                {
                    MongoDbHelper<MaxOrders> max = new MongoDbHelper<MaxOrders>(CommandEnum.RedisKey.MaxOrder, tablename);
                    maxlist.AddRange(max.QueryAll());
                }
            }
            var listss = maxlist;
        }

        public async void GetMaxOrders()
        {
            DateTime st = new DateTime(2021, 02, 21);

            List<MaxOrders> maxlist = new List<MaxOrders>();
            var tablename = CommandEnum.RedisKey.MaxOrder + st.ToString("yyyy");

            MongoDbHelper<MaxOrders> max = new MongoDbHelper<MaxOrders>(CommandEnum.RedisKey.MaxOrder, tablename);

            var list = await max.GetListAsync(p => p.times > new DateTime(2021, 03, 05));

            foreach (var item in list)
            {
                Console.WriteLine(item.market + item.qty);
                break;
            }


            //maxlist.AddRange(max.QueryAllExpress(p=>p.times>new DateTime(2021,03,05)).OrderByDescending(p=>p.CreateTime));


            Console.WriteLine("ss");
        }

        public void Getlqdata()
        {
            var tablename = CommandEnum.RedisKey.liquidation + new DateTime(2021, 04, 15).ToString("yyyy-MM-dd");
           // MongoRepository<Liquidations> a = new MongoRepository<Liquidations>();
           //var listw= a.GetList(CommandEnum.RedisKey.liquidation,tablename);


            //var tablename = CommandEnum.RedisKey.liquidation + new DateTime(2021, 04, 15).ToString("yyyy-MM-dd");
            MongoDbHelper<Liquidations> max = new MongoDbHelper<Liquidations>(CommandEnum.RedisKey.liquidation, tablename);
           var counts= max.GetRecordCount();
            var list = max.GetqueryList();



            var lqdatalist = max.QueryAll();
        }



        public void test()
        {
            var client = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase database = client.GetDatabase("PERP");

            string keys = "PERP" + "ALLCoinALLExchange" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            string key = "binance_2021-04-05 09_1INCHUSDT";

            var collection1 = database.GetCollection<Dictionary<string, PermanentFutures>>(keys);
            var collection = database.GetCollection<PermanentFuture>(key);

            var documents = collection.Find(a => a.exchange == "binance").ToList();

            var documentss = collection1.Find(a => a.Values != null).ToList();
            //var list = collection1.Find(new BsonDocument()).ToCursor().ToEnumerable();
            //foreach (var item in list)
            //{
            //    Console.WriteLine(item);
            //}
        }

        public void SaveData()
        {
            string key = "PERP" + "ALLCoinALLExchange" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            var hourtradeMasterlist = RedisHelper.GetAllHash<PermanentFutures>(key);

            List<PermanentFutures> perlist = new List<PermanentFutures>();

            MongoRepository<Dictionary<string, PermanentFutures>> SS = new MongoRepository<Dictionary<string, PermanentFutures>>();
            var SQ = SS.Add(hourtradeMasterlist, "PERP", key);
            Console.WriteLine(SQ);
        }

        public void savedate()
        {
            string key = "PERP" + "ALLCoinALLExchange" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            var hourtradeMasterlist = RedisHelper.GetAllHash<PermanentFutures>(key);

            MongoDbContext t = new MongoDbContext("PERP");
            var insert = t.create<Dictionary<string, PermanentFutures>>(key);
            insert.InsertOne(hourtradeMasterlist);
            Console.WriteLine("sq");
        }

        public void GetData()
        {
            string key = "PERP" + "ALLCoinALLExchange" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");

            MongoDbContext t = new MongoDbContext("PERP");
            var datas = t.GetCollection<Dictionary<string, PermanentFutures>>(key);
            var DATSQ = datas.AsQueryable();
            var sqwyy = DATSQ.ToJson();
            //return collection.AsQueryable<T>().Where(predicate).ToList();

            //MongoRepository<Dictionary<string, PermanentFutures>> getdata = new MongoRepository<Dictionary<string, PermanentFutures>>();
            //var sq = getdata.GetListAsync(a => a.Keys.Contains(""), "PERP", key);
            //var sqw = sq;
            //var collection1 = database.GetCollection<Test1>("Test");
        }

        public void gettdas()
        {

            string key = "PERP" + "ALLCoinALLExchange" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");

            MongoRepository<Dictionary<string, PermanentFutures>> getdata = new MongoRepository<Dictionary<string, PermanentFutures>>();
            var sq = getdata.GetListAsync(a => a.Keys.Contains(""), "PERP", key);

            //var list = await _IMongoRepository.GetListAsync(p => p.Context.IndexOf("测试企业") > -1, dbName, tableName);
            // return list;
        }


        public async void Savedatas()
        {
            string key = "PERP" + "ALLCoinALLExchange" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            var hourtradeMasterlist = RedisHelper.GetAllHash<PermanentFutures>(key);

            List<PermanentFutures> perlist = new List<PermanentFutures>();

            MongoRepository<PermanentFutures> helper = new DataGeneration.MongoRepository<PermanentFutures>();
            //var SQ = SS.Add(hourtradeMasterlist, "PERP", key);




            foreach (var item in hourtradeMasterlist)
            {

                bool sqs = await helper.Add(item.Value, "PERP", item.Key);
                Console.WriteLine(sqs + item.Key);
            }

        }

    }
}
