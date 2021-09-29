using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    public class FundingRateAndOpenInterest
    {
        /// <summary>
        /// 获取Fundrate数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<FundRate> GetFundRateData(string key)
        {
            List<FundRate> list = new List<FundRate>();
            var results = RedisHelper.GetSetSScanObjectT<FundRate>(key, "DB0");
            try
            {
                if (results != null && results.Count > 0)
                {
                    list = results;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("获取redis Fundrate数据 出现异常，异常信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "Fundrate数据出现异常，异常信息：" + e.Message.ToString());
            }
            return list;
        }

        /// <summary>
        /// 获取持仓数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<OpenInterest> GetOpenInterests(string key)
        {
            List<OpenInterest> list = new List<OpenInterest>();
            try
            {
                var results = RedisHelper.GetSetSScanObjectT<OpenInterest>(key, "DB0");
                if (results != null && results.Count > 0)
                {
                    list = results;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("获取redis OpenInterest数据 出现异常，异常信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "OpenInterest数据 出现异常，异常信息：" + e.Message.ToString());
            }
            return list;
        }


        /// <summary>
        /// 获取持仓数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<LiquidationModel> GetLQdata(string key)
        {
            List<LiquidationModel> list = new List<LiquidationModel>();
            try
            {
                 list = RedisHelper.GetSetSScanObjectT<LiquidationModel>(key, "DB0");         
            }
            catch (Exception e)
            {
                Console.WriteLine("获取redis OpenInterest数据 出现异常，异常信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "OpenInterest数据 出现异常，异常信息：" + e.Message.ToString());
            }
            return list;
        }


        /// <summary>
        /// 获取当天爆仓数据
        /// </summary>
        /// <returns></returns>
        public List<LiquidationModel> GetLQdata(DateTime dt)
        {
            List<LiquidationModel> alllist = new List<LiquidationModel>();
            var key = CommandEnum.RedisKey.liquidation + dt.ToString("yyyy-MM-dd");
            alllist = GetLQdata(key);
       

            if (alllist.Count > 0)
            {
                var resultlist = alllist.Distinct(new ProductNoComparer());
                return resultlist.ToList();
            }
            else
            {
                return alllist;
            }
        }


        public List<LiquidationModel> GetLQdataFast(DateTime dt)
        {
            var key = CommandEnum.RedisKey.liquidation + dt.ToString("yyyy-MM-dd");
            List<string> st = new List<string>();
            List<LiquidationModel> alllist = new List<LiquidationModel>();
            try
            {
                st = RedisHelper.GetSetSScanObjectstring(key, "DB0");
            }
            catch (Exception e)
            {
                Console.WriteLine("获取redis OpenInterest数据 出现异常，异常信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "OpenInterest数据 出现异常，异常信息：" + e.Message.ToString());
            }


          var listsq=  st.Where(p => p.Contains("[")).ToArray().ToString().ToList<LiquidationModel>();

            foreach (var item in st.Where(p => p.Contains("[")))
            {              
               var res = item.ToList<LiquidationModel>();
                alllist.AddRange(res);              
            }


            foreach (var item in st.Where(p =>!p.Contains("[")))
            {
                var res = item.ToObject<LiquidationModel>();
                alllist.Add(res);
            }



            if (alllist.Count > 0)
            {
                var resultlist = alllist.Distinct(new ProductNoComparer());
                return resultlist.ToList();
            }
            else
            {
                return alllist;
            }
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
            }
            catch (Exception e)
            {

                Console.WriteLine("保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "保存数据到redis失败,失败的key" + key + "失败原因:" + e.Message.ToString());
            }
            return lists;

        }



    }
}
