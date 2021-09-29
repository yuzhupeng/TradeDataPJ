using FreeRedis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GetTradeHistoryData
{
    // /// <summary>
    // /// 缓存帮助类
    // /// </summary>
    //public class FreeRedisHelper
    // {
    //     private static RedisClient ConnentClient=null;
    //     private static readonly object lockHelper = new object();
    //     public static RedisClient CreateInstance(string db)
    //     {
    //         if (ConnentClient == null)
    //         {
    //             lock (lockHelper)
    //             {
    //                 ConnentClient = new RedisClient("127.0.0.1:6379,password=test123,defaultDatabase=DB0");
    //                 //ConnentClient.Notice += (s, e) => Console.WriteLine(e.Log);
    //             }
    //         }
    //         return ConnentClient;
    //     }

    //     /// <summary>
    //     /// 释放
    //     /// </summary>
    //     public static void  Dispose()
    //     {
    //         ConnentClient.Dispose();
    //     }
    // }


    /// <summary>
    /// 缓存帮助类
    /// </summary>
    public class FreeRedisHelper
    {
        private static RedisClient ConnentClient = null;
        private static readonly object lockHelper = new object();

        protected static ConnectionStringBuilder Connection = new ConnectionStringBuilder()
        {
            Host = "127.0.0.1:6379",
            Password = "test123",
            Database = 0,
            MaxPoolSize = 10,
            Retry =1000000,
            Protocol = RedisProtocol.RESP2,
            ClientName = "FreeRedis"
        };
        //static Lazy<RedisClient> _cliLazy = new Lazy<RedisClient>(() =>
        //{

        //    var r = new RedisClient(Connection); //redis 6.0
        //   // var r = new RedisClient(connectionString); //redis 6.0
        //    r.Serialize = obj => JsonConvert.SerializeObject(obj);
        //    r.Deserialize = (json, type) => JsonConvert.DeserializeObject(json, type);
        //    r.Notice += (s, e) => Trace.WriteLine(e.Log);
        //    return r;
        //});

        static Lazy<RedisClient> _cliLazy = new Lazy<RedisClient>(() =>
        {
            //var r = new RedisClient(new ConnectionStringBuilder[] { "127.0.0.1:6379,database=1,password=123" }); //redis 3.2 cluster
            //var r = new RedisClient("127.0.0.1:6379,database=1"); //redis 3.2
            //var r = new RedisClient("127.0.0.1:6379,database=1", "127.0.0.1:6379,database=1");
            var r = new RedisClient(Connection); //redis 6.0
                                                 // var r = new RedisClient(connectionString); //redis 6.0
            r.Serialize = obj => JsonConvert.SerializeObject(obj);
            r.Deserialize = (json, type) => JsonConvert.DeserializeObject(json, type);
            r.Notice += (s, e) => Trace.WriteLine(e.Log);
            return r;
        });


        public static RedisClient CreateInstance(string db = "")
        {


            //RedisClient cli = new RedisClient($"127.0.0.1:6379,password=test123,defaultDatabase=DB0");
            //cli.Notice += (s, e) => Console.WriteLine(e.Log);

            //ConnentClient = cli;
            //try
            //{

            //    var strings = cli.Ping();

            //}
            //catch (Exception e)
            //{

            //}
            return _cliLazy.Value;

        }

        /// <summary>
        /// 释放
        /// </summary>
        public static void Dispose()
        {
            ConnentClient.Dispose();
        }
    }



    public class TestBase
    {
        protected static ConnectionStringBuilder Connection = new ConnectionStringBuilder()
        {
            Host = "127.0.0.1:6379",
            Password = "123456",
            Database = 1,
            MaxPoolSize = 10,
            Protocol = RedisProtocol.RESP2,
            ClientName = "FreeRedis"
        };
        static Lazy<RedisClient> _cliLazy = new Lazy<RedisClient>(() =>
        {
            //var r = new RedisClient(new ConnectionStringBuilder[] { "127.0.0.1:6379,database=1,password=123" }); //redis 3.2 cluster
            //var r = new RedisClient("127.0.0.1:6379,database=1"); //redis 3.2
            //var r = new RedisClient("127.0.0.1:6379,database=1", "127.0.0.1:6379,database=1");
            var r = new RedisClient(Connection); //redis 6.0
                                                 // var r = new RedisClient(connectionString); //redis 6.0
            r.Serialize = obj => JsonConvert.SerializeObject(obj);
            r.Deserialize = (json, type) => JsonConvert.DeserializeObject(json, type);
            //r.Notice += (s, e) => Trace.WriteLine(e.Log);
            return r;
        });
        public static RedisClient cli => _cliLazy.Value;



        //static Lazy<RedisClient> _cliLazy = new Lazy<RedisClient>(() => new RedisClient("127.0.0.1:6379,database=1", "127.0.0.1:6379,database=1"));
    }



}
