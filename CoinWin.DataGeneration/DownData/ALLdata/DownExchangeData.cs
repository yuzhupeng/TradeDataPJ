using HtmlAgilityPack;
 
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    public class DownExchangeData
    {
        /// <summary>
        /// 根据时间戳换算为
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }



        /// <summary>
        /// 获取Unix时间戳
        /// </summary>
        /// <param name="time">时间</param>
        public static long GetUnixTimestamp(DateTime time)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long ticks = (time - start.Add(new TimeSpan(8, 0, 0))).Ticks;
            return ToLong(ticks / TimeSpan.TicksPerMillisecond);
        }

        /// <summary>
        /// 从Unix时间戳获取时间
        /// </summary>
        /// <param name="timestamp">Unix时间戳</param>
        public static DateTime GetTimeFromUnixTimestamp(long timestamp)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            TimeSpan span = new TimeSpan(long.Parse(timestamp + "0000000"));
            return start.Add(span).Add(new TimeSpan(8, 0, 0));
        }
        /// <summary>  
        /// 时间戳Timestamp转换成日期  
        /// </summary>  
        /// <param name="timeStamp"></param>  
        /// <returns></returns>  
        public static DateTime GetTimeFromUnixTimestamps(string time)
        {
            return new DateTime((Convert.ToInt64(time) * 10000) + 621355968000000000);
        }






        /// <summary>
        /// 转换为64位整型
        /// </summary>
        /// <param name="input">输入值</param>
        public static long ToLong(object input)
        {
            return ToLongOrNull(input) ?? 0;
        }

        /// <summary>
        /// 转换为64位可空整型
        /// </summary>
        /// <param name="input">输入值</param>
        public static long? ToLongOrNull(object input)
        {
            var success = long.TryParse(input.ToString(), out var result);
            if (success)
                return result;
            try
            {
                var temp = ToDecimalOrNull(input, 0);
                if (temp == null)
                    return null;
                return System.Convert.ToInt64(temp);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 转换为128位可空浮点型,并按指定小数位舍入
        /// </summary>
        /// <param name="input">输入值</param>
        /// <param name="digits">小数位数</param>
        public static decimal? ToDecimalOrNull(object input, int? digits = null)
        {
            var success = decimal.TryParse(input.ToString(), out var result);
            if (!success)
                return null;
            if (digits == null)
                return result;
            return Math.Round(result, digits.Value);
        }


        /// <summary>
        /// 获取开始和结束时间(往后取)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetStartEndTime()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();


            LogHelper.WriteLog(typeof(DownExchangeData), "开始获最后一次获取的数据");


            string endtime = DateTime.UtcNow.ToShortDateString();


            //var sql = string.Format("select * from Coin_ExchageData order by Times limit 1");
            var sql = string.Format("select top 1* from Coin_ExchageData order by Times");


            var starttime = SqlDapperHelper.ExecuteReaderReturnList<ExchageDataModel>(sql).FirstOrDefault();

            if (starttime != null)
            {
                var end = GetUnixTimestamp(Convert.ToDateTime(starttime.utime));

                long start = ToLong(end) - 86400000;




                data.Add(start.ToString(), end.ToString());
            }

            return data;

        }


        /// <summary>
        /// 获取开始和结束时间（往前取）
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetNowStartEndTime()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();


            LogHelper.WriteLog(typeof(DownExchangeData), "开始获最后一次获取的数据");


            string endtime = DateTime.UtcNow.ToShortDateString();


            //var sql = string.Format("select * from Coin_ExchageData order by Times limit 1");
            var sql = string.Format("select top 1* from Coin_ExchageData order by Times desc");


            var starttime = SqlDapperHelper.ExecuteReaderReturnList<ExchageDataModel>(sql).FirstOrDefault();

            if (starttime != null)
            {
                var start = GetUnixTimestamp(Convert.ToDateTime(starttime.utime));
                                          
                long end = ToLong(start) + 86400000;
                data.Add((start + 900000).ToString(), end.ToString());
            }

            return data;

        }



        /// <summary>
        /// 获取开始和结束时间(往后取)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GettypeTime(string  type)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();


            LogHelper.WriteLog(typeof(DownExchangeData), "开始获最后一次获取的数据");


            string endtime = DateTime.UtcNow.ToShortDateString();

            string sql = "";

            if (type == "pass")
            {
                 sql = string.Format("select top 1* from Coin_ExchageDataByMin order by Times");
            }
            else
            {
                sql = string.Format("select top 1* from Coin_ExchageDataByMin order by Times desc ");
            }
          


            var starttime = SqlDapperHelper.ExecuteReaderReturnList<ExchageDataModel>(sql).FirstOrDefault();



            if (starttime != null)
            {
                if (type != "pass")//往前取
                {

                    var start = GetUnixTimestamp(Convert.ToDateTime(starttime.Times));

                    //var start = GetUnixTimestamp(Convert.ToDateTime(starttime.utime));

                    //if (start < 1619388000000)
                    //{
                    //    start = 1619388000000;
                    //}
                    //long end = ToLong(start) + 14200000;

                    //long end = ToLong(start) + 43200000;

                    //data.Add((start + 60000).ToString(), end.ToString());

                    //start = 1630857600000;
                    long end = ToLong(start) + 243200000;

                    data.Add((start).ToString(), end.ToString());


                }
                else//往后取
                {
                    var end = GetUnixTimestamp(Convert.ToDateTime(starttime.utime));
                    //end = end - 60000;
                    long start = ToLong(end) - 21200000;
                    data.Add(start.ToString(), end.ToString());
                }


            }
            else
            {
                string start = "1587330000000";
                long end = ToLong(start) + 14200000;
                data.Add(start.ToString(), end.ToString());
            }


            return data;

        }



        /// <summary>
        /// 获取开始和结束时间(往后取)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GettypeTimenew(string type)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();


            LogHelper.WriteLog(typeof(DownExchangeData), "开始获最后一次获取的数据");


            string endtime = DateTime.UtcNow.ToShortDateString();

            string sql = "";

            if (type == "pass")
            {
                sql = string.Format("select top 1* from Coin_ExchageDataByMin order by Times");
            }
            else
            {
                sql = string.Format("select top 1* from Coin_ExchageDataByMin order by Times desc ");
            }



            var starttime = SqlDapperHelper.ExecuteReaderReturnList<ExchageDataModel>(sql).FirstOrDefault();



            if (starttime != null)
            {
                if (type != "pass")//往前取
                {


                    var start = GetUnixTimestamp(Convert.ToDateTime(starttime.utime));

                    //if (start < 1619388000000)
                    //{
                    //    start = 1619388000000;
                    //}
                    //long end = ToLong(start) + 14200000;

                    long end = ToLong(start) + 243200000;

                    data.Add((start - 600000).ToString(), end.ToString());
                }
                else//往后取
                {
                    var end = GetUnixTimestamp(Convert.ToDateTime(starttime.utime));
                    //end = end - 60000;
                    long start = ToLong(end) - 21200000;
                    data.Add(start.ToString(), end.ToString());
                }


            }
            else
            {
                string start = "1587330000000";
                long end = ToLong(start) + 14200000;
                data.Add(start.ToString(), end.ToString());
            }


            return data;

        }





        /// <summary>
        /// 根据最后一次成功提交数据更新通用数据
        /// </summary>
        public void UpdateExchageData()
        {

            LogHelper.WriteLog(typeof(DownExchangeData), "开始更新通用数据");

            var startend = GetStartEndTime();
            if (startend.Count != 0)
            {
                try
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "开始获取价格数据");
                    string dates = DateTime.Now.ToString();
                    string url = string.Format("https://api.aggr.trade/btcusd/historical/{0}/{1}/{2}", startend.FirstOrDefault().Key, startend.FirstOrDefault().Value, "900000");
                    Console.WriteLine(url);
                    var list = ApiHelper.GetExt(url);

                    var results = ((object)list.results).ToString().ToList<ResultsItem>();
                    Console.WriteLine("done");
                    List<ExchageDataModel> savelist = new List<ExchageDataModel>();



                    if (results != null && results.Count() != 0)
                    {
                        foreach (var item in results)
                        {
                            ExchageDataModel model = new ExchageDataModel();

                            ObjectUtil.MapTo(item, model);
                            model.Times = Convert.ToDateTime(item.time);
                            model.utime = item.time;
                            model.vol = Math.Round(model.vol, 2);
                            model.vol_buy = Math.Round(model.vol_buy, 2);
                            model.vol_sell = Math.Round(model.vol_sell, 2);
                            model.high = Math.Round(model.high, 2);
                            model.low = Math.Round(model.low, 2);
                            model.open = Math.Round(model.open, 2);

                            model.close = Math.Round(model.close, 2);
                            model.liquidation_buy = Math.Round(model.liquidation_buy, 2);
                            model.liquidation_sell = Math.Round(model.liquidation_sell, 2);

                            model.liquidation = model.liquidation_buy + model.liquidation_sell;

                            savelist.Add(model);
                        }
                    }
                    LogHelper.WriteLog(typeof(DownExchangeData), "结束获取价格数据");



                    SaveList(savelist);




                }
                catch (Exception e)
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "获取价格数据表异常,错误信息:" + e.ToString());

                }
            }
            else
            {

                try

                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "开始获取价格数据");
                    DateTime starttime = new DateTime(2020, 12, 23, 12, 15, 00);


                    //string dateend = (1608728844882).ToString();
                    //string datestart = (1608249600000).ToString();
                    string dateend = (1608771600000).ToString();
                    string datestart = (1608729300000).ToString();

                    string timeframe = "900000";
                    string url = string.Format("https://api.aggr.trade/btcusd/historical/{0}/{1}/{2}", datestart, dateend, timeframe);


                    //HtmlWeb web = new HtmlWeb();
                    //var htmlDoc = web.Load(url);


                    var list = ApiHelper.GetExt(url);


                    var results = ((object)list.results).ToString().ToList<ResultsItem>();

                    List<ExchageDataModel> savelist = new List<ExchageDataModel>();



                    if (results != null && results.Count() != 0)
                    {
                        foreach (var item in results)
                        {
                            ExchageDataModel model = new ExchageDataModel();

                            ObjectUtil.MapTo(item, model);

                            model.Times = Convert.ToDateTime(item.time);
                            model.utime = item.time;
                            model.vol = Math.Round(model.vol, 2);
                            model.vol_buy = Math.Round(model.vol_buy, 2);
                            model.vol_sell = Math.Round(model.vol_sell, 2);

                            model.liquidation_buy = Math.Round(model.liquidation_buy, 2);
                            model.liquidation_sell = Math.Round(model.liquidation_sell, 2);

                            model.liquidation = model.liquidation_buy + model.liquidation_sell;

                            model.high = Math.Round(model.high, 2);
                            model.low = Math.Round(model.low, 2);
                            model.open = Math.Round(model.open, 2);

                            model.close = Math.Round(model.close, 2);
                            savelist.Add(model);
                        }
                    }

                    SaveList(savelist);
                    LogHelper.WriteLog(typeof(DownExchangeData), "结束获取价格数据");
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "获取价格数据表异常,错误信息:" + e.ToString());
                }
            }
            LogHelper.WriteLog(typeof(DownExchangeData), "结束更新通用数据");

        }

        /// <summary>
        /// 获取最新的数据的时间
        /// </summary>
        public void UpdateExchangeDataNow()
        {
            LogHelper.WriteLog(typeof(DownExchangeData), "开始更新通用数据");

            var startend = GetNowStartEndTime();
            if (startend.Count != 0)
            {
                try
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "开始获取价格数据");
                    string dates = DateTime.Now.ToString();
                    string url = string.Format("https://api.aggr.trade/btcusd/historical/{0}/{1}/{2}", startend.FirstOrDefault().Key, startend.FirstOrDefault().Value, "900000");
                    Console.WriteLine(url);
                    var list = ApiHelper.GetExt(url);

                    var results = ((object)list.results).ToString().ToList<ResultsItem>();
                    Console.WriteLine("done");
                    List<ExchageDataModel> savelist = new List<ExchageDataModel>();



                    if (results != null && results.Count() != 0)
                    {
                        foreach (var item in results)
                        {
                            ExchageDataModel model = new ExchageDataModel();

                            ObjectUtil.MapTo(item, model);
                            model.Times = Convert.ToDateTime(item.time);
                            model.utime = item.time;
                            model.vol = Math.Round(model.vol, 2);
                            model.vol_buy = Math.Round(model.vol_buy, 2);
                            model.vol_sell = Math.Round(model.vol_sell, 2);
                            model.high = Math.Round(model.high, 2);
                            model.low = Math.Round(model.low, 2);
                            model.open = Math.Round(model.open, 2);

                            model.close = Math.Round(model.close, 2);
                            model.liquidation_buy = Math.Round(model.liquidation_buy, 2);
                            model.liquidation_sell = Math.Round(model.liquidation_sell, 2);

                            model.liquidation = model.liquidation_buy + model.liquidation_sell;

                            savelist.Add(model);
                        }
                    }
                    LogHelper.WriteLog(typeof(DownExchangeData), "结束获取价格数据");



                    SaveList(savelist);




                }
                catch (Exception e)
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "获取价格数据表异常,错误信息:" + e.ToString());

                }
            }

        }


        /// <summary>
        /// 根据最后一次成功提交数据更新通用数据
        /// </summary>
        public void UpdateExchageDataBymin(string type)
        {

            LogHelper.WriteLog(typeof(DownExchangeData), "开始更新通用数据");

            var startend = GettypeTime(type);
            if (startend.Count != 0)
            {
                try
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "开始获取价格数据");
                    string dates = DateTime.Now.ToString();
                    //string url = string.Format("https://api.aggr.trade/btcusd/historical/{0}/{1}/{2}", startend.FirstOrDefault().Key, startend.FirstOrDefault().Value, "60000");
                    string url = string.Format("https://api.aggr.trade/historical/{0}/{1}/{2}/{3}", startend.FirstOrDefault().Key, startend.FirstOrDefault().Value, "60000", "BITFINEX:BTCUSD+BINANCE:btcusdt+OKEX:BTC-USDT+KRAKEN:XBT/USD+COINBASE:BTC-USD+POLONIEX:USDT_BTC+HUOBI:btcusdt+BITSTAMP:btcusd+BITMEX:XBTUSD+BITFINEX:BTCF0:USTF0+OKEX:BTC-USD-SWAP+OKEX:BTC-USDT-SWAP+BINANCE_FUTURES:btcusdt+BINANCE_FUTURES:btcusd_perp+HUOBI:BTC-USD+KRAKEN:PI_XBTUSD+DERIBIT:BTC-PERPETUAL+FTX:BTC-PERP+BYBIT:BTCUSD");
                    string start =  (startend.FirstOrDefault().Key);
                    string end =  (startend.FirstOrDefault().Value); //long.Parse


                    LogHelper.WriteLog(typeof(DownExchangeData), url);

                    Console.WriteLine("开始时间："+DownExchangeData.GetTimeFromUnixTimestamps(start));
                    Console.WriteLine("结束时间：" + DownExchangeData.GetTimeFromUnixTimestamps(end));



                    Console.WriteLine(url);
                    ////var list = ApiHelper.GetExtbinance(url);

                    ////string RESULTSQ = ApiHelper.fileToString("D:\\code\\test.txt");
                    ////var results = RESULTSQ.ToList<ResultsItem>();
                    //HttpClientDataDown dd = new HttpClientDataDown();

                    //var list = dd.downdata(url);
                    ////var results = ((object)list.results).ToString().ToList<ResultsItem>();
                    //var resultss = list.ToString().ToList<ResultsItemArry>();

                    //var results = ((object)list.results).ToString().ToList<ResultsItemArry>();

                    HttpClientDataDown dd = new HttpClientDataDown();
                    var results = dd.downdatas(url);



                    Console.WriteLine("done");
                    List<ExchageDataMin> savelist = new List<ExchageDataMin>();



                    if (results != null && results.Count() != 0)
                    {
                        foreach (var item in results)
                        {
                            ExchageDataMin model = new ExchageDataMin();

                            ObjectUtil.MapTo(item, model);
                            //model.Times = Convert.ToDateTime(item.time);
                            model.Times = ReadTime(item.time);
                            model.utime = item.time;
                      
                            if (model.vol_sell != null)
                            {
                                model.vol_sell = Math.Round(model.vol_sell.Value, 2);
                            }
                            else
                            {
                                model.vol_sell = 0;
                            }
                            if (model.vol_buy != null)
                            {
                                model.vol_buy = Math.Round(model.vol_buy.Value, 2);
                            }
                            else
                            {
                                model.vol_buy = 0;
                            }

                            model.count = model.count_buy + model.count_sell;                       
                            model.vol = model.vol_buy + model.vol_sell;


                            string[] arr3 = model.exchange.Split(':');

                            var exchange = arr3[0];//OKEX
                            var pair = arr3[1];//PAIR

                            
                            model.exchange = exchange;
                            model.pair = pair;
                            model.Unit = item.exchange;
                            //model.vol_buy = Math.Round(model.vol_buy, 2);
                            //model.vol_sell = Math.Round(model.vol_sell, 2);
                            model.high = Math.Round(model.high, 2);
                            model.low = Math.Round(model.low, 2);
                            model.open = Math.Round(model.open, 2);

                            model.close = Math.Round(model.close, 2);
                            model.liquidation_buy = Math.Round(model.liquidation_buy, 2);
                            model.liquidation_sell = Math.Round(model.liquidation_sell, 2);

                            model.liquidation = model.liquidation_buy + model.liquidation_sell;

                            savelist.Add(model);
                        }
                    }
                    LogHelper.WriteLog(typeof(DownExchangeData), "结束获取价格数据");



                    SaveListone(savelist);




                }
                catch (Exception e)
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "获取价格数据表异常,错误信息:" + e.ToString());

                }
            }
            else
            {

                try

                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "开始获取价格数据");
                    //DateTime starttime = new DateTime(2020, 12, 23, 12, 15, 00);


                    //string dateend = (1608728844882).ToString();
                    //string datestart = (1608249600000).ToString();
                    string dateend = (1609828200000).ToString();  
                    string datestart = (1609826400000).ToString();

                    string timeframe = "60000";
                    string url = string.Format("https://api.aggr.trade/btcusd/historical/{0}/{1}/{2}", datestart, dateend, timeframe);


                    //HtmlWeb web = new HtmlWeb();
                    //var htmlDoc = web.Load(url);


                    var list = ApiHelper.GetExt(url);


                    var results = ((object)list.results).ToString().ToList<ResultsItem>();

                    List<ExchageDataMin> savelist = new List<ExchageDataMin>();



                    if (results != null && results.Count() != 0)
                    {
                        foreach (var item in results)
                        {
                            ExchageDataMin model = new ExchageDataMin();

                            ObjectUtil.MapTo(item, model);

                            model.Times = Convert.ToDateTime(item.time);
                            model.utime = item.time;
                            if (model.vol != null)
                            {
                                model.vol = Math.Round(model.vol.Value, 2);
                            }
                            else
                            {
                                model.vol = 0;
                            }
                            if (model.vol_sell != null)
                            {
                                model.vol_sell = Math.Round(model.vol_sell.Value, 2);
                            }
                            else
                            {
                                model.vol_sell = 0;
                            }
                            if (model.vol_buy != null)
                            {
                                model.vol_buy = Math.Round(model.vol_buy.Value, 2);
                            }
                            else
                            {
                                model.vol_buy = 0;
                            }
                            //model.vol_buy = Math.Round(model.vol_buy, 2);
                            //model.vol_sell = Math.Round(model.vol_sell, 2);

                            model.liquidation_buy = Math.Round(model.liquidation_buy, 2);
                            model.liquidation_sell = Math.Round(model.liquidation_sell, 2);

                            model.liquidation = model.liquidation_buy + model.liquidation_sell;

                            model.high = Math.Round(model.high, 2);
                            model.low = Math.Round(model.low, 2);
                            model.open = Math.Round(model.open, 2);

                            model.close = Math.Round(model.close, 2);
                            savelist.Add(model);
                        }
                    }
                    SaveListmin(savelist);
                    LogHelper.WriteLog(typeof(DownExchangeData), "结束获取价格数据");
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "获取价格数据表异常,错误信息:" + e.ToString());
                }
            }
            LogHelper.WriteLog(typeof(DownExchangeData), "结束更新通用数据");

        }






        public  DateTime ReadTime(string times)
        {

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(times + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);

            //long num = long.Parse(reader.Value!.ToString());
            //var times =    new DateTime((num * 10000) + 621355968000000000);
            //DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //long lTime = long.Parse(times.ToString() + "0000");
            //TimeSpan toNow = new TimeSpan(lTime);
            //var results = dtStart.Add(toNow);
         
        }

        public void SaveListmin(List<ExchageDataMin> savelist)
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

        public void SaveListone(List<ExchageDataMin> savelist)
        {
            using (SqlConnection con = SqlDapperHelper.GetOpenConnection())
            {
               
                    foreach (var item in savelist)
                {
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            SqlDapperHelper.ExecuteInsert<ExchageDataMin>(item, transaction);
                            transaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("重复数据：" + item.ToJson().ToString());
                            LogHelper.WriteLog(typeof(object), "保存价格数据发生错误，错误信息:" + ex.Message.ToString());
                            transaction.Rollback();
                            //throw ex;
                        }

                    }
                }
            }
        }




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

        public void SaveList(List<ExchageDataModel> savelist)
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
        /// 根据最后一次成功提交数据更新通用数据
        /// </summary>
        public void UpdateExchageDataBydate(string type)
        {

            LogHelper.WriteLog(typeof(DownExchangeData), "开始更新通用数据");

            var startend = GettypeTimenew(type);
            if (startend.Count != 0)
            {
                try
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "开始获取价格数据");
                    string dates = DateTime.Now.ToString();
               
                    //string url = string.Format("https://api.aggr.trade/historical/{0}/{1}/{2}/{3}", "1579467600000", "1625482667000", "86400000", "BITFINEX:BTCUSD+BINANCE:btcusdt+OKEX:BTC-USDT+KRAKEN:XBT/USD+COINBASE:BTC-USD+POLONIEX:USDT_BTC+HUOBI:btcusdt+BITSTAMP:btcusd+BITMEX:XBTUSD+BITFINEX:BTCF0:USTF0+OKEX:BTC-USD-SWAP+OKEX:BTC-USDT-SWAP+BINANCE_FUTURES:btcusdt+BINANCE_FUTURES:btcusd_perp+HUOBI:BTC-USD+KRAKEN:PI_XBTUSD+DERIBIT:BTC-PERPETUAL+FTX:BTC-PERP+BYBIT:BTCUSD");

                    string url = string.Format("https://api.aggr.trade/historical/{0}/{1}/{2}/{3}", startend.FirstOrDefault().Key, startend.FirstOrDefault().Value, "86400000", "BITFINEX:BTCUSD+BINANCE:btcusdt+OKEX:BTC-USDT+KRAKEN:XBT/USD+COINBASE:BTC-USD+POLONIEX:USDT_BTC+HUOBI:btcusdt+BITSTAMP:btcusd+BITMEX:XBTUSD+BITFINEX:BTCF0:USTF0+OKEX:BTC-USD-SWAP+OKEX:BTC-USDT-SWAP+BINANCE_FUTURES:btcusdt+BINANCE_FUTURES:btcusd_perp+HUOBI:BTC-USD+KRAKEN:PI_XBTUSD+DERIBIT:BTC-PERPETUAL+FTX:BTC-PERP+BYBIT:BTCUSD");



                    string start = (startend.FirstOrDefault().Key);
                    string end = (startend.FirstOrDefault().Value); //long.Parse


                    LogHelper.WriteLog(typeof(DownExchangeData), url);

                    Console.WriteLine("开始时间：" + DownExchangeData.GetTimeFromUnixTimestamps(end));
                    Console.WriteLine("结束时间：" + DownExchangeData.GetTimeFromUnixTimestamps(start));



                    Console.WriteLine(url);
                    var list = ApiHelper.GetExtbinance(url);

                    var results = ((object)list.results).ToString().ToList<ResultsItem>();
                    Console.WriteLine("done");
                    List<ExchageDataMin> savelist = new List<ExchageDataMin>();



                    if (results != null && results.Count() != 0)
                    {
                        foreach (var item in results)
                        {
                            ExchageDataMin model = new ExchageDataMin();

                            ObjectUtil.MapTo(item, model);
                            model.Times = Convert.ToDateTime(item.time);
                            model.utime = item.time;

                            if (model.vol_sell != null)
                            {
                                model.vol_sell = Math.Round(model.vol_sell.Value, 2);
                            }
                            else
                            {
                                model.vol_sell = 0;
                            }
                            if (model.vol_buy != null)
                            {
                                model.vol_buy = Math.Round(model.vol_buy.Value, 2);
                            }
                            else
                            {
                                model.vol_buy = 0;
                            }

                            model.count = model.count_buy + model.count_sell;
                            model.vol = model.vol_buy + model.vol_sell;
                            string[] arr3 = "1:2".Split(':'); 
                            try
                            {
                                 arr3 = model.exchange.Split(':');
                            } catch (Exception e)
                            { 
                            
                            }
                            var exchange = arr3[0];//OKEX
                            var pair = arr3[1];//PAIR


                            model.exchange = exchange;
                            model.pair = pair;
                            model.Unit = item.exchange;
                            //model.vol_buy = Math.Round(model.vol_buy, 2);
                            //model.vol_sell = Math.Round(model.vol_sell, 2);
                            model.high = Math.Round(model.high, 2);
                            model.low = Math.Round(model.low, 2);
                            model.open = Math.Round(model.open, 2);

                            model.close = Math.Round(model.close, 2);
                            model.liquidation_buy = Math.Round(model.liquidation_buy, 2);
                            model.liquidation_sell = Math.Round(model.liquidation_sell, 2);

                            model.liquidation = model.liquidation_buy + model.liquidation_sell;

                            savelist.Add(model);
                        }
                    }
                    LogHelper.WriteLog(typeof(DownExchangeData), "结束获取价格数据");



                    SaveListone(savelist);




                }
                catch (Exception e)
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "获取价格数据表异常,错误信息:" + e.ToString());

                }
            }
            else
            {

                try

                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "开始获取价格数据");
                    //DateTime starttime = new DateTime(2020, 12, 23, 12, 15, 00);


                    //string dateend = (1608728844882).ToString();
                    //string datestart = (1608249600000).ToString();
                    string dateend = (1609828200000).ToString();
                    string datestart = (1609826400000).ToString();

                    string timeframe = "60000";
                    string url = string.Format("https://api.aggr.trade/btcusd/historical/{0}/{1}/{2}", datestart, dateend, timeframe);


                    //HtmlWeb web = new HtmlWeb();
                    //var htmlDoc = web.Load(url);


                    var list = ApiHelper.GetExt(url);


                    var results = ((object)list.results).ToString().ToList<ResultsItem>();

                    List<ExchageDataMin> savelist = new List<ExchageDataMin>();



                    if (results != null && results.Count() != 0)
                    {
                        foreach (var item in results)
                        {
                            ExchageDataMin model = new ExchageDataMin();

                            ObjectUtil.MapTo(item, model);

                            model.Times = Convert.ToDateTime(item.time);
                            model.utime = item.time;
                            if (model.vol != null)
                            {
                                model.vol = Math.Round(model.vol.Value, 2);
                            }
                            else
                            {
                                model.vol = 0;
                            }
                            if (model.vol_sell != null)
                            {
                                model.vol_sell = Math.Round(model.vol_sell.Value, 2);
                            }
                            else
                            {
                                model.vol_sell = 0;
                            }
                            if (model.vol_buy != null)
                            {
                                model.vol_buy = Math.Round(model.vol_buy.Value, 2);
                            }
                            else
                            {
                                model.vol_buy = 0;
                            }
                            //model.vol_buy = Math.Round(model.vol_buy, 2);
                            //model.vol_sell = Math.Round(model.vol_sell, 2);

                            model.liquidation_buy = Math.Round(model.liquidation_buy, 2);
                            model.liquidation_sell = Math.Round(model.liquidation_sell, 2);

                            model.liquidation = model.liquidation_buy + model.liquidation_sell;

                            model.high = Math.Round(model.high, 2);
                            model.low = Math.Round(model.low, 2);
                            model.open = Math.Round(model.open, 2);

                            model.close = Math.Round(model.close, 2);
                            savelist.Add(model);
                        }
                    }
                    SaveListmin(savelist);
                    LogHelper.WriteLog(typeof(DownExchangeData), "结束获取价格数据");
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "获取价格数据表异常,错误信息:" + e.ToString());
                }
            }
            LogHelper.WriteLog(typeof(DownExchangeData), "结束更新通用数据");

        }



        /// <summary>
        /// 未使用
        /// </summary>
        /// <param name="savelist"></param>
        public void SaveLists(List<ExchageDataModel> savelist)
        {
            string sql = @"INSERT INTO Coin_ExchageData (ID,DID,Times,utime,BTCclose,allcount,count_buy,count_sell,exchange,high,liquidation_buy,liquidation_sell,liquidation,low,Bopen,pair,vol,vol_buy,vol_sell,Unit,SYS_Createby,SYS_CreateDate,SYS_Status,)
             VALUES(@ID,@DID,@Times,@utime,@BTCclose,@allcount,@count_buy,@count_sell,@exchange,@high,@liquidation_buy,@liquidation_sell,@low,@Bopen,@pair,@vol_buy,@vol_sell,@Unit,@SYS_Createby,@SYS_CreateDate,@SYS_Status); ";
            using (SqlConnection con = SqlDapperHelper.GetOpenConnection())
            {
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {

                        SqlDapperHelper.Add(savelist, sql, transaction);
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

    }
}
