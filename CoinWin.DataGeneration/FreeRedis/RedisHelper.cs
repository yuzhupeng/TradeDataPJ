using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace CoinWin.DataGeneration
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
      
            var redisclient = FreeRedisHelper.CreateInstance(db);

            //using (var redisclient = FreeRedisHelper.CreateInstance())
            //{
            try
            {
                redisclient.Set<List<T>>(key, t);

                //redisclient.Dispose();
            }
            catch (Exception e)
            {
                redisclient.Dispose();
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
          
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
             

            var redisclient = FreeRedisHelper.CreateInstance(db);
            //using (var redisclient = FreeRedisHelper.CreateInstance())
            //{
            try
            {
                redisclient.Set<T>(key, t);
                //redisclient.Dispose();
            }
            catch (Exception e)
            {
                redisclient.Dispose();
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
         
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
          
            var redisclient = FreeRedisHelper.CreateInstance(dbname);

         
            try
            {
               var results= redisclient.SAdd(key, t);
            }
            catch (Exception e)
            {
                redisclient.Dispose();
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                
            }
        }


        /// <summary>
        /// 获取set集合数据--数据量大时会造成
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public  static List<UPermanentFutures> GetSet(string key, string dbname)
        {       
            var redisclient = FreeRedisHelper.CreateInstance(dbname);
            List<UPermanentFutures> lists = new List<UPermanentFutures>();
            try
            {
                var results = redisclient.SMembers<string>(key);
                foreach (var item in results)
                {
                    var res = item.ToList<UPermanentFutures>();
                    lists.AddRange(res);
                }           
            }
            catch (Exception e)
            {

                Console.WriteLine("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            return lists;

        }


        /// <summary>
        /// 获取set集合数据--数据量大时会造成
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public static List<T> GetSetT<T>(string key, string dbname)
        {
            var redisclient = FreeRedisHelper.CreateInstance(dbname);
            List<T> lists = new List<T>();
            try
            {
                var results = redisclient.SMembers<string>(key);
                foreach (var item in results)
                {
                    var res = item.ToList<T>();
                    lists.AddRange(res);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            return lists;

        }

        /// <summary>
        /// 获取set集合数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public static List<T> GetSetSScanObjectT<T>(string key, string dbname)
        {
            var redisclient = FreeRedisHelper.CreateInstance(dbname);
            List<T> lists = new List<T>();

            try
            {
                long a = 0;
                while (true)
                {
                    var resultss = redisclient.SScan(key, a, "*", 10000);

                    //Thread.Sleep(1000);

                    a = resultss.cursor;
                    if (resultss.length == 0)
                    {
                        break;
                    }
                    foreach (var item in resultss.items)
                    {
                        if (item.Contains("["))
                        {
                            var res = item.ToList<T>();
                            lists.AddRange(res);
                        }
                        else
                        {
                            var res = item.ToObject<T>();
                            lists.Add(res);
                        }
                    }

                    if (resultss.cursor == 0)
                    {
                        break;
                    }

                }
 
            }
            catch (Exception e)
            {

                Console.WriteLine("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            return lists;

        }

        /// <summary>
        /// 获取set集合数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public static List<string> GetSetSScanObjectstring(string key, string dbname)
        {
            var redisclient = FreeRedisHelper.CreateInstance(dbname);
            List<string> lists = new List<string>();

            try
            {
                long a = 0;
                while (true)
                {
                    var resultss = redisclient.SScan(key, a, "*", 100000);

                    //Thread.Sleep(1000);

                    a = resultss.cursor;
                    if (resultss.length == 0)
                    {
                        break;
                    }

                    lists.AddRange(resultss.items);
                  
                    if (resultss.cursor == 0)
                    {
                        break;
                    }

                }

            }
            catch (Exception e)
            {

                Console.WriteLine("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            return lists;

        }






        /// <summary>
        /// 获取set集合数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public static List<UPermanentFutures> GetSetSScan(string key, string dbname)
        {


            var redisclient = FreeRedisHelper.CreateInstance(dbname);
            List<UPermanentFutures> lists = new List<UPermanentFutures>();

            try
            {
                long a = 0;
                while (true)
                {
                    var resultss = redisclient.SScan(key, a, "*", 10000);

                    //Thread.Sleep(1000);

                    a = resultss.cursor;
                    if (resultss.length == 0)
                    {
                        break;
                    }
                    foreach (var item in resultss.items)
                    {
                        var res = item.ToList<UPermanentFutures>();
                        lists.AddRange(res);
                    }

                    if (resultss.cursor == 0)
                    {
                        break;
                    }

                }

                    //var counts = redisclient.SCard(key);
                    //for (int i = 0; i < counts; i += 10000)
                    //{

                    //    var resultss = redisclient.SScan(key, i, "*", 10000);
                    //    foreach (var item in resultss.items)
                    //    {
                    //        var res = item.ToList<UPermanentFutures>();
                    //        lists.AddRange(res);
                    //    }


                    //}
                    //long a = 0;

                    //var counts = redisclient.SCard(key);
                    //while (true)
                    //{              
                    //    var resultss = redisclient.SScan(key, a, "*", 10000);
                    //    long c= a + 10000;
                    //    if (c > counts)
                    //    {
                    //        a = counts - a;
                    //        a = counts - a;
                    //    }
                    //    else
                    //    {
                    //        a = c;
                    //    }

                    //    if (resultss.length == 0)
                    //    {
                    //        break;
                    //    }
                    //    foreach (var item in resultss.items)
                    //    {
                    //        var res = item.ToList<UPermanentFutures>();
                    //        lists.AddRange(res);
                    //    }


                    //}






                    //var results = redisclient.SScan<string>(key,);
                    //foreach (var item in results)
                    //{
                    //    var res = item.ToList<UPermanentFutures>();
                    //    lists.AddRange(res);
                    //}



                }
            catch (Exception e)
            {

                Console.WriteLine("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            return lists;

        }



        /// <summary>
        /// 获取set集合数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public static List<UPermanentFutures> GetSetSScanObject(string key, string dbname)
        {


            var redisclient = FreeRedisHelper.CreateInstance(dbname);
            List<UPermanentFutures> lists = new List<UPermanentFutures>();

            try
            {
                long a = 0;
                while (true)
                {
                    var resultss = redisclient.SScan(key, a, "*", 10000);

                    //Thread.Sleep(1000);

                    a = resultss.cursor;
                    if (resultss.length == 0)
                    {
                        break;
                    }
                    foreach (var item in resultss.items)
                    {
                        if (item.Contains("["))
                        {
                            var res = item.ToList<UPermanentFutures>();
                            lists.AddRange(res);
                        }
                        else
                        {
                            var res = item.ToObject<UPermanentFutures>();
                            lists.Add(res);
                        }
                    }

                    if (resultss.cursor == 0)
                    {
                        break;
                    }

                }

                //var counts = redisclient.SCard(key);
                //for (int i = 0; i < counts; i += 10000)
                //{

                //    var resultss = redisclient.SScan(key, i, "*", 10000);
                //    foreach (var item in resultss.items)
                //    {
                //        var res = item.ToList<UPermanentFutures>();
                //        lists.AddRange(res);
                //    }


                //}
                //long a = 0;

                //var counts = redisclient.SCard(key);
                //while (true)
                //{              
                //    var resultss = redisclient.SScan(key, a, "*", 10000);
                //    long c= a + 10000;
                //    if (c > counts)
                //    {
                //        a = counts - a;
                //        a = counts - a;
                //    }
                //    else
                //    {
                //        a = c;
                //    }

                //    if (resultss.length == 0)
                //    {
                //        break;
                //    }
                //    foreach (var item in resultss.items)
                //    {
                //        var res = item.ToList<UPermanentFutures>();
                //        lists.AddRange(res);
                //    }


                //}






                //var results = redisclient.SScan<string>(key,);
                //foreach (var item in results)
                //{
                //    var res = item.ToList<UPermanentFutures>();
                //    lists.AddRange(res);
                //}



            }
            catch (Exception e)
            {

                Console.WriteLine("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            return lists;

        }



        /// <summary>
        /// 根据key删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool DeleteKeys(string key)
        {
            bool result = true;
            var redisclient = FreeRedisHelper.CreateInstance("");
            
            try
            {

                redisclient.Del(key);

            }
            catch (Exception e)
            {
                

                LogHelper.WriteLog(typeof(DownExchangeData), "删除数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                return false;
            }
            return result;
        }

        ///// <summary>
        ///// 获取set集合数据
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="dbname"></param>
        ///// <returns></returns>
        //public static List<UPermanentFutures> GetSscan(string key, string dbname)
        //{


        //    var redisclient = FreeRedisHelper.CreateInstance(dbname);
        //    List<UPermanentFutures> lists = new List<UPermanentFutures>();

        //    try
        //    {
        //        var results = redisclient.SScan<string>(key,);
        //        foreach (var item in results)
        //        {
        //            var res = item.ToList<UPermanentFutures>();
        //            lists.AddRange(res);
        //        }



        //    }
        //    catch (Exception e)
        //    {
        //        redisclient.Dispose();

        //        LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
        //    }
        //    return lists;

        //}
        public static void SaveData(List<UPermanentFuturesModel> list)
        {
             
                using (SqlConnection con = SqlDapperHelper.GetOpenConnection())
                {
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            SqlDapperHelper.ExecuteInsertList(list, transaction);
                            transaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog(typeof(object), "保存QIHUO数据发生错误，错误信息:" + ex.Message.ToString());
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
           

        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filed"></param>
        public static string GetHash(string key, string filed)
        {
            //var log = LogHelper.CreateInstance();
            var redisclient = FreeRedisHelper.CreateInstance("");
            string results = "";

            try
            {
                results = redisclient.HMGet(key, filed)[0];
            }
            catch (Exception e)
            {
                //redisclient.Dispose();
                Console.WriteLine("获取hash redis失败,失败的key:" + key + filed+ "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog("获取hash redis失败,失败的key:" + key + filed + "失败原因:" + e.Message.ToString());
            }
            return results;
        }

        public static Dictionary<string, T> GetAllHash<T>(string key)
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
                Console.WriteLine("获取所有hash redis失败,失败的key:" + key +"失败原因:" + e.Message.ToString());
                LogHelper.WriteLog("获取所有hash redis失败,失败的key:" + key + "失败原因:" + e.Message.ToString());
            }
            return result;
        }


        public static List<T> GetAllHashList<T>(string key)
        {
            //var log = LogHelper.CreateInstance();
            var redisclient = FreeRedisHelper.CreateInstance("");
            Dictionary<string, T> result = new Dictionary<string, T>();
            List<T> res = new List<T>();
            try
            {
                result = redisclient.HGetAll<T>(key);
            }
            catch (Exception e)
            {
                //redisclient.Dispose();
                Console.WriteLine("获取所有hash redis失败,失败的key:" + key + "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog("获取所有hash redis失败,失败的key:" + key + "失败原因:" + e.Message.ToString());
            }
            res.AddRange(result.Values);
 

            //foreach (var item in result)
            //{
            //    res.Add()
            //}
            return res;
        }


        /// <summary>
        /// 更新或插入hash值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filed"></param>
        /// <param name="value"></param>
        public static void SetOrUpdateHash(string key, string filed, string value)
        {
            //var log = LogHelper.CreateInstance();
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
                LogHelper.WriteLog("保存hash数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
        }
    }
}

