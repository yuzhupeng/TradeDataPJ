using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public class RedisHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">数据类</param>
        /// <param name="type">平台</param>
        /// <param name="key">键值</param>
        public static void SaveList<T>(List<T> t, string db, string key)
        {
            var log = LogHelper.CreateInstance();
            var redisclient = FreeRedisHelper.CreateInstance(db);

            //using (var redisclient = FreeRedisHelper.CreateInstance())
            //{
            try
            {
                redisclient.Set<List<T>>(key, t);
                    
            }
            catch (Exception e)
            {
                Console.WriteLine("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                log.Error("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">数据类</param>
        /// <param name="type">平台</param>
        /// <param name="key">键值</param>
        public static void Save<T>(T t, string db, string key)
        {
            var log = LogHelper.CreateInstance();

            var redisclient = FreeRedisHelper.CreateInstance(db);
            //using (var redisclient = FreeRedisHelper.CreateInstance())
            //{
            try
            {
                redisclient.Set<T>(key, t);
       
            }
            catch (Exception e)
            {
                Console.WriteLine("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                //redisclient.Dispose();
                log.Error("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            //}
        }


        /// <summary>
        /// 向list中末端插入列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="dbname"></param>
        /// <param name="key"></param>
        public static void Pushdata(string t, string dbname, string key)
        {
            var log = LogHelper.CreateInstance();
            var redisclient = FreeRedisHelper.CreateInstance(dbname);
            //if (key != CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"))
            //{
            //    return;
            //}



            try
            {
               var results= redisclient.SAdd(key, t);
            }
            catch (Exception e)
            {
             
                Console.WriteLine("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                log.Error("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 获取set集合数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public static List<UPermanentFutures> GetSet(string key, string dbname)
        {

            var log = LogHelper.CreateInstance();
            var redisclient = FreeRedisHelper.CreateInstance(dbname);
            List<UPermanentFutures> lists = new List<UPermanentFutures>();

            try
            {
              var   results = redisclient.SMembers<List<UPermanentFutures>>(key);
            }
            catch (Exception e)
            {
                //redisclient.Dispose();
                Console.WriteLine("获取redis数据失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                log.Error("获取redis数据失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            return lists;

        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filed"></param>
        public static string GetHash(string key, string filed)
        {
            var log = LogHelper.CreateInstance();
            var redisclient = FreeRedisHelper.CreateInstance("");
            string  results = "";

            try
            {
              results = redisclient.HMGet(key, filed)[0];
            }
            catch (Exception e)
            {
                //redisclient.Dispose();
                Console.WriteLine("获取hash redis失败,失败的key" + key + filed+"失败原因:" + e.Message.ToString());
                log.Error("获取hash redis失败,失败的key" + key + filed+"失败原因:" + e.Message.ToString());
            }
            return results;
        }

        /// <summary>
        /// 更新或插入hash值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filed"></param>
        /// <param name="value"></param>
        public static void SetOrUpdateHash(string key,string filed,string value)
        {
            var log = LogHelper.CreateInstance();
            var redisclient = FreeRedisHelper.CreateInstance("");
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add(filed, value);              
                var results = redisclient.HSet<string>(key, dic);
            }
            catch (Exception e)
            {
                //redisclient.Dispose();
                Console.WriteLine("保存hash数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                log.Error("保存hash数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 获取hash值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filed"></param>
        /// <param name="value"></param>
        public static Dictionary<string, string> GetALLHash(string key)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var log = LogHelper.CreateInstance();
            var redisclient = FreeRedisHelper.CreateInstance("");
            try
            {              
                dic = redisclient.HGetAll<string>(key);
            }
            catch (Exception e)
            {
                //redisclient.Dispose();
                Console.WriteLine("HUOQUhash数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                log.Error("HUOQUhash数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            return dic;
        }

        public static Dictionary<string, T> GetAllHashs<T>(string key)
        {
            //var log = LogHelper.CreateInstance();
            var redisclient = FreeRedisHelper.CreateInstance("");
            Dictionary<string, T> result = new Dictionary<string, T>();

            try
            {
                result = redisclient.HGetAll<T>(key);
            }
            catch (Exception e)
            {
                //redisclient.Dispose();
                Console.WriteLine("获取所有hash redis失败,失败的key:" + key + "失败原因:" + e.Message.ToString());
                
            }
            return result;
        }

    }
}

