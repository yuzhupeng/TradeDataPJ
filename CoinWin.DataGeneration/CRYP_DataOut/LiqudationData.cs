using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    public class LiqudationData
    {

        public List<LiquidationModelMail> SaveLQDataMax(string key)
        {
            //DateTime dt = GetLQLastTime();
            List<LiquidationModelMail> list = new List<LiquidationModelMail>();
            CRDataOut geter = new CRDataOut();
            var results = geter.GetLQData(key);

            if (results.Count() == 0)
            {
                return null;
            }
            //var targetlist = results.Where(p => Convert.ToDateTime(p.times) > dt) ;
            var targetlist = results;
            foreach (var item in targetlist)
            {

                LiquidationModelMail one = new LiquidationModelMail();
                if (item.exchange.ToUpper() != "BYBIT")
                {
                    one.description = item.side.ToUpper() == "SELL" ? "多单爆仓" : "空单爆仓";
                }
                else
                {
                    one.description = item.side.ToUpper() == "SELL" ? "空单爆仓" : "多单爆仓";
                }
                ObjectUtil.MapTo(item, one);
                list.Add(one);
            }

            return list;

        }


        public List<LiquidationModel> SaveLQData(string key)
        {
            //DateTime dt = GetLQLastTime();
            List<LiquidationModel> list = new List<LiquidationModel>();
            CRDataOut geter = new CRDataOut();
            var results = geter.GetLQData(key);

            if (results.Count() == 0)
            {
                return null;
            }
            //var targetlist = results.Where(p => Convert.ToDateTime(p.times) > dt) ;
            var targetlist = results;
            foreach (var item in targetlist)
            {

                LiquidationModel one = new LiquidationModel();
                if (item.exchange.ToUpper() != "BYBIT")
                {
                    one.description = item.side.ToUpper() == "SELL" ? "多单爆仓" : "空单爆仓";
                }
                else
                {
                    one.description = item.side.ToUpper() == "SELL" ? "空单爆仓" : "多单爆仓";
                }
                ObjectUtil.MapTo(item, one);
                list.Add(one);
            }

            return list;

        }


        public List<LiquidationModel> SaveLQDataNEW(string key, DateTime dt)
        {
            //DateTime dt = GetLQLastTime();
            List<LiquidationModel> list = new List<LiquidationModel>();
            CRDataOut geter = new CRDataOut();
            var results = geter.GetLQData(key);

            if (results.Count() == 0)
            {
                return null;
            }
            var targetlist = results.Where(p => Convert.ToDateTime(p.times) > dt);
            //var targetlist = results;
            foreach (var item in targetlist)
            {

                LiquidationModel one = new LiquidationModel();
                if (item.exchange.ToUpper() != "BYBIT")
                {
                    one.description = item.side.ToUpper() == "SELL" ? "多单爆仓" : "空单爆仓";
                }
                else
                {
                    one.description = item.side.ToUpper() == "SELL" ? "空单爆仓" : "多单爆仓";
                }
                ObjectUtil.MapTo(item, one);
                list.Add(one);
            }

            return list;

        }
        /// <summary>
        /// 获取大单最后更新时间
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <returns></returns>
        public DateTime GetMaxLastTime()
        {
            DateTime result = new DateTime(1901, 01, 01);
            try
            {
                string sql = string.Format("select top 1 * from[MaxOrder] order by SYS_CreateDate desc");

                result = Convert.ToDateTime(SqlDapperHelper.ExecuteReaderReturnList<MaxOrder>(sql).FirstOrDefault().SYS_CreateDate);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(typeof(DownExchangeData), "获取最后更新时间出错：" + e.Message.ToString());

            }
            return result;
        }


        /// <summary>
        /// 获取大单最后更新时间
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <returns></returns>
        public DateTime GetLQLastTime()
        {
            DateTime result = new DateTime(2021, 02, 27, 13, 00, 00);
            try
            {
                string sql = string.Format("select top 1 * from[Liquidation] order by times desc");

                result = Convert.ToDateTime(SqlDapperHelper.ExecuteReaderReturnList<LiquidationModel>(sql).FirstOrDefault().times);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(typeof(DownExchangeData), "获取最后更新时间出错：" + e.Message.ToString());

            }
            return result;
        }


        /// <summary>
        /// 永续
        /// </summary>
        public void SavePermanentFuturesData(List<LiquidationModel> savelist)
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
                        LogHelper.WriteLog(typeof(object), "保存大单数据发生错误，错误信息:" + ex.Message.ToString());
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }


        /// <summary>
        /// 获取发件信息--旧 已废弃
        /// </summary>
        /// <returns></returns>
        public void SendEmail()
        {
            string subject = string.Empty;
            string dt = DateTime.Now.ToString("yyyy-MM-dd");
            string LQkey = CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd");
            string Maxkey = CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd");
            MaxData max = new MaxData();
            var LQdata = SaveLQData(LQkey);
            var bigdata = max.GetMaxData(Maxkey);
            int count = LQdata.Count;

            while (true)
            {
                int countnow = SaveLQData(LQkey).Count();
                if (dt != DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    count = countnow;
                    LQkey = CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd");
                    dt = DateTime.Now.ToString("yyyy-MM-dd");
                }


                if (countnow > count)
                {
                    LQkey = CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd");
                    Maxkey = CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd");

                    var LQdatanow = SaveLQData(LQkey).OrderByDescending(p => p.times).ToList();
                    var bigdatanow = max.GetMaxData(Maxkey).OrderByDescending(p => p.times).ThenByDescending(p => p.vol).ToList();
                    count = LQdatanow.Count();
                    string lqdata = GetHtmlString(ListToDataTable(LQdatanow));
                    string bigdatas = GetHtmlString(ListToDataTable(bigdatanow));

                    string message = "";
                    message += lqdata;
                    message += "<br/>" + "=========================================================" + "<br/>";
                    message += bigdatas;


                    Console.WriteLine("发送邮件时间长：" + DateTime.Now.ToString());
                    SendEmail(message);
                }
                Thread.Sleep(30000);
            }
        }



        /// <summary>
        /// 获取发件信息
        /// </summary>
        /// <returns></returns>
        public void SendEmails()
        {
        tragin:
            try
            {

                while (true)
                {
                    string LQkey = CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd");
                    string Maxkey = CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd");
                    MaxData max = new MaxData();



                    var LQdatanow = SaveLQDataMax(LQkey);
                    var bigdatanow = max.GetMaxData(Maxkey);

                    List<TradeMatermail> ulist = new List<TradeMatermail>();

                    var PERPRealDeal = RedisHelper.GetHash(CommandEnum.RedisKey.PERPRealDeal + DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") + "_" + "BTC");
                    if (PERPRealDeal != "")
                    {
                        var c = PERPRealDeal.ToObject<TradeMatermail>();

                        c.ContractType = "永续当天汇总";
                        ulist.Add(c);
                    }

                    var PERPRealDealHOUR = RedisHelper.GetHash(CommandEnum.RedisKey.PERPRealDeal + "Hour" + DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd HH") + "_" + "BTC");

                    if (PERPRealDealHOUR != "")
                    {
                        var h = PERPRealDealHOUR.ToObject<TradeMatermail>();
                        h.ContractType = "BTC永续小时汇总";
                        ulist.Add(h);
                    }



                    var PERPRealDealETH = RedisHelper.GetHash(CommandEnum.RedisKey.PERPRealDeal + "Hour" + DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd HH") + "_" + "ETH");

                    if (PERPRealDealETH != "")
                    {
                        var h = PERPRealDealETH.ToObject<TradeMatermail>();
                        h.ContractType = "ETH永续小时汇总";
                        ulist.Add(h);
                    }






                    var DeliverRealDeal = RedisHelper.GetHash(CommandEnum.RedisKey.DeliverRealDeal + DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") + "_" + "BTC");
                    if (DeliverRealDeal != "")
                    {
                        var d = DeliverRealDeal.ToObject<TradeMatermail>();
                        d.ContractType = "交割当天汇总";
                        ulist.Add(d);

                    }





                    var DeliverRealDealHOUR = RedisHelper.GetHash(CommandEnum.RedisKey.DeliverRealDeal + "Hour" + DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd HH") + "_" + "BTC");
                    if (DeliverRealDealHOUR != "")
                    {
                        var dh = DeliverRealDealHOUR.ToObject<TradeMatermail>();
                        dh.ContractType = "交割小时汇总";
                        ulist.Add(dh);
                    }

                    var times = TimeCore.GetHourList(DateTime.Now).Where(p => p <= DateTime.Now);


                    foreach (var item in times)
                    {
                        var SPOTRealDealdate = RedisHelper.GetHash(CommandEnum.RedisKey.PERPRealDeal + "Hour" + DateTime.Now.ToString("yyyy-MM-dd"), item.ToString("yyyy-MM-dd HH") + "_" + "BTC");
                        if (SPOTRealDealdate != "")
                        {
                            var spot = SPOTRealDealdate.ToObject<TradeMatermail>();
                            spot.ContractType = "永续当天每小时汇总";
                            ulist.Add(spot);
                        }
                    }

                    foreach (var item in times)
                    {
                        var SPOTRealDealdate = RedisHelper.GetHash(CommandEnum.RedisKey.PERPRealDeal + "Hour" + DateTime.Now.ToString("yyyy-MM-dd"), item.ToString("yyyy-MM-dd HH") + "_" + "ETH");
                        if (SPOTRealDealdate != "")
                        {
                            var spot = SPOTRealDealdate.ToObject<TradeMatermail>();
                            spot.ContractType = "永续当天每小时汇总";
                            ulist.Add(spot);
                        }
                    }







                    var SPOTRealDeal = RedisHelper.GetHash(CommandEnum.RedisKey.SPOTRealDeal + DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") + "_" + "BTC");
                    if (SPOTRealDeal != "")
                    {
                        var spot = SPOTRealDeal.ToObject<TradeMatermail>();
                        spot.ContractType = "现货当天汇总";
                        ulist.Add(spot);

                    }


                    LQkey = CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd");
                    Maxkey = CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd");

                    string lqdata = "";
                    if (LQdatanow != null)
                    {
                        var tb = ListToDataTable(LQdatanow.OrderByDescending(p => p.times).ToList());
                        remove(tb, "lq");
                        lqdata = GetHtmlString(tb);
                    }
                    string bigdatas = "";
                    if (bigdatanow != null)
                    {
                        var tb = ListToDataTable(bigdatanow.Where(a => a.vol > 2999999).OrderByDescending(p => p.times).ThenByDescending(p => p.vol).ToList());
                        remove(tb, "big");
                        bigdatas = GetHtmlString(tb);
                        //bigdatas = GetHtmlString(ListToDataTable(bigdatanow.Where(a => a.vol > 2999999).OrderByDescending(p => p.times).ThenByDescending(p => p.vol).ToList()));
                    }
                    string tradedata = "";
                    if (ulist != null)
                    {
                        var tb = ListToDataTable(ulist);
                        remove(tb, "uperp");
                        tradedata = GetHtmlString(tb);
                        //tradedata = GetHtmlString(ListToDataTable(ulist));


                    }
                    string openstring = "";

                    openstring = getOpeninsterestData();
                    string message = "";
                    message += lqdata;
                    message += "<br/>" + "=========================================================" + "<br/>";
                    message += bigdatas;
                    message += "<br/>" + "=========================================================" + "<br/>";
                    message += tradedata;
                    message += "<br/>" + "=========================================================" + "<br/>";
                    message += openstring;
                    message += "<br/>" + "=========================================================" + "<br/>";
                    message += getListData();


                    //string message = "";
                    //foreach (var item in LQdatanow)
                    //{
                    //    message += "<br/>" + item.ToJson()+ "<br/>";
                    //}
                    //message = "<br/>" + "=========================================================" + "<br/>";
                    //foreach (var item in bigdatanow)
                    //{
                    //    message += "<br/>" + item.ToJson() + "<br/>";
                    //}
                    Console.WriteLine("发送邮件时间长：" + DateTime.Now.ToString());
                    SendEmail(message);
                    Thread.Sleep(60000);
                    while (true)
                    {
                        if ((DateTime.Now.Minute % 9 == 0))
                        {
                            break;
                        }

                        Thread.Sleep(60000);
                    }


                }
            }
            catch (Exception E)
            {
                Console.WriteLine("发送邮件出错：" + E.Message.ToString());
                goto tragin;
            }

        }

        public DataTable remove(DataTable tb, string type)
        {
            if (type == "lq")
            {
                tb.Columns.Remove(tb.Columns["did"]);
                tb.Columns.Remove(tb.Columns["uuuid"]);
                tb.Columns.Remove(tb.Columns["utctime"]);
                tb.Columns.Remove(tb.Columns["timestamp"]);
                tb.Columns.Remove(tb.Columns["qty"]);
                tb.Columns.Remove(tb.Columns["amount"]);
                tb.Columns.Remove(tb.Columns["pair"]);
                tb.Columns.Remove(tb.Columns["vol"]);
            }
            if (type == "big")
            {
                tb.Columns.Remove(tb.Columns["did"]);
                tb.Columns.Remove(tb.Columns["uuuid"]);
                tb.Columns.Remove(tb.Columns["utctime"]);
                tb.Columns.Remove(tb.Columns["timestamp"]);
                tb.Columns.Remove(tb.Columns["qty"]);
                tb.Columns.Remove(tb.Columns["pair"]);
                tb.Columns.Remove(tb.Columns["vol"]);
            }
            if (type == "uperp")
            {
                tb.Columns.Remove(tb.Columns["did"]);
                tb.Columns.Remove(tb.Columns["uuuid"]);
                tb.Columns.Remove(tb.Columns["utime"]);
                tb.Columns.Remove(tb.Columns["timestamp"]);
                tb.Columns.Remove(tb.Columns["qty"]);
                tb.Columns.Remove(tb.Columns["pair"]);
                tb.Columns.Remove(tb.Columns["Unit"]);
                tb.Columns.Remove(tb.Columns["count"]);
                tb.Columns.Remove(tb.Columns["count_buy"]);
                tb.Columns.Remove(tb.Columns["count_sell"]);

                tb.Columns.Remove(tb.Columns["market"]);
                tb.Columns.Remove(tb.Columns["types"]);
                tb.Columns.Remove(tb.Columns["FundingRateStart"]);
                tb.Columns.Remove(tb.Columns["FundingRateNow"]);



                tb.Columns.Remove(tb.Columns["qty_buy"]);
                tb.Columns.Remove(tb.Columns["qty_sell"]);

                tb.Columns.Remove(tb.Columns["fundingrate"]);
                tb.Columns.Remove(tb.Columns["estimated_rate"]);
                tb.Columns.Remove(tb.Columns["description"]);
                //tb.Columns.Remove(tb.Columns["liquidation_sell"]);
                //tb.Columns.Remove(tb.Columns["liquidation"]);
                tb.Columns.Remove(tb.Columns["basis"]);
                tb.Columns.Remove(tb.Columns["timecode"]);
                //tb.Columns.Remove(tb.Columns["contractcode"]);
            }
            if (type == "open")
            {
                tb.Columns.Remove(tb.Columns["uid"]);
                tb.Columns.Remove(tb.Columns["symbol"]);
                tb.Columns.Remove(tb.Columns["contractcode"]);
                tb.Columns.Remove(tb.Columns["STcoin"]);
                tb.Columns.Remove(tb.Columns["ENcoin"]);
                tb.Columns.Remove(tb.Columns["CoinPrecent"]);
                //tb.Columns.Remove(tb.Columns["qty"]);
            }
            //fundingrate estimated_rate  liquidation_buy liquidation_sell    liquidation basis   SYS_CreateDate timecode    contractcode

            return tb;
        }



        /// <summary>
        /// 获取持仓
        /// </summary>
        /// <returns></returns>
        public string getOpeninsterestData()
        {
            List<OpenInsterestDataMail> savelist = new List<OpenInsterestDataMail>();

            var item = DateTime.Now;


            //永续持仓小时
            var perplisthour = GetList<OpenInsterestDataMail>(item, CommandEnum.RedisKey.PERP + CommandEnum.RedisKey.OpenInterestDataHour);
            //永续持仓日
            var perpdatalist = GetList<OpenInsterestDataMail>(item, CommandEnum.RedisKey.PERP + CommandEnum.RedisKey.OpenInterestDataDate);
            //永续持仓小时
            var deliverhour = GetList<OpenInsterestDataMail>(item, CommandEnum.RedisKey.DELIVERY + CommandEnum.RedisKey.OpenInterestDataHour);
            //永续持仓日
            var deliverdate = GetList<OpenInsterestDataMail>(item, CommandEnum.RedisKey.DELIVERY + CommandEnum.RedisKey.OpenInterestDataDate);


            if (perplisthour.Count > 0)
            {
                perplisthour.ForEach(a => a.types = CommandEnum.RedisKey.PERP + "-" + CommandEnum.RedisKey.OpenInterestDataHour);
                savelist.AddRange(perplisthour.Where(a => Math.Abs(a.SumOpenInterestPrecents) > 3 && a.Times.Hour == DateTime.Now.Hour && a.SumOpenInterestValueEn > 5000000));
            }
            // && a.exchange == CommandEnum.RedisKey.binance
            if (perpdatalist.Count > 0)
            {
                perpdatalist.ForEach(a => a.types = CommandEnum.RedisKey.PERP + "-" + CommandEnum.RedisKey.OpenInterestDataDate);
                savelist.AddRange(perpdatalist.Where(a => Math.Abs(a.SumOpenInterestPrecents) > 6 && a.SumOpenInterestValueEn > 5000000));
            }

            if (deliverhour.Count > 0)
            {
                deliverhour.ForEach(a => a.types = CommandEnum.RedisKey.DELIVERY + "-" + CommandEnum.RedisKey.OpenInterestDataHour);
                savelist.AddRange(deliverhour.Where(a => Math.Abs(a.SumOpenInterestPrecents) > 3 && a.Times.Hour == DateTime.Now.Hour && a.SumOpenInterestValueEn > 5000000));
            }

            if (deliverdate.Count > 0)
            {
                deliverdate.ForEach(a => a.types = CommandEnum.RedisKey.DELIVERY + "-" + CommandEnum.RedisKey.OpenInterestDataDate);
                savelist.AddRange(deliverdate.Where(a => Math.Abs(a.SumOpenInterestPrecents) > 5 && a.SumOpenInterestValueEn > 5000000));
            }

            string openinst = "";
            if (savelist.Count > 0)
            {
                var tb = ListToDataTable(savelist);
                remove(tb, "open");
                openinst = GetHtmlString(tb);

            }
            return openinst;


        }

        /// <summary>
        /// 获取历史永续和交割天汇总
        /// </summary>
        /// <returns></returns>
        public string getListData()
        {
            List<TradeMatermail> ulist = new List<TradeMatermail>();

            DateTime st = new DateTime(2021, 03, 22);
            DateTime end = DateTime.Now;
            var times = TimeCore.GetDate(st, end);


            foreach (var item in times)
            {
                var PERPRealDeal = RedisHelper.GetHash(CommandEnum.RedisKey.PERPRealDeal + item.ToString("yyyy-MM-dd"), item.ToString("yyyy-MM-dd") + "_" + "BTC");
                if (PERPRealDeal != "")
                {
                    var c = PERPRealDeal.ToObject<TradeMatermail>();
                    c.ContractType = "永续当天汇总";
                    ulist.Add(c);
                }
            }
            foreach (var item in times)
            {
                var DeliverRealDeal = RedisHelper.GetHash(CommandEnum.RedisKey.DeliverRealDeal + item.ToString("yyyy-MM-dd"), item.ToString("yyyy-MM-dd") + "_" + "BTC");
                if (DeliverRealDeal != "")
                {
                    var d = DeliverRealDeal.ToObject<TradeMatermail>();
                    d.ContractType = "交割当天汇总";
                    ulist.Add(d);
                }
            }
            string openinst = "";
            if (ulist.Count > 0)
            {
                var tb = ListToDataTable(ulist);
                remove(tb, "uperp");
                openinst = GetHtmlString(tb);

            }
            return openinst;
        }


        /// <summary>
        /// 其他大单统计
        /// </summary>
        public void MaxOtherOrder()
        { 
        
        }



        /// <summary>
        /// 获取发件信息
        /// </summary>
        /// <returns></returns>
        public void SendEmailopen()
        {
        tragin:
            try
            {

                while (true)
                {

                    string openstring = "";

                    openstring = getOpeninsterestData();
                    string message = "";

                    message += openstring;

                    Console.WriteLine("发送邮件时间长：" + DateTime.Now.ToString());
                    SendEmail(message);
                    Thread.Sleep(60000);
                    while (true)
                    {
                        if ((DateTime.Now.Minute % 5 == 0))
                        {
                            break;
                        }

                        Thread.Sleep(60000);
                    }


                }
            }
            catch (Exception E)
            {
                Console.WriteLine("发送邮件出错：" + E.Message.ToString());
                goto tragin;
            }

        }

        //按天保存
        public List<T> GetList<T>(DateTime start, string key)
        {


            List<T> maxlist = new List<T>();
            var tablename = key + start.ToString("yyyy-MM-dd");

            maxlist = RedisHelper.GetAllHashList<T>(tablename);

            return maxlist;
        }


        public void SendEmail(string message)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add("364466548@qq.com");
            //mailMessage.To.Add("q3637466@163.com");
            mailMessage.From = new MailAddress("364466548@qq.com");

            mailMessage.Subject = "BigData" + DateTime.Now.ToString();
            mailMessage.SubjectEncoding = Encoding.UTF8;
            //var ones = RedisHelper.GetALLHash(CommandEnum.RedisKey.timecheck);
            //string title = "";

            //foreach (var item in ones)
            //{
            //    title += "<br/>" + item.Key + item.Value + "<br/>";

            //}




            mailMessage.IsBodyHtml = true;
            mailMessage.Body = message;
            mailMessage.BodyEncoding = Encoding.UTF8;

            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Host = "smtp.qq.com"; //邮件服务器SMTP
            smtpClient.Port = 587; //邮件服务器端口
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("364466548@qq.com", "dghmcrgfkqrbcajd");

            ServicePointManager.ServerCertificateValidationCallback =
delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; };

            smtpClient.Send(mailMessage);
            //邮件服务器验证信息， qq邮箱授权码通过设
        }

        public static DataTable ListToDataTable<T>(List<T> list)
        {
            Type tp = typeof(T);
            PropertyInfo[] proInfos = tp.GetProperties();
            DataTable dt = new DataTable();
            foreach (var item in proInfos)
            {
                //解决DataSet不支持System.Nullable<>问题
                //if (item.Name == "ContractType" || item.Name == "Times" || item.Name == "times" || item.Name == "exchange" || item.Name == "pair" || item.Name == "vol" || item.Name == "price" || item.Name == "qty" || item.Name == "side" || item.Name == "description" || item.Name == "vol_sell" || item.Name == "vol_buy" || item.Name == "market")
                //{
                Type colType = item.PropertyType;
                if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    colType = colType.GetGenericArguments()[0];
                }
                //添加列明及对应类型 

                {
                    dt.Columns.Add(item.Name, colType);
                }
                //}
            }
            foreach (var item in list)
            {
                DataRow dr = dt.NewRow();
                foreach (var proInfo in proInfos)
                {
                    //if (proInfo.Name == "ContractType" || proInfo.Name == "Times" || proInfo.Name == "times" || proInfo.Name == "exchange" || proInfo.Name == "pair" || proInfo.Name == "vol" || proInfo.Name == "price" || proInfo.Name == "qty" || proInfo.Name == "side" || proInfo.Name == "description" || proInfo.Name == "vol_sell" || proInfo.Name == "vol_buy" || proInfo.Name == "market")
                    //{
                    object obj = proInfo.GetValue(item);
                    if (obj == null)
                    {
                        continue;
                    }
                    if (proInfo.PropertyType == typeof(DateTime) && Convert.ToDateTime(obj) < Convert.ToDateTime("1753-01-01"))
                    {
                        continue;
                    }
                    dr[proInfo.Name] = obj;
                    //}
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// DataTable 转换为 Html
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string GetHtmlString(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head>");
            sb.Append("<title>Excel转换为Table</title>");
            sb.Append("<meta http-equiv='content-type' content='text/html; charset=GB2312'> ");
            sb.Append("<style type=text/css>");
            sb.Append("td{font-size: 9pt;border:solid 1 #000000; text-align:center; }");
            sb.Append("table{padding:3 0 3 0;border:solid 1 #000000;margin:0 0 0 0;BORDER-COLLAPSE: collapse;word -break:break-all; word - wrap:break-all; }");

            //word -break:break-all; word - wrap:break-all;
            sb.Append("</style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<table cellSpacing='0' cellPadding='0' width ='100%' border='1'>");
            sb.Append("<tr valign='middle'>");
            sb.Append("<td><b></b></td>");
            foreach (DataColumn column in dt.Columns)
            {
                sb.Append("<td><b><span>" + column.ColumnName + "</span></b></td>");
            }
            sb.Append("</tr>");
            int iColsCount = dt.Columns.Count;
            int rowsCount = dt.Rows.Count - 1;
            for (int j = 0; j <= rowsCount; j++)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + ((int)(j + 1)).ToString() + "</td>");
                for (int k = 0; k <= iColsCount - 1; k++)
                {
                    sb.Append("<td>");
                    object obj = dt.Rows[j][k];
                    if (obj == DBNull.Value)
                    {
                        obj = " ";//如果是NULL则在HTML里面使用一个空格替换之
                    }
                    if (obj.ToString() == "")
                    {
                        obj = " ";
                    }
                    string strCellContent = obj.ToString().Trim();
                    sb.Append("<span>" + strCellContent + "</span>");
                    sb.Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            //点击单元格 输出 行和列
            sb.Append("<script src='https://cdn.bootcss.com/jquery/1.12.4/jquery.min.js'></script>");
            sb.Append("<script type='text/javascript'>");
            sb.Append("$('table tbody').on('click', 'td', function (e) {");
            sb.Append("var row = $(this).parent().prevAll().length-1 ;");
            sb.Append("var column = $(this).prevAll().length-1 ;");
            sb.Append("var str = 'dt.Rows[' + row + '][' + column + '].ToString()';");
            sb.Append("console.log(str);alert(str);");
            sb.Append("});");
            sb.Append("</script>");

            sb.Append("</body></html>");
            return sb.ToString();
        }




        public string getOpeninsterestBetweenData(DateTime st, DateTime end)
        {
            List<OpenInsterestDataMail> savelist = new List<OpenInsterestDataMail>();

            var datelist = TimeCore.GetDate(st, end);


            foreach (var item in datelist)
            {


                //永续持仓小时
                var perplisthour = GetList<OpenInsterestDataMail>(item, CommandEnum.RedisKey.PERP + CommandEnum.RedisKey.OpenInterestDataHour);
                //永续持仓日
                var perpdatalist = GetList<OpenInsterestDataMail>(item, CommandEnum.RedisKey.PERP + CommandEnum.RedisKey.OpenInterestDataDate);



                if (perplisthour.Count > 0)
                {
                    perplisthour.ForEach(a => a.types = CommandEnum.RedisKey.PERP + "-" + CommandEnum.RedisKey.OpenInterestDataHour);
                    savelist.AddRange(perplisthour.Where(a => Math.Abs(a.SumOpenInterestPrecents) > 4 && a.Times.Hour == DateTime.Now.Hour && a.SumOpenInterestValueEn > 5000000));
                }
                // && a.exchange == CommandEnum.RedisKey.binance
                if (perpdatalist.Count > 0)
                {
                    perpdatalist.ForEach(a => a.types = CommandEnum.RedisKey.PERP + "-" + CommandEnum.RedisKey.OpenInterestDataDate);
                    savelist.AddRange(perpdatalist.Where(a => Math.Abs(a.SumOpenInterestPrecents) > 6 && a.SumOpenInterestValueEn > 5000000));
                }

            }

            string openinst = "";
            if (savelist.Count > 0)
            {
                var tb = ListToDataTable(savelist);
                remove(tb, "open");
                openinst = GetHtmlString(tb);

            }
            return openinst;


        }

    }
}
