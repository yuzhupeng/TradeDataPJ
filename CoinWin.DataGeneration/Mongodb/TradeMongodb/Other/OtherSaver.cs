using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    public class OtherSaver
    {
        #region  废弃
        ///// <summary>
        ///// 保存大单
        ///// </summary>
        ///// <param name="start"></param>
        //public void SaveMaxOrder(DateTime start)
        //{


        //    DateTime st = start;
        //    DateTime end = DateTime.Now.AddDays(-1);
        //    var list = TimeCore.GetDate(st, end);
        //    string table = CommandEnum.RedisKey.MaxOrder + st.ToString("yyyy");
        //    foreach (var item in list)
        //    {
        //        List<MaxOrders> maxlist = new List<MaxOrders>();
        //        var tablename = CommandEnum.RedisKey.MaxOrder + item.ToString("yyyy-MM-dd");
        //        CRDataOut geter = new CRDataOut();
        //        maxlist = geter.GetDataObject<MaxOrders>(tablename);
        //        MongoDbHelper<MaxOrders> max = new MongoDbHelper<MaxOrders>(CommandEnum.RedisKey.MaxOrder, table);
        //        if (maxlist != null && maxlist.Count() > 0)
        //        {      
        //            max.InsertBatch(maxlist);
        //        }             
        //    }
        //}

        ///// <summary>
        ///// 保存爆仓单
        ///// </summary>
        ///// <param name="start"></param>
        //public void SaveLqOrder(DateTime start)
        //{
        //    DateTime st = start;
        //    DateTime end = DateTime.Now;
        //    //DateTime end = DateTime.Now.AddDays(-1);
        //    var list = TimeCore.GetDate(st, end);
        //    string table = CommandEnum.RedisKey.liquidation + st.ToString("yyyy");
        //    foreach (var item in list)
        //    {
        //        List<Liquidations> maxlist = new List<Liquidations>();
        //        var tablename = CommandEnum.RedisKey.liquidation + item.ToString("yyyy-MM-dd");
        //        CRDataOut geter = new CRDataOut();
        //        maxlist = geter.GetDataObject<Liquidations>(tablename);
        //        MongoDbHelper<Liquidations> max = new MongoDbHelper<Liquidations>(CommandEnum.RedisKey.liquidation, table);
        //        if (maxlist!=null&&maxlist.Count()>0)
        //        {
        //            max.InsertBatch(maxlist);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 保存爆仓大单
        ///// </summary>
        ///// <param name="start"></param>
        //public void SaveMaxLQOrder(DateTime start)
        //{
        //    DateTime st = start;
        //    DateTime end = DateTime.Now;
        //    var list = TimeCore.GetDate(st, end);
        //    string table = CommandEnum.RedisKey.BIGMaxLQ + st.ToString("yyyy");
        //    foreach (var item in list)
        //    {
        //        List<Liquidations> maxlist = new List<Liquidations>();
        //        var tablename = CommandEnum.RedisKey.BIGMaxLQ + item.ToString("yyyy-MM-dd");
        //        CRDataOut geter = new CRDataOut();
        //        maxlist = geter.GetDataObject<Liquidations>(tablename);
        //        MongoDbHelper<Liquidations> max = new MongoDbHelper<Liquidations>(CommandEnum.RedisKey.BIGMaxLQ, table);
        //        if (maxlist != null && maxlist.Count() > 0)
        //        {
        //            max.InsertBatch(maxlist);
        //        }
        //    }

        //}

        ///// <summary>
        ///// 保存费率
        ///// </summary>
        ///// <param name="start"></param>
        //public void SaveFundingRate(DateTime start)
        //{
        //    DateTime st = start;
        //    DateTime end = DateTime.Now;
        //    var list = TimeCore.GetDate(st, end);
        //    string table = CommandEnum.RedisKey.FundingRate + st.ToString("yyyy");
        //    foreach (var item in list)
        //    {
        //        List<FundRates> maxlist = new List<FundRates>();
        //        var tablename = CommandEnum.RedisKey.FundingRate + item.ToString("yyyy-MM-dd");
        //        CRDataOut geter = new CRDataOut();
        //        maxlist = geter.GetDataObject<FundRates>(tablename);
        //        MongoDbHelper<FundRates> max = new MongoDbHelper<FundRates>(CommandEnum.RedisKey.FundingRate, table);
        //        if (maxlist != null && maxlist.Count() > 0)
        //        {
        //            max.InsertBatch(maxlist);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 保存持仓
        ///// </summary>
        ///// <param name="start"></param>
        //public void SaveOpeninstert(DateTime start)
        //{
        //    DateTime st = start;
        //    DateTime end = DateTime.Now;
        //    var list = TimeCore.GetDate(st, end);
        //    string table = CommandEnum.RedisKey.OpenInterest + st.ToString("yyyy");
        //    foreach (var item in list)
        //    {
        //        List<OpenInterests> maxlist = new List<OpenInterests>();
        //        var tablename = CommandEnum.RedisKey.OpenInterest + item.ToString("yyyy-MM-dd");
        //        CRDataOut geter = new CRDataOut();
        //        maxlist = geter.GetDataObject<OpenInterests>(tablename);
        //        MongoDbHelper<OpenInterests> max = new MongoDbHelper<OpenInterests>(CommandEnum.RedisKey.OpenInterest, table);
        //        if (maxlist != null && maxlist.Count() > 0)
        //        {
        //            max.InsertBatch(maxlist);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 处理 持仓汇总小时
        ///// </summary>
        ///// <param name="start"></param>
        //public void SaveOpeninstertDatahour(DateTime start)
        //{
        //    DateTime st = start;
        //    DateTime end = DateTime.Now;
        //    var list = TimeCore.GetDate(st, end);
        //    string table = CommandEnum.RedisKey.OpenInterestDataHour +st.ToString("yyyy-MM-dd");
        //    foreach (var item in list)
        //    {
        //        List<OpenInsterestDatas> maxlist = new List<OpenInsterestDatas>();
        //        var tablename = CommandEnum.RedisKey.OpenInterestDataHour + item.ToString("yyyy-MM-dd");
        //        CRDataOut geter = new CRDataOut();
        //        maxlist = geter.GetDataObject<OpenInsterestDatas>(tablename);
        //        MongoDbHelper<OpenInsterestDatas> max = new MongoDbHelper<OpenInsterestDatas>(CommandEnum.RedisKey.OpenInterestDataHour, table);
        //        if (maxlist != null && maxlist.Count() > 0)
        //        {
        //            max.InsertBatch(maxlist);
        //        }
        //    }

        //}


        ///// <summary>
        ///// 处理 持仓汇总小时
        ///// </summary>
        ///// <param name="start"></param>
        //public void SaveOpeninstertDataDate(DateTime start)
        //{
        //    DateTime st = start;
        //    DateTime end = DateTime.Now;
        //    var list = TimeCore.GetDate(st, end);
        //    string table = CommandEnum.RedisKey.OpenInterestDataDate + st.ToString("yyyy-MM-dd");
        //    foreach (var item in list)
        //    {
        //        List<OpenInsterestDatas> maxlist = new List<OpenInsterestDatas>();
        //        var tablename = CommandEnum.RedisKey.OpenInterestDataDate + item.ToString("yyyy-MM-dd");
        //        CRDataOut geter = new CRDataOut();
        //        maxlist = geter.GetDataObject<OpenInsterestDatas>(tablename);
        //        MongoDbHelper<OpenInsterestDatas> max = new MongoDbHelper<OpenInsterestDatas>(CommandEnum.RedisKey.OpenInterestDataDate, table);
        //        if (maxlist != null && maxlist.Count() > 0)
        //        {
        //            max.InsertBatch(maxlist);
        //        }
        //    }

        //}
        #endregion


        //按天保存
        public void SaveData<T>(DateTime start, string key) where T : BaseEntity
        {

            //string table = key + start.ToString("yyyy-MM-dd");
            List<T> maxlist = new List<T>();
            var tablename = key + start.ToString("yyyy-MM-dd");
            MongoDbHelper<T> max = new MongoDbHelper<T>(key, tablename);

            if (max.GetRecordCount() > 0)
            {
                Console.WriteLine(tablename + "已经存在数据，不可重复插入！");
                return;
            }



            CRDataOut geter = new CRDataOut();
            maxlist = geter.GetDataObject<T>(tablename);

            if (maxlist != null && maxlist.Count() > 0)
            {
                max.InsertBatch(maxlist);
            }



        }
        //按天保存
        public void SaveDatadit<T>(DateTime start, string key) where T : BaseEntity
        {

            //string table = key + start.ToString("yyyy-MM-dd");
            List<T> maxlist = new List<T>();
            var tablename = key + start.ToString("yyyy-MM-dd");

            MongoDbHelper<T> max = new MongoDbHelper<T>(key, tablename);

            if (max.GetRecordCount() > 0)
            {
                Console.WriteLine(tablename + "已经存在数据，不可重复插入！");
                return;
            }

            maxlist = RedisHelper.GetAllHashList<T>(tablename);

            if (maxlist != null && maxlist.Count() > 0)
            {
                max.InsertBatch(maxlist);
            }
        }
        public void Delted<T>(DateTime start, string key) where T : BaseEntity
        {


            List<T> maxlist = new List<T>();
            var tablename = key + start.ToString("yyyy-MM-dd");
            MongoDbHelper<T> max = new MongoDbHelper<T>(key, tablename);

            if (max.GetRecordCount() > 0)
            {
                //Console.WriteLine(tablename + "已经存在数据，进行删除！");

                bool results = RedisHelper.DeleteKeys(tablename);
                if (results)
                {
                    Console.WriteLine("删除key成功：" + tablename);
                    LogHelper.WriteLog(typeof(DownExchangeData), "删除key成功：" + tablename);
                }
            }
            else
            {
                return;
            }


        }


        public void RunAll(DateTime dt)
        {

            MongoDbHelper<Liquidations> max = new MongoDbHelper<Liquidations>(CommandEnum.RedisKey.liquidation);
            var lists = max.db.ListCollections().ToList();
           var datelist= lists.SelectMany(a=>a.Elements.Where(b=>b.Name=="name").Select(w=>w.Value.AsString)).ToList();   
            var date = datelist.OrderBy(p => p).Last().Replace(CommandEnum.RedisKey.liquidation,"");
            var time = Convert.ToDateTime(date);
            DateTime end = DateTime.Now.AddDays(-1);
            DateTime st = time;
            var list = TimeCore.GetDate(st, end);

            foreach (var item in list)
            {
                
                Console.WriteLine(item.ToString());
                //var item = new DateTime(2021, 04, 14);
                //保存爆仓 item开始日期
                SaveData<Liquidations>(item, CommandEnum.RedisKey.liquidation);
                //保存持仓
                SaveData<OpenInterests>(item, CommandEnum.RedisKey.OpenInterest);
                //保存大单
                SaveData<MaxOrders>(item, CommandEnum.RedisKey.MaxOrder);
                //保存费率
                SaveData<FundRates>(item, CommandEnum.RedisKey.FundingRate);
                //保存爆仓大单
                SaveData<Liquidations>(item, CommandEnum.RedisKey.BIGMaxLQ);
            
                
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

                Delted<Liquidations>(item, CommandEnum.RedisKey.liquidation);
            }


        }

        public void DelteAll(DateTime dt)
        {
            MongoDbHelper<Liquidations> max = new MongoDbHelper<Liquidations>(CommandEnum.RedisKey.liquidation);
            var lists = max.db.ListCollections().ToList();
            var datelist = lists.SelectMany(a => a.Elements.Where(b => b.Name == "name").Select(w => w.Value.AsString)).ToList();
            var date = datelist.OrderBy(p => p).Last().Replace(CommandEnum.RedisKey.liquidation, "");
            var time = Convert.ToDateTime(date);
            DateTime end = DateTime.Now.AddDays(-2);
            DateTime st = time;
            var list = TimeCore.GetDate(st, end);
      

            foreach (var item in list)
            {
                Console.WriteLine(item.ToString());
                //var item = new DateTime(2021, 04, 14);
                //保存爆仓 item开始日期

                Delted<Liquidations>(item, CommandEnum.RedisKey.liquidation);
                //保存持仓
                Delted<OpenInterests>(item, CommandEnum.RedisKey.OpenInterest);
                ////保存大单
                //Delted<MaxOrders>(item, CommandEnum.RedisKey.MaxOrder);
                ////保存爆仓大单
                //Delted<Liquidations>(item, CommandEnum.RedisKey.BIGMaxLQ);



                //保存费率
                Delted<FundRates>(item, CommandEnum.RedisKey.FundingRate);
         
                //永续持仓小时
                Delted<OpenInsterestDatas>(item, CommandEnum.RedisKey.PERP + CommandEnum.RedisKey.OpenInterestDataHour);
                //永续持仓日
                Delted<OpenInsterestDatas>(item, CommandEnum.RedisKey.PERP + CommandEnum.RedisKey.OpenInterestDataDate);
                //永续持仓小时
                Delted<OpenInsterestDatas>(item, CommandEnum.RedisKey.DELIVERY + CommandEnum.RedisKey.OpenInterestDataHour);
                //永续持仓日
                Delted<OpenInsterestDatas>(item, CommandEnum.RedisKey.DELIVERY + CommandEnum.RedisKey.OpenInterestDataDate);
                //爆仓小时汇总
                Delted<LQSummarystatisticss>(item, CommandEnum.RedisKey.liquidationHour);
                //爆仓日汇总
                Delted<LQSummarystatisticss>(item, CommandEnum.RedisKey.liquidationDate);
            }
        }

        public void RunAlltime(DateTime dt)
        {

         
            DateTime end = DateTime.Now.AddDays(-1);
            DateTime st = dt;
            var list = TimeCore.GetDate(st, end);

            foreach (var item in list)
            {
                Console.WriteLine(item.ToString());
                //var item = new DateTime(2021, 04, 14);
                //保存爆仓 item开始日期
                SaveData<Liquidations>(item, CommandEnum.RedisKey.liquidation);           
                SaveData<Liquidations>(item, CommandEnum.RedisKey.BIGMaxLQ);             
            }


        }


    }

    public class CollectionInfo
    { 
    public string name { get; set; }
        public string type { get; set; }
        public object options { get; set; }

        public object info { get; set; }
        public object idIndex { get; set; }
    }

}

//}
////按天保存
//public void SaveDataback<T>(DateTime start, string key) where T : BaseEntity
//{
//    DateTime st = start;
//    DateTime end = DateTime.Now;
//    var list = TimeCore.GetDate(st, end);
//    string table = key + st.ToString("yyyy-MM-dd");
//    foreach (var item in list)
//    {
//        List<T> maxlist = new List<T>();
//        var tablename = key + item.ToString("yyyy-MM-dd");
//        CRDataOut geter = new CRDataOut();
//        maxlist = geter.GetDataObject<T>(tablename);
//        MongoDbHelper<T> max = new MongoDbHelper<T>(key, table);
//        if (maxlist != null && maxlist.Count() > 0)
//        {
//            max.InsertBatch(maxlist);
//        }
//    }

//}

////按年保存
//public void SaveDataYear<T>(DateTime start, string key) where T : BaseEntity
//{
//    DateTime st = start;
//    DateTime end = DateTime.Now;
//    var list = TimeCore.GetDate(st, end);
//    string table = key + st.ToString("yyyy");
//    foreach (var item in list)
//    {
//        List<T> maxlist = new List<T>();
//        var tablename = key + item.ToString("yyyy-MM-dd");
//        CRDataOut geter = new CRDataOut();
//        maxlist = geter.GetDataObject<T>(tablename);
//        MongoDbHelper<T> max = new MongoDbHelper<T>(key, table);
//        if (maxlist != null && maxlist.Count() > 0)
//        {
//            max.InsertBatch(maxlist);
//        }
//    }

//}