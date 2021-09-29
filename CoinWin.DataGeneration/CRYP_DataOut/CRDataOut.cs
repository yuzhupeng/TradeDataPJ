using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
   public class CRDataOut
    {
        public List<UPermanentFuturesModel> GetData(string key)
        {
            List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();
            var results = RedisHelper.GetSetSScan(key, "DB0");

            try
            {
                if (results != null && results.Count > 0)
                {
                    foreach (var item in results)
                    {
                        UPermanentFuturesModel one = new UPermanentFuturesModel();                      
                        ObjectUtil.MapTo(item, one);                     
                        list.Add(one);
                    }                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("转化类出现异常，异常信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "转化类出现异常，异常信息："+e.Message.ToString());
            }


            return list;
        }


        /// <summary>
        /// 获取大单数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<MaxOrder> GetDataObject(string key)
        {
            List<MaxOrder> list = new List<MaxOrder>();
        

            try
            {
                var results = RedisHelper.GetSetSScanObjectT<MaxOrder>(key, "DB0");
                //if (results != null && results.Count > 0)
                //{
                //    foreach (var item in results)
                //    {
                //        UPermanentFuturesModel one = new UPermanentFuturesModel();
                //        ObjectUtil.MapTo(item, one);
                //        list.Add(one);
                //    }
                //}
                list = results;
            }
            catch (Exception e)
            {
                Console.WriteLine("大单转化类出现异常，异常信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "大单转化类出现异常，异常信息：" + e.Message.ToString());
            }


            return list;
        }


        /// <summary>
        /// 获取大单数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetDataObject<T>(string key)
        {
            List<T> list = new List<T>();
            try
            {
                var results = RedisHelper.GetSetSScanObjectT<T>(key, "DB0");     
                list = results;
            }
            catch (Exception e)
            {
                Console.WriteLine("获取数据出现异常，异常信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "获取数据出现异常，异常信息：" + e.Message.ToString());
            }
            return list;
        }








        /// <summary>
        /// 获取LQdata
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Liquidation> GetLQData(string key)
        {
            List<Liquidation> list = new List<Liquidation>();
            var results = RedisHelper.GetSetSScanObject(key, "DB0");

            try
            {
                if (results != null && results.Count > 0)
                {
                    foreach (var item in results)
                    {
                        Liquidation one = new Liquidation();
                        ObjectUtil.MapTo(item, one);
                        list.Add(one);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("LQ转化类出现异常，异常信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "LQ转化类出现异常，异常信息：" + e.Message.ToString());
            }


            return list;
        }




        #region 处理交割合约数据统计


        /// <summary>
        /// 交割  bz  jys
        /// </summary>
        public void DownDeliveryFuturesData(List<UPermanentFuturesModel> data,DateTime st,DateTime en)
        {
                       
            //var Minlist = TimeCore.GetAllDateMin(new DateTime(2021, 01, 29));


            //List<UPermanentFuturesModel> data = new List<UPermanentFuturesModel>();
            //data = adata.ToList<UPermanentFuturesModel>();
            //data= GetData().OrderByDescending(a=>a.timestamp).ToList();


            var exchagelist = CommandEnum.ExchangeData.ExchangeList;
            var symbolList= CommandEnum.ExchangeData.symbolList;

            if (data.Count == 0)
            {
                return;
            }


            var strat = Convert.ToDateTime(data.Min(p => Convert.ToDateTime(p.times)));

            var end = Convert.ToDateTime(data.Max(p => Convert.ToDateTime(p.times)));


            var Minlist = TimeCore.GetStartAndEndTime(strat, end);


            foreach (var item in exchagelist)
            {
                
                var exchage= data.Where(p => p.exchange == item).ToList();
                if (exchage != null && exchage.Count() > 0)
                    ProcessOneExchage(exchage, symbolList, Minlist, item);

           
            }
        }

        /// <summary>
        /// 处理交割合约数据统计
        /// </summary>
        /// <param name="data"></param>
        /// <param name="symbollist"></param>
        public void ProcessOneExchage(List<UPermanentFuturesModel> data, List<string> symbollist,List<DateTime>dtlist,string exchange)
        {
            List<DeliveryFutures> savelist = new List<DeliveryFutures>();

            List<string> grouplist = new List<string>();
         

            if (data != null && data.Count > 0)
            {
                foreach (var dt in dtlist)//每一分钟
                {
                    //SaveListDelivery(savelist);
                    var resultslist = data.Where(p => Convert.ToDateTime(p.SYS_CreateDate).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")).ToList();

                    grouplist = resultslist.GroupBy(p => p.market).Select(a => a.Key).ToList();

                    if (resultslist != null && resultslist.Count > 0)
                    {
                        foreach (var sy in grouplist)//bizhong
                        {
                            var Modellist = resultslist.Where(p => p.market.Contains(sy)).OrderBy(p=>p.SYS_CreateDate).ToList();
                            if (Modellist.Count() > 0)
                            {
                                DeliveryFutures df = new DeliveryFutures();
                                df.price = Modellist.Average(p => p.price);
                                df.open = Modellist.FirstOrDefault().price;
                                df.close = Modellist.Last().price;
                                df.low = Modellist.Min(p => p.price);
                                df.high = Modellist.Max(p => p.price);
                           
                                //var buyslist = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY);

                                df.vol = Modellist.Sum(p => p.vol);
                                df.vol_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Sum(a => a.vol);
                                df.vol_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Sum(a => a.vol);



                                df.qty = Modellist.Sum(p => p.qty);
                                df.qty_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Sum(a => a.qty);
                                df.qty_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Sum(a => a.qty);


                                df.Unit = Modellist.FirstOrDefault().Unit == null ? "" : Modellist.FirstOrDefault().Unit;

                                df.pair = Modellist.FirstOrDefault().pair;
                                df.count = Modellist.Count();
                                df.count_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Count();
                                df.count_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Count();
                                df.exchange = exchange;
                                df.liquidation = 0;
                                df.liquidation_buy = 0;
                                df.liquidation_sell = 0;
                                df.basis = "0";
                                df.market = Modellist.FirstOrDefault().market;
                                df.timestamp = Modellist.FirstOrDefault().timestamp;
                                df.types = Modellist.FirstOrDefault().types;
                                df.utime = Modellist.FirstOrDefault().utctime;
                                df.Times = dt;


                                savelist.Add(df);
                            }
                            //data.RemoveAll(Modellist);
                            //if (Modellist.Count > 0)
                            //{
                            //    //for (int i = 0; i < Modellist.Count - 1;)
                            //    //{

                            //    //    data.Remove(Modellist[i]);


                            //    //    i++;

                            //    //}
                            //}

                        }
                    }
                }
            }

            //var listsss = savelist;
            if (savelist.Count() > 0)
            {
                SaveListDelivery(savelist);
            }
            //foreach (var item in symbollist)
            //{
            //  var lists=data.Where(p => p.exchange == item).ToList();
            //    if (lists != null && lists.Count > 0)
            //    {
            //        foreach (var dt in dtlist)
            //        {
            //         var resultslist=lists.Where(p => Convert.ToDateTime(p.times).ToString("yyyy-MM-dd HH:mm") == dt.ToString("yyyy-MM-dd HH:mm")).ToList();

            //            if (resultslist != null && resultslist.Count > 0)
            //            { 



            //            }

            //            }



            //    }



            //}

        }





        /// <summary>
        /// 处理交割合约
        /// </summary>
        /// <param name="data"></param>
        /// <param name="symbollist"></param>
   
        public void SaveListDelivery(List<DeliveryFutures> savelist)
        {
            using (SqlConnection con = SqlDapperHelper.GetOpenConnection())
            {
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        SqlDapperHelper.ExecuteInsertList(savelist, transaction);
                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(typeof(object), "保存价格数据发生错误，错误信息:" + ex.Message.ToString());
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }


        #endregion







        #region 处理永续合约数据



        /// <summary>
        /// 永续  bz  jys
        /// </summary>
        public void DownPERPFuturesData(List<UPermanentFuturesModel> data, DateTime st, DateTime en)
        {
            var exchagelist = CommandEnum.ExchangeData.ExchangeList;
            var symbolList = CommandEnum.ExchangeData.symbolList;

            if (data.Count == 0)
            {
                return;
            }

            //var strat = Convert.ToDateTime(data.First().SYS_CreateDate);

            //var strat = Convert.ToDateTime(data.FirstOrDefault().times);

            //var end = Convert.ToDateTime(data.Last().SYS_CreateDate);

            //var end = new DateTime(2021, 01, 29, 23, 59, 00);


            var strat = Convert.ToDateTime(data.Min(p=>Convert.ToDateTime(p.times)));

            var end = Convert.ToDateTime(data.Max(p => Convert.ToDateTime(p.times)));


            var Minlist = TimeCore.GetStartAndEndTime(strat, end);

            foreach (var item in exchagelist)
            {
                var exchage = data.Where(p => p.exchange == item).ToList();
                if(exchage!=null&&exchage.Count()>0)
                ProcessOneExchagePerp(exchage, symbolList, Minlist, item);

            }

        }


        /// <summary>
        /// 处理永续合约
        /// </summary>
        /// <param name="data"></param>
        /// <param name="symbollist"></param>
        /// <param name="dtlist"></param>
        /// <param name="exchange"></param>
        public void ProcessOneExchagePerp(List<UPermanentFuturesModel> data, List<string> symbollist, List<DateTime> dtlist, string exchange)
        {
            List<PermanentFutures> savelist = new List<PermanentFutures>();

            List<string> grouplist = new List<string>();


            if (data != null && data.Count > 0)
            {
                foreach (var dt in dtlist)//每一分钟
                {
                    //SaveListDelivery(savelist);
                    var resultslist = data.Where(p => Convert.ToDateTime(p.SYS_CreateDate).ToString("yyyy-MM-dd HH") == dt.ToString("yyyy-MM-dd HH")).ToList();

                    grouplist = resultslist.GroupBy(p => p.pair).Select(a => a.Key).ToList();

                    if (resultslist != null && resultslist.Count > 0)
                    {
                        foreach (var sy in grouplist)//bizhong
                        {
                            var Modellist = resultslist.Where(p => p.pair==sy).OrderBy(p => p.SYS_CreateDate).ToList();
                            if (Modellist.Count() > 0)
                            {
                                PermanentFutures df = new PermanentFutures();
                                df.price = Modellist.Average(p => p.price);
                                df.open = Modellist.FirstOrDefault().price;
                                df.close = Modellist.Last().price;
                                df.low = Modellist.Min(p => p.price);
                                df.high = Modellist.Max(p => p.price);
                              
                                var buyslist = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY);


                                df.vol = Modellist.Sum(p => p.vol);
                                df.vol_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Sum(a => a.vol);
                                df.vol_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Sum(a => a.vol);

                                df.qty = Modellist.Sum(p => p.qty);
                                df.qty_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Sum(a => a.qty);
                                df.qty_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Sum(a => a.qty);


                                df.Unit = Modellist.FirstOrDefault().Unit==null? "" : Modellist.FirstOrDefault().Unit;
                                df.pair = Modellist.FirstOrDefault().pair;
                                df.count = Modellist.Count();
                                df.count_buy = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.BUY).Count();
                                df.count_sell = Modellist.Where(p => p.side.ToUpper() == CommandEnum.RedisKey.SELL).Count();
                                df.exchange = exchange;
                                df.liquidation = 0;
                                df.liquidation_buy = 0;
                                df.liquidation_sell = 0;
                                df.basis = "0";
                                df.market = Modellist.FirstOrDefault().market;
                                df.timestamp = Modellist.FirstOrDefault().timestamp;
                                df.types = Modellist.FirstOrDefault().types;
                                df.utime = Modellist.FirstOrDefault().utctime;
                                df.Times = dt;


                                savelist.Add(df);
                            }                
                        }
                    }
                }
            }

       
            if (savelist.Count() > 0)
            {
                SavePermanentFuturesData(savelist);
            }
            

        }

        /// <summary>
        /// 永续
        /// </summary>
        public void SavePermanentFuturesData(List<PermanentFutures> savelist)
        {
            using (SqlConnection con = SqlDapperHelper.GetOpenConnection())
            {
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        SqlDapperHelper.ExecuteInsertList(savelist, transaction);
                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(typeof(object), "保存价格数据发生错误，错误信息:" + ex.Message.ToString());
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 获取最后更新时间
        /// </summary>
        /// <param name="type">
        /// 交割 JG
        /// /永续 YX
        /// /现货 XH
        /// </param>
        /// <returns></returns>
        public DateTime GetLastTime(string type)
        {
            tryagain:
            DateTime result = new DateTime();
            try
            {

                string sql = "";

                if (type == "JG")
                {
                    sql = string.Format("select top 1* from [DeliveryFutures] order by Times desc");
                }
                else if (type == "YX")
                {
                    sql = string.Format("select top 1* from [PermanentFutures] order by Times desc");
                }
                else
                {
                    sql = string.Format("select top 1 * from[DeliveryFutures] order by Times desc");
                }



                result = SqlDapperHelper.ExecuteReaderReturnList<DeliveryFutures>(sql).FirstOrDefault().Times;
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(typeof(DownExchangeData), "获取最后更新时间出错："+ e.Message.ToString());
                goto tryagain;
            }
                
            return result;
        }



        #endregion
    }
}
