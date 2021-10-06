using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using System.Web;
using Newtonsoft.Json;
using QPP.Core;
using WebSocketSharp;
using System.Diagnostics;//记得加入此引用
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using GetTradeHistoryData.BaseCore;

namespace GetTradeHistoryData
{
    class Program
    {
        static void Main(string[] args)
        {

            //object temp2 = null;
            //string temp1 = temp2.ToString();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException); //;OnDomainUnhandledExceptio


            string yesORno;
            bool flag = true;
            //var SQW = CommandEnum.Karkens.GetSymbolList().Where(a=>a.tradeable=="true").ToList();
            //var sq = SQW.ToJson();
            //var list = CommandEnum.Karkens.GetSymbolList().Select(p => p.symbol).ToList();
            //var op = ($"{{\"event\":\"subscribe\", \"product_ids\":{list.ToJson()},\"feed\":\"trade\"}}");
            //string ops = $"{{\"event\":\"subscribe\",\"product_ids\":[\"fi_bchusd_210625\",\"PI_XBTUSD\"],\"feed\":\"trade\"}}";
            //string opss = $"{{\"event\":\"subscribe\",\"product_ids\":[{list.ToJson()}],\"feed\":\"trade\"}}";
            //UpdateSymbolList();
            //var list = CommandEnum.coinbasepro.GetSymbolList().Where(a => a.quote_currency == "USD" && a.status == "online").Select(p => p.id).ToList();
            //var op = ("{\"type\":\"subscribe\", \"channels\":[{\"name\":\"matches\",\"product_ids\":"+list.ToJson() +"}]}");



            //FTXSpotWebScoket clients = new FTXSpotWebScoket("wss://ftx.com/ws/");
            //clients.Start(clients.GetSendData());
            //clients.SendMessage(clients.GetSendData());
            //o.runhuobis();

            //Allrun t = new Allrun();
            //t.huobi();

            //new Thread(() =>
            //{
            //    LogHelper.CreateInstance().Info("开始获取kraken数据");
            //    KarkenSpotWSocketClient clients = new KarkenSpotWSocketClient("wss://ws.kraken.com");
            //    clients.Start(clients.GetSendData());
            //    clients.SendMessage(clients.GetSendData());

            //}).Start();



            //new Thread(() =>
            //{
            //    HuobiSpotWebscoket client = new HuobiSpotWebscoket("wss://api.huobi.pro/ws", "");
            //    client.Start(client.GetSendData());
            //    client.SendMessage("");
            //}).Start();


            //new Thread(() =>
            //{
            //    OkexWebscoketSpot okexclient = new OkexWebscoketSpot("wss://real.okex.com:8443/ws/v3", "");
            //    okexclient.Start(okexclient.GetSendData());
            //    okexclient.SendMessage("");
            //}).Start();

            BybitLQhelper p = new BybitLQhelper();
            p.runtest();


            //RunAll o = new RunAll();
            //o.liquidationData();
            //o.runspots();
            //o.runspot();


            //o.liquidationData();

            //RunAll o = new RunAll();


            //new Thread(() =>
            //{
            //    LogHelper.CreateInstance().Info("开始获取huobi交割数据");
            //    huobiWebScoketDeliveryFutures clients = new huobiWebScoketDeliveryFutures("wss://api.btcgateway.pro/ws", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV,ADA,FIL,ETC");
            //    clients.Start(clients.GetSendData());
            //    clients.SendMessage(clients.GetSendData());
            //}).Start();


            //o.FundingOpeninsert();
            // o.runspots();

            //  o.RunhuobiFutures();//火币
            // o.RunFutures();//ftx bybit 
            // o.RunActuals(); //Deribit bitmex



            // LogHelper.CreateInstance().Info("开始获取ftx永续和期货数据");
            //FtxMarketHelper.GetSymbollist("Part");
            //FTXWebScoket fTXWebScoket = new FTXWebScoket("wss://ftx.com/ws/", FtxMarketHelper.GetSymbollist("Part"), "0326");


            //LogHelper.CreateInstance().Info("开始获取币本位永续huobi数据");
            //huobiWebScoket client = new huobiWebScoket("wss://api.hbdm.pro/swap-ws", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SUSHI,ETC,ADA", "USD");
            //client.Start(client.GetSendData());
            //client.SendMessage(client.GetSendData());






            //LogHelper.CreateInstance().Info("开始获取huobi交割数据");
            //huobiWebScoketDeliveryFutures clients = new huobiWebScoketDeliveryFutures("wss://api.hbdm.com/ws", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV,ADA,FIL,ETC");
            //clients.Start(clients.GetSendData());
            //clients.SendMessage(clients.GetSendData());


            //LogHelper.CreateInstance().Info("开始获取Okex永续币本位和USDT数据");
            //OkexWebscoket OkexWebscoketclient = new OkexWebscoket("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ADA");
            //OkexWebscoketclient.Start(OkexWebscoketclient.GetSendData());
            //OkexWebscoketclient.SendMessage(OkexWebscoketclient.GetSendData());



            //RunAll o = new RunAll();
            //LogHelper.CreateInstance().Info("开始获取Okex交割数据");
            //OkexWebscoketDeliveryFutures Okexclient = new OkexWebscoketDeliveryFutures("wss://real.okex.com:8443/ws/v3", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP,ETC,BSV", "210326");
            //Okexclient.Start(Okexclient.GetSendData());
            //Okexclient.SendMessage(Okexclient.GetSendData());

            //o.FundingOpeninsert();
            //o.runspots();
            //o.liquidationData();
            // o.RunhuobiFutures();//火币
            //o.RunFutures();//ftx bybit 
            //o.RunPermanentOKFutures();

            //o.RunbinanceFutures();//币安





            //o.RunPermanentOKFutures();//ok



            //o.RunFutures();//ftx bybit 


            //o.RunPermanentOKFutures();//ok

            //o.runspot();

            //o.liquidationData();
            //o.FundingOpeninsert();
            //OkexLQHelper t = new OkexLQHelper();
            //t.runSwap();
            //o.RunActuals();
            //BitmexMarket t = new BitmexMarket();
            //KarkenMarket k = new KarkenMarket();

            //t.GetFundingRateAndOpenInterest();
            //k.GetFundingRateAndOpenInterest();


            //o.RunPermanentOKFutures();
            //o.RunhuobiFutures();
            //o.liquidationData();
            //o.FundingOpeninsert();








            //o.RunActuals();

            //BinanceFuturesUsdtMarket binance = new BinanceFuturesUsdtMarket();

            ////binance
            //string BINANCEFUTURESUSDTMARKET_DEFAULT_HOST = "https://fapi.binance.com/";
            //string BINANCEFUTURESUSDMARKET_DEFAULT_HOST = "https://dapi.binance.com/";

            //binance.GetFundingRateAndOpenInterest(BINANCEFUTURESUSDMARKET_DEFAULT_HOST, "USD");

            //binance.GetFundingRateAndOpenInterest(BINANCEFUTURESUSDTMARKET_DEFAULT_HOST, "USDT");
            //o.liquidationData();
            //o.FundingOpeninsert();
            //o.liquidationData();
            //o.RunhuobiFutures();//火币
            //o.runspot();
            Console.WriteLine("是否继续输入:请输入y或者n");
            yesORno = Console.ReadKey().Key.ToString();

            switch (yesORno)
            {
                case "Y":
                    flag = true;
                    break;
                case "N":
                    flag = false;
                    break;
            }

            if (flag)
            {
                //Process.GetCurrentProcess().Kill();
            }

        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           // ExceptionToMessageHelper.WriteLog(string.Format("应用程序出现异常，请重新启动！异常情况如下:\r\n{0}", e.ExceptionObject.ToString()));
           //Console.WriteLine(-40003011, "应用程序出现异常，请重新启动！"); //发送消息到监控系统
            LogHelper.CreateInstance().Error(string.Format("应用程序出现异常，请重新启动！异常情况如下:\r\n{0}", e.ExceptionObject.ToString()));
            LogHelper.CreateInstance().Error("应用程序出现异常，请重新启动！");
         
            //关闭当前实例
            //Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 永续的Webscoket
        /// </summary>
        public static void strategyThread()
        {
            RunAll o = new RunAll();
            o.RunPermanentOKFutures();
        }

        /// <summary>
        /// 交割的Webscoket
        /// </summary>
        public static void strategyThreadPASS()
        {
            RunAll o = new RunAll();
            o.RunhuobiFutures();

        }

        public async Task<HuobiFundingRate> GetFundingRateAsync(string contractCode)
        {
            // location
            string location = $"/linear-swap-api/v1/swap_funding_rate?contract_code={contractCode}";
            string DEFAULT_HOST = "api.btcgateway.pro";
            string url = DEFAULT_HOST+(location);
            return await HttpRequest.GetAsync<HuobiFundingRate>(url);


        }


        public static void sendmessage()
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add("364466548@qq.com");
            mailMessage.To.Add("q3637466@163.com");
            mailMessage.From = new MailAddress("364466548@qq.com");

            mailMessage.Subject = "ASP.NET e-mail test";
            mailMessage.SubjectEncoding = Encoding.UTF8;
          var ones=  RedisHelper.GetALLHash(CommandEnum.RedisKey.timecheck);
            string title = "";
      
            foreach (var item in ones)
            {
                title += "<br/>" + item.Key+ item.Value+ "<br/>";
           
            }
           



            mailMessage.IsBodyHtml = true;
            mailMessage.Body = title;
            mailMessage.BodyEncoding = Encoding.UTF8;
      
            SmtpClient smtpClient = new SmtpClient();
           
            smtpClient.Host = "smtp.qq.com"; //邮件服务器SMTP
            smtpClient.Port = 587; //邮件服务器端口
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("364466548@qq.com", "sqzwfsqormsvcahg");
            smtpClient.Send(mailMessage);
            //邮件服务器验证信息， qq邮箱授权码通过设
        }

        public static void UpdateSymbolList()
        {
          var list  =BinanceFuturesUsdtMarket.GetSymbolFutureList().Where(p=>p.ContractType!= "CURRENT_QUARTER");
            RedisHelper.SetOrUpdateHash("SymbolList", CommandEnum.RedisKey.PERP, list.ToJson());
            //RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UPermanentFutures);
        }


 
    }



    public class Rootobject
    {

        public Rootobject()
        {
            this.@event = "subscribe";



        }
        public string @event { get; set; }
        public string[] pair { get; set; }
        public Subscription subscription { get; set; }
    }

    public class Subscription
    {
        public string name { get; set; }
    }

}


//try
//{


//    ThreadStart childref = new ThreadStart(strategyThread);
//    Console.WriteLine("永续的Webscoket");
//    Thread childThread = new Thread(strategyThread);
//    childThread.Start();



//    ThreadStart childref2 = new ThreadStart(strategyThreadPASS);
//    Console.WriteLine("交割的Webscoket");
//    Thread childThread2 = new Thread(strategyThreadPASS);
//    childThread2.Start();

//}
//catch (Exception e)
//{
//    Console.WriteLine("致命错误："+e.Message.ToString());
//    LogHelper.CreateInstance().Error("致命错误：" + e.Message.ToString());
//}







// List<Ftx> testlist = new List<Ftx>();
// Ftx a = new Ftx();
// Ftx b = new Ftx();
// a.time = DateTime.Now.ToString();
// b.time = DateTime.Now.ToString();
// testlist.Add(a);
// testlist.Add(b);


//var ones= testlist.ToJson();

//LogHelper.CreateInstance().Info("开始获取bybit数据");
//bybitWebScoket bybitWebScoket = new bybitWebScoket("wss://stream.bytick.com/realtime");
//bybitWebScoket.Start(bybitWebScoket.GetSendData());
//bybitWebScoket.SendMessage(bybitWebScoket.GetSendData());
//Console.ReadKey(true);
//bybitWebScoket.Dispose();
//FreeRedisHelper.Dispose();
//LogHelper.CreateInstance().Info("关闭获取数据");


//LogHelper.CreateInstance().Info("开始获取bitmex数据");
//bitmexWebscoket bitmexclient = new bitmexWebscoket("wss://www.bitmex.com/realtime?subscribe=trade:XBTUSD");
//bitmexclient.Start(bitmexclient.GetSendData());
//bitmexclient.SendMessage(bitmexclient.GetSendData());
//Console.ReadKey(true);
//bitmexclient.Dispose();

//FreeRedisHelper.Dispose();
//LogHelper.CreateInstance().Info("关闭获取数据");

//LogHelper.CreateInstance().Info("开始获取ftx数据");
//FTXWebScoket fTXWebScoket = new FTXWebScoket("wss://ftx.com/ws/");
//fTXWebScoket.Start(fTXWebScoket.GetSendData());
//fTXWebScoket.SendMessage(fTXWebScoket.GetSendData());
//Console.ReadKey(true);
//fTXWebScoket.Dispose();
//FreeRedisHelper.Dispose();
//LogHelper.CreateInstance().Info("关闭获取数据");



//LogHelper.CreateInstance().Info("开始获取kraken数据");
//KarkenWSocketClient client = new KarkenWSocketClient("wss://ws.kraken.com");
//client.Start(client.GetSendData());
//client.SendMessage(client.GetSendData());
//Console.ReadKey(true);
//client.Dispose();


//LogHelper.CreateInstance().Info("关闭获取数据");


//LogHelper.CreateInstance().Info("开始获取huobi数据");
//huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/linear-swap-ws","BTC,LTC,BCH,ETH");
//client.Start(client.GetSendData());
//client.SendMessage(client.GetSendData());
//Console.ReadKey(true);
//client.Dispose();


//LogHelper.CreateInstance().Info("关闭获取数据");



//LogHelper.CreateInstance().Info("开始获取huobi永续数据");
//huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/linear-swap-ws", "BTC,LTC,BCH,EOS,ETH,LINK,DOT,XRP,UNI");
//client.Start(client.GetSendData());
//client.SendMessage(client.GetSendData());
//Console.ReadKey(true);
//client.Dispose();


//LogHelper.CreateInstance().Info("关闭获取数据");



//LogHelper.CreateInstance().Info("开始获取huobi交割数据");
//huobiWebScoketDeliveryFutures clients = new huobiWebScoketDeliveryFutures("wss://api.btcgateway.pro/ws", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP");
//clients.Start(client.GetSendData());
//clients.SendMessage(client.GetSendData());
//Console.ReadKey(true);
//clients.Dispose();


//LogHelper.CreateInstance().Info("关闭获取数据");




//LogHelper.CreateInstance().Info("开始获取huobi永续数据");
//huobiWebScoket client = new huobiWebScoket("wss://api.btcgateway.pro/linear-swap-ws", "BTC,LTC,BCH,EOS,ETH,LINK,DOT,XRP,UNI");
//client.Start(client.GetSendData());
//client.SendMessage(client.GetSendData());




//LogHelper.CreateInstance().Info("开始获取huobi交割数据");
//huobiWebScoketDeliveryFutures clients = new huobiWebScoketDeliveryFutures("wss://api.btcgateway.pro/ws", "BTC,LTC,BCH,ETH,EOS,LINK,DOT,XRP");
//clients.Start(client.GetSendData());
//clients.SendMessage(client.GetSendData());
//Console.ReadKey(true);
//clients.Dispose();


//client.Dispose();


//LogHelper.CreateInstance().Info("关闭获取数据");

