using FreeRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    public class UpLoadDataToServer
    {
        //public static  RedisClient cli = new RedisClient("127.0.0.1:6148,password=@tosbin.*0925,defaultDatabase=01");



        //protected static ConnectionStringBuilder Connection = new ConnectionStringBuilder()
        //{
        //    Host = "127.0.0.1:6379",
        //    Password = "test123",
        //    Database = 0,
        //    MaxPoolSize = 10,
        //    Retry = 1000000,
        //    Protocol = RedisProtocol.RESP2,
        //    ClientName = "FreeRedis"
        //};


        public void UpLoadOhterData()
        {
            var item = DateTime.Now;


            ////保存大单
            //SaveData<MaxOrders>(item, CommandEnum.RedisKey.MaxOrder);
            ////保存爆仓大单
            //SaveData<Liquidations>(item, CommandEnum.RedisKey.BIGMaxLQ);

            //永续持仓小时
            SaveDatadit<OpenInsterestDatas>(item, CommandEnum.RedisKey.PERP + CommandEnum.RedisKey.OpenInterestDataHour);
            //永续持仓日
            SaveDatadit<OpenInsterestDatas>(item, CommandEnum.RedisKey.PERP + CommandEnum.RedisKey.OpenInterestDataDate);
            //永续持仓小时
            SaveDatadit<OpenInsterestDatas>(item, CommandEnum.RedisKey.DELIVERY + CommandEnum.RedisKey.OpenInterestDataHour);
            //永续持仓日
            SaveDatadit<OpenInsterestDatas>(item, CommandEnum.RedisKey.DELIVERY + CommandEnum.RedisKey.OpenInterestDataDate);
            //爆仓小时汇总
            SaveDatadit<LQSummarystatisticss>(item, CommandEnum.RedisKey.liquidationHour);
            //爆仓日汇总
            SaveDatadit<LQSummarystatisticss>(item, CommandEnum.RedisKey.liquidationDate);

        }
        //上传
        public void UpLoadTradeData()
        {
            var item = DateTime.Now;

            Console.WriteLine("处理日期：" + item.ToString());
            SaveDatadit<TradeMater>(item, "DeliverALLCoinALLExchange");
            SaveDatadit<TradeMater>(item, "DeliverRealDealHour");
            SaveDatadit<TradeMater>(item, "DeliverRealDeal");

            SaveDatadit<TradeMater>(item, "PERPALLCoinALLExchange");
            SaveDatadit<TradeMater>(item, "PERPRealDealHour");
            SaveDatadit<TradeMater>(item, "PERPRealDeal");

            SaveDatadit<TradeMater>(item, "SPOTALLCoinALLExchange");
            SaveDatadit<TradeMater>(item, "SPOTRealDealHour");
            SaveDatadit<TradeMater>(item, "SPOTRealDeal");
        }
        /// <summary>
        /// 上传本地 大单和爆仓大单到服务器
        /// </summary>
        public void UploadList()
        {
        tryagin:
            try
            {
                var serverclient = RedisServerClients.CreateInstance("");
                DateTime item = DateTime.Now;
                List<MaxOrders> locallist = null;
                List<MaxOrders> serverlist = null;
                //保存大单
                SaveData<MaxOrders>(item, CommandEnum.RedisKey.MaxOrder, out locallist, out serverlist);

                if (locallist != null && locallist.Count > 0)
                {
                    if (serverlist.Count > 0)
                    {
                        var maxtime = serverlist.Max(p => p.times);
                        var uploadlist = locallist.Where(p => p.times > maxtime).ToList();
                        if (uploadlist.Count > 0)
                        {
                            foreach (var maxs in uploadlist)
                            {
                                Pushdata(maxs.ToJson(), CommandEnum.RedisKey.MaxOrder, serverclient);
                            }
                        }
                    }
                    else
                    {
                        foreach (var maxs in locallist)
                        {
                            Pushdata(maxs.ToJson(), CommandEnum.RedisKey.MaxOrder, serverclient);
                        }
                    }


                }



                List<Liquidations> Liquidationslocallist = null;
                List<Liquidations> Liquidationsserverlist = null;

                //保存爆仓大单
                SaveData<Liquidations>(item, CommandEnum.RedisKey.BIGMaxLQ, out Liquidationslocallist, out Liquidationsserverlist);

            }
            catch (Exception e)
            {
                Console.WriteLine("上传 爆仓大单/大额订单数据异常,错误信息：" + e.Message.ToString());
                goto tryagin;
            }

        }
        //按天保存
        public bool SaveData<T>(DateTime start, string key, out List<T> locallist, out List<T> serverlist)
        {
            var redisclient = RedisServerClients.CreateInstance("");
            //List<T> maxlist = new List<T>();
            var tablename = key + start.ToString("yyyy-MM-dd");
            CRDataOut geter = new CRDataOut();
            locallist = geter.GetDataObject<T>(tablename);//获取本地

            serverlist = GetSetSScanObjectT<T>(tablename, redisclient);


            if (locallist != null && locallist.Count() > 0 && serverlist != null && serverlist.Count() > 0)
            {

                return true;

            }
            else
            { return false; }


        }

        /// <summary>
        /// 上传hash值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start"></param>
        /// <param name="key"></param>
        public void SaveDatadit<T>(DateTime start, string key)
        {
            var tablename = key + start.ToString("yyyy-MM-dd");
            var maxlists = RedisHelper.GetAllHash<T>(tablename);
            if (maxlists != null && maxlists.Count() > 0)
            {
                foreach (var item in maxlists)
                {
                    //上传到服务器
                    SetOrUpdateHash(key, item.Key, item.Value.ToJson(), RedisServerClients.CreateInstance(""));
                }
            }
        }

        /// <summary>
        /// 更新或插入hash值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filed"></param>
        /// <param name="value"></param>
        public static void SetOrUpdateHash(string key, string filed, string value, RedisClient redisclient)
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add(filed, value);
                var results = redisclient.HSet<string>(key, dic);
            }
            catch (Exception e)
            {
                Console.WriteLine("保存hash数据到服务器redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog("保存hash数据到服务器redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 向list中末端插入列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="dbname"></param>
        /// <param name="key"></param>
        public static void Pushdata(string data, string key, RedisClient redisclient)
        {
            try
            {
                var results = redisclient.SAdd(key, data);
            }
            catch (Exception e)
            {
                redisclient.Dispose();
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到Server redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
        }

        /// <summary>
        /// 获取set集合数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public static List<T> GetSetSScanObjectT<T>(string key, RedisClient redisclient)
        {
            List<T> lists = new List<T>();
            try
            {
                long a = 0;
                while (true)
                {
                    var resultss = redisclient.SScan(key, a, "*", 10000);
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
                Console.WriteLine("从Server获取数据到本地redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "从Server获取数据到本地redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            return lists;

        }

    }
}
