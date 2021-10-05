using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace CoinWin.DataGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            //DownExchangeData helper = new DownExchangeData();
            //helper.UpdateExchageDataBydate("1pass");

            //BBODataProcess bbo = new BBODataProcess();
            //bbo.CheckCount();
            //bbo.CalcBooData();

            //HttpClientDataDown t = new HttpClientDataDown();
            //t.downdata("");


            strategyThread();


            //LiqudationData d = new LiqudationData();
            //d.SendEmails();


            // InsterestProcess o = new InsterestProcess();
            //o.CalcInsterestHour(new DateTime(2021,05,06));
            ////o.CalcInsterestDay();

            //LiqudationData d = new LiqudationData();



            //new Thread(() =>
            //{
            //    d.SendEmailopen();
            //}).Start();



            //InsterestProcess b= new InsterestProcess();
            //b.getOpeninsterestData();

            //strategyThread();
            //LiquidationDataProcess t = new LiquidationDataProcess();
            //t.ProcessLQdata();

            //t.ProcessLQAlldata(new DateTime(2021,03,30));
            //MongoRepository<>


            //MongoDbHelpers ts = new MongoDbHelpers();
            //ts.Getlqdata();

            //string key = "PERPOpenInsertDataHour2021-04-19";
            //var hourtradeMasterlist = RedisHelper.GetAllHashList<OpenInsterestDatas>(key);

            //LiquidationDataProcess aq = new LiquidationDataProcess();
            //aq.ProcessLQAlldata(new DateTime(2021, 04, 20));



            ////保存其他数据
            //OtherSaver t = new OtherSaver();
            //t.RunAll(DateTime.Now);
            ////删除辅助数据 
            //t.DelteAll(DateTime.Now);


            //////保存成交数据
            //DataSaver der = new DataSaver();
            ////der.Runall();

            //////删除交易对成交数据    
            //der.DeletedAll();





            //t.RunAlltime(new DateTime(2021,04,20));




            //t.DelteAll(DateTime.Now);
            //t.RunAll(DateTime.Now);


            //LiqudationData d = new LiqudationData();

            //new Thread(() =>
            //{
            //    d.SendEmails();
            //}).Start();

            //InsterestProcess o = new InsterestProcess();
            ////////o.CalcInsterestHour();
            ////////o.CalcInsterestDay();


            //new Thread(() =>
            //{
            //    while (true)
            //    {


            //        Console.WriteLine("持仓日统计开始！" + DateTime.Now);
            //        o.CalcInsterestDay();
            //        Console.WriteLine("持仓日统计结束！" + DateTime.Now);
            //        Thread.Sleep(300000);
            //    }


            //}).Start();


            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Console.WriteLine("持仓小时统计开始！" + DateTime.Now);
            //        o.CalcInsterestHour(DateTime.Now.AddHours(-1));
            //        Console.WriteLine("持仓小时统计结束！" + DateTime.Now);
            //        Thread.Sleep(300000);
            //    }
            //}).Start();


            //CRDataOut t = new CRDataOut();
            //var openlist = t.GetDataObject<OpenInterest>(CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd"));
            //var opendatas = openlist.OrderByDescending(p => Convert.ToDateTime(p.times)).ToList();
            //opendatas = opendatas.Distinct(new OpenInterestComparer()).ToList();
            //var openstartdata = openlist.OrderBy(p => Convert.ToDateTime(p.times)).ToList();
            //openstartdata = openstartdata.Distinct(new OpenInterestComparer()).ToList();
            //foreach (var item in openstartdata)
            //{
            //    var content = "时间："+item.times+"交易所和币对："+item.exchange+"|"+item.symbol+"|"+item.market+"|当前持仓价值usdt："+item.SumOpenInterestValue+"|持仓币"+item.coin+"|24小时成交量："+item.volumeUsd24h;

            //    LogHelper.WriteLog(typeof(DownExchangeData), content);
            //}
            //var frdung = t.GetDataObject<FundRate>(CommandEnum.RedisKey.FundingRate + DateTime.Now.ToString("yyyy-MM-dd"));
            //Console.ReadKey();

            //DataSaver der = new DataSaver();
            // der.Runall();

            // MongoDbHelpers mongo = new MongoDbHelpers();
            //mongo.Getlqdata();

            // //PerpDataSaver perp = new PerpDataSaver();
            // //perp.Runall();

            // OtherSaver save = new OtherSaver();
            // save.RunAll();

            // save.SaveLqOrder(new DateTime(2021, 03, 30));
            //save.SaveMaxLQOrder(new DateTime(2021,02,25));

            //mongo.savedate();

            //var times = TimeCore.GetHourList(DateTime.Now).Where(p => p <= DateTime.Now);
            //Console.ReadKey(true);

            //UPermanentFutures model = new UPermanentFutures();
            //model.Unit = "1";
            //model.side = "buy";
            //model.times = "1111";

            //UPermanentFutures oneS = new UPermanentFutures();
            //ObjectUtil.MapTo(model, oneS);

            //Program o = new Program();
            //o.CalcPERPFuturesData();
            //o.CalcDeliveryFuturesData();



            ////o.deletehash();
            //Program o = new Program();
            //o.LQDatanew();//爆仓数据汇总计算
            //o.MaxData();//大单统计

            //o.CalcPERPFuturesData();


            //   Dictionary<string, List<BinanceFuturesSymbol>> dit = new Dictionary<string, List<BinanceFuturesSymbol>>();

            //var   dit = RedisHelper.GetAllHash<List<BinanceFuturesSymbol>>("SymbolList");
            //   var list = dit.Values.FirstOrDefault();



            //   foreach (var item in dit)
            //   {

            //   }




            AllTradeProcess t = new AllTradeProcess();



            //new Thread(() =>
            //{
                t.CalcPere();
            //}).Start();

            //new Thread(() =>
            //{
            //    t.CalcDeliver();
            //}).Start();


            //new Thread(() =>
            //{
            //    t.CalcSpot();
            //}).Start();




            //t.CalcPere();

            //QuenProcess o = new QuenProcess();
            //o.CalcSpot();
            ////o.CalcDeliver();

            //new Thread(() =>
            //{
            //    o.CalcPere();
            //}).Start();

            //new Thread(() =>
            //{
            //    o.CalcDeliver();
            //}).Start();


            //new Thread(() =>
            //{
            //    o.CalcSpot();
            //}).Start();



            //Program o = new Program();
            //o.LQDatanew();//爆仓数据汇总计算
            //o.MaxData();//大单统计


            //o.CalcPERPFuturesData();
            //  永续数据汇总计算

            //o.CalcDeliveryFuturesData();//交割数据汇总计算


            //o.CalcDeliveryFuturesData();
            //o.LQDatanew();//爆仓数据汇总计算
            //o.MaxData();//大单统计

            //o.deletekeys();
            //QuenProcess o = new QuenProcess();
            //o.CalcPere();
            //o.CalcPere();
            //o.CalcDeliver();
            //o.CalcSpot();

            //new Thread(() =>
            //{
            //    o.CalcPere();
            //}).Start();


            //new Thread(() =>
            //{
            //    o.CalcDeliver();
            //}).Start();


            //new Thread(() =>
            //{
            //    o.CalcSpot();
            //}).Start();


            //LiqudationData d = new LiqudationData();
            //d.SendEmails();



            //  o.LQData();//爆仓数据汇总计算



            //   o.CalcPERPFuturesData();
            //  永续数据汇总计算

            //  o.CalcDeliveryFuturesData();//交割数据汇总计算


            //var datetime = new DateTime(2021, 02, 08, 02, 00, 00);

            //string key = datetime.ToString("yyyy-MM-dd HH") + "UPermanentFutures";

            ////var resultsss = RedisHelper.GetSet(key, "");

            //var results = RedisHelper.GetSetSScan(key, "");

            string kesy = "结束数据统计";
            //Process.GetCurrentProcess().Kill();
            Console.WriteLine(kesy);

            Console.ReadKey(true);

            //CRDataOut.DownData(data);
            //var sqw = s.ToList<DeliveryFutures>();
            //o.SaveListDelivery(sqw);
        }

        public void deletekeys()
        {

            DateTime st = new DateTime(2021, 04, 02, 17, 00, 00);

            DateTime end = DateTime.Now;
            List<DateTime> dtlist = new List<DateTime>();
            dtlist = TimeCore.GetStartAndEndTime(st, end);



            foreach (var item in dtlist)
            {

                string keys = "liquidation" + item.ToString("yyyy-MM-dd HH");

                bool results = RedisHelper.DeleteKeys(keys);
                if (results)
                {
                    Console.WriteLine("删除key成功：" + keys);
                    LogHelper.WriteLog(typeof(DownExchangeData), "删除key成功：" + keys);
                }
            }
        }

        public void deletehash()
        {
            var redisclient = FreeRedisHelper.CreateInstance("");

            var ones = redisclient.HKeys("LQTimeStamp");
            foreach (var item in ones.Where(p => p.Contains("okex")))
            {
                redisclient.HDel("LQTimeStamp", item);
            }

        }

        public void CalcDeliveryFuturesData()
        {

            CRDataOut CRDataOut = new CRDataOut();

            List<UPermanentFuturesModel> data = new List<UPermanentFuturesModel>();

            DateTime dt = CRDataOut.GetLastTime("JG");
            DateTime end = DateTime.Now;



            //DateTime dt = DateTime.Now.AddHours(-3);
            //DateTime end = DateTime.Now;

            List<DateTime> dtlist = new List<DateTime>();

            dtlist = TimeCore.GetStartAndEndTime(dt, end);

            LogHelper.WriteLog(typeof(DownExchangeData), "根据redsi 数据 存储数据：开始和结束时间：" + dt.ToString() + "to" + end.ToString());



            foreach (var item in dtlist)
            {

                if (item.Hour == DateTime.Now.Hour)
                {
                    continue;
                }

                var st = item;
                var en = item.AddHours(1);

                string keys = item.ToString("yyyy-MM-dd HH") + "UDeliveryFutures";

                Console.WriteLine("获取数据：" + keys);

                var datas = CRDataOut.GetData(keys);

                Console.WriteLine(keys);

                if (datas == null || datas.Count == 0)
                {
                    LogHelper.WriteLog(typeof(DownExchangeData), "数据为空，跳过：" + keys);
                    Console.WriteLine("数据为空，跳过：" + keys);
                    continue;
                }


                CRDataOut.DownDeliveryFuturesData(datas, st, en);

                bool results = RedisHelper.DeleteKeys(keys);

                if (results)
                {
                    Console.WriteLine("删除key成功：" + keys);
                    LogHelper.WriteLog(typeof(DownExchangeData), "删除key成功：" + keys);
                }
                else
                {
                    Console.WriteLine("删除key失败：" + keys);
                    LogHelper.WriteLog(typeof(DownExchangeData), "删除key失败：" + keys);
                }
                //}).Start();
            }
        }

        public void CalcPERPFuturesData()
        {
            try
            {

                CRDataOut CRDataOut = new CRDataOut();

                List<UPermanentFuturesModel> data = new List<UPermanentFuturesModel>();

                DateTime dt = CRDataOut.GetLastTime("YX");
                DateTime end = DateTime.Now;

                //DateTime dt = DateTime.Now.AddHours(-3);
                //DateTime end = DateTime.Now;
                List<DateTime> dtlist = new List<DateTime>();

                dtlist = TimeCore.GetStartAndEndTime(dt, end);

                LogHelper.WriteLog(typeof(DownExchangeData), "根据redsi 数据 存储数据：开始和结束时间：" + dt.ToString() + "to" + end.ToString());

                foreach (var item in dtlist)
                {

                    if (item.Hour == end.Hour)
                    {
                        continue;
                    }

                    var st = item;
                    var en = item.AddHours(1);

                    string keys = item.ToString("yyyy-MM-dd HH") + "UPermanentFutures";

                    Console.WriteLine("获取数据：" + keys);

                    data = CRDataOut.GetData(keys);

                    Console.WriteLine(keys);

                    CRDataOut.DownPERPFuturesData(data, st, en);

                    data = null;
                    bool results = RedisHelper.DeleteKeys(keys);

                    if (results)
                    {
                        Console.WriteLine("删除key成功：" + keys);
                        LogHelper.WriteLog(typeof(DownExchangeData), "删除key成功：" + keys);
                    }
                    else
                    {
                        Console.WriteLine("删除key失败：" + keys);
                        LogHelper.WriteLog(typeof(DownExchangeData), "删除key失败：" + keys);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("处理永续数据汇总出错，错误信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "处理永续数据汇总出错，错误信息：" + e.Message.ToString());
            }
        }

        public void MaxData()
        {
            try
            {
                MaxData l = new MaxData();
                DateTime dt = l.GetLastTime();

                DateTime st = dt;
                DateTime end = DateTime.Now;

                var list = TimeCore.GetDate(st, end);



                foreach (var item in list)
                {
                    var key = CommandEnum.RedisKey.MaxOrder + item.ToString("yyyy-MM-dd");
                    l.SaveMaxData(key);
                }
                Console.WriteLine("结束大单数据持久化！");
            }
            catch (Exception e)
            {
                Console.WriteLine("大单数据持久化出错，错误信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "大单数据持久化出错，错误信息：" + e.Message.ToString());
            }

        }

        // Token: 0x06000041 RID: 65 RVA: 0x00002D50 File Offset: 0x00000F50
        public static void strategyThread()
        {
            DownExchangeData helper = new DownExchangeData();

            for (int i = 0; i < 1000000000; i++)
            {
                //获取过去
                //   helper.UpdateExchageDataBymin("pass");

                //获取现在
                helper.UpdateExchageDataBymin("1pass");

                Thread.Sleep(120000);
            }


            // helper.UpdateExchageDataBymin("pass");
            //helper.UpdateExchangeDataNow();
            //helper.UpdateExchageData();

        }

        public void DownDataByhttp()
        {
            string urlss = "https://jingci.wjx.cn/corplogin.aspx";
            var handlers = new HttpClientHandler();
            handlers.UseCookies = true;
            //注意：登錄之後不設置跳轉
            handlers.AllowAutoRedirect = true;
            //var cookies = new CookieContainer();
            //handlers.CookieContainer = cookies;
            HttpClient httpClients = new HttpClient(handlers);
            httpClients.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            httpClients.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            httpClients.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            httpClients.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");// 
            httpClients.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,fil;q=0.7,zh-TW;q=0.6");// 
            httpClients.DefaultRequestHeaders.Add("Origin", "https://jingci.wjx.cn");// 
            httpClients.DefaultRequestHeaders.Add("Referer", "https://jingci.wjx.cn/corplogin.aspx");// 
            httpClients.DefaultRequestHeaders.Add("Connection", "keep-alive");
            //登录页
            var response = httpClients.PostAsync(new Uri(urlss), new FormUrlEncodedContent(paramhelper.getwjxParma())).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var cookie = handlers.CookieContainer;


            Console.WriteLine("进入登录页：");
            Console.WriteLine("登录成功！");



            var cookies = new CookieContainer();
            cookies.Add(handlers.CookieContainer.GetCookies(new Uri(urlss)));

            var handl = new HttpClientHandler();
            handl.AllowAutoRedirect = true;
            handl.UseCookies = true;
            handl.CookieContainer = cookies;
            HttpClient httpClientfile = new HttpClient(handl);

            //DownFile(httpClientfile, cookies);
        }

        public static void strategyThreadPASS()
        {

            DownExchangeData helper = new DownExchangeData();

            for (int i = 0; i < 1000000000; i++)
            {
                LogHelper.WriteLog(typeof(DownExchangeData), "获取过去 通用数据");
                //获取过去
                helper.UpdateExchageDataBymin("pass");

                //获取现在
                //helper.UpdateExchageDataBymin("1pass");
                Thread.Sleep(60000);
            }


        }

        public static void GetData()
        {

            string key = DateTime.Now.ToString("yyyy-MM-dd") + "UDeliveryFutures";
            var results = RedisHelper.GetSet(key, "DB0");
            List<UPermanentFuturesModel> list = new List<UPermanentFuturesModel>();


            if (results != null && results.Count > 0)
            {
                foreach (var item in results)
                {
                    UPermanentFuturesModel one = new UPermanentFuturesModel();
                    ObjectUtil.MapTo(item, one);
                    list.Add(one);
                }
                RedisHelper.SaveData(list);
            }
        }

        /// <summary>
        /// 爆仓数据保存-弃用
        /// </summary>
        public void LQData()
        {

            LiqudationData l = new LiqudationData();
            DateTime dt = l.GetLQLastTime();

            DateTime st = dt.AddHours(1);
            DateTime end = DateTime.Now.AddHours(-1);

            var list = TimeCore.GetStartAndEndTime(st, end);



            if (list == null)
            {
                Console.WriteLine("爆仓小时数据未完成，无法持久化！");
                return;
            }


            List<LiquidationModel> alllist = new List<LiquidationModel>();
            foreach (var item in list)
            {
                var key = CommandEnum.RedisKey.liquidation + item.ToString("yyyy-MM-dd HH");
                Console.WriteLine(key);
                var result = l.SaveLQData(key);
                if (result != null)
                {
                    alllist.AddRange(result);
                }
            }
            var resultlist = alllist.Distinct(new ProductNoComparer());


            LiqudationData o = new LiqudationData();
            o.SavePermanentFuturesData(resultlist.ToList());

            Console.WriteLine("结束爆仓数据持久化！");
        }

        /// <summary>
        /// 爆仓数据保存
        /// 无法处理之前未保存的数据
        /// </summary>
        public void LQDatanew()
        {
            try
            {
               

                LiqudationData l = new LiqudationData();
                DateTime dt = l.GetLQLastTime();
                if (dt.Hour == DateTime.Now.Hour)
                {
                    return;
                }


                DateTime end = DateTime.Now;

                var list = TimeCore.GetDate(dt, end);

                if (list == null)
                {
                    Console.WriteLine("爆仓小时数据未完成，无法持久化！");
                    return;
                }

                List<LiquidationModel> alllist = new List<LiquidationModel>();
                foreach (var item in list)
                {
                    var key = CommandEnum.RedisKey.liquidation + item.ToString("yyyy-MM-dd");
                    Console.WriteLine(key);

                    var result = l.SaveLQDataNEW(key, dt);
                    if (result != null)
                    {
                        alllist.AddRange(result);
                    }
                }
                var resultlist = alllist.Distinct(new ProductNoComparer());



                LiqudationData o = new LiqudationData();
                o.SavePermanentFuturesData(resultlist.ToList());

                Console.WriteLine("结束爆仓数据持久化！");
            }
            catch (Exception e)
            {
                Console.WriteLine("爆仓数据持久化出错，错误信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "爆仓数据持久化出错，错误信息：" + e.Message.ToString());
            }
        }

          
    }

    static class RunOne
    {
        private static bool flag = false;

        public static string getrandomname()
        {
            var rundom = new Random();
            return rundom.Next(1, 20000).ToString();
        }
        private static System.Threading.Mutex mutex = new System.Threading.Mutex(true, getrandomname(), out flag);
        public static bool IsAlreadyRun()//返回true为此程序已经有在运行，返回false则为此程序没有在运行
        {
            return !flag;
        }
    }

    //public class Rootobject
    //{

    //    public Rootobject()
    //    {
    //        this.@event = "subscribe";



    //    }
    //    public string @event { get; set; }
    //    public string[] pair { get; set; }
    //    public Subscription subscription { get; set; }
    //}

    //public class Subscription
    //{
    //    public string name { get; set; }
    //}
}
