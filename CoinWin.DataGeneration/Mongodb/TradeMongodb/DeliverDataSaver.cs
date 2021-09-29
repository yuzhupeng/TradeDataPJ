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
   public class DataSaver
    {
        /// <summary>
        ///   .逐个交易所 币本位和U本位数据 天汇总
        ///   .逐个交易所 币本位和U本位数据 小时汇总
        ///   PERPALLCoinALLExchange2021-04-06
        ///   ftx_2021-04-06 08_HOT-PERP
        ///   ftx_2021-04-06_ATOM-PERP
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="key">DeliverALLCoinALLExchange</param>
        private void SaveALLCoinALLExchange(DateTime dt,string key)
        {
            //string key = "DeliverALLCoinALLExchange";

            string tablename = key + dt.ToString("yyyy-MM-dd");

            var hourtradeMasterlist = RedisHelper.GetAllHash<TradeMaters>(tablename);
            if (hourtradeMasterlist.Count == 0)
            {
                return;
            }


            List<TradeMaters> others = new List<TradeMaters>();
            List<TradeMaters> listhour = new List<TradeMaters>();
            List<TradeMaters> listdate = new List<TradeMaters>();
            string timecode = "_" + dt.ToString("yyyy-MM-dd") + "_";
            foreach (var item in hourtradeMasterlist)
            {
                if (item.Key.Contains(dt.ToString("yyyy-MM-dd")))
                {
                    if (item.Key.Contains(timecode))
                    {
                        listdate.Add(item.Value);
                    }
                    else
                    {
                        listhour.Add(item.Value);
                    }
                }
                else
                {
                    others.Add(item.Value);
                }
            }
            string dbs1 = key + "Hour";
            string tablename1 = dbs1 + dt.ToString("yyyy-MM-dd");

            SaveListData(listhour, tablename1, dbs1);


            string dbs2 = key + "Date";
            string tablename2 = dbs2 + dt.ToString("yyyy-MM-dd");
            SaveListData(listdate, tablename2, dbs2);
        }

        /// <summary>
        /// PERPRealDeal --旧数据对
        /// 全网单币小时汇总(包含币本位和U本位)
        /// </summary>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="key"></param>
        private void SaveRealDeal(DateTime dt,string key, List<TradeMaters> list)
        {
            //List<TradeMaters> list = new List<TradeMaters>();
            List<TradeMaters> others = new List<TradeMaters>();
            //string key = "PERPRealDeal";
            string tablename = key +"Hour"+dt.ToString("yyyy-MM-dd");
            list = list.Where(p=>p.Times.ToString("yyyy-MM-dd")== dt.ToString("yyyy-MM-dd")).ToList();
            if (list.Count == 0)
            {
                return;
            }

       

            //foreach (var item in hourtradeMasterlist)
            //{
            //    list.Add(item);

            //    //if (item.Times.ToString("yyyy-MM-dd").Contains(dt.ToString("yyyy-MM-dd")))
            //    //{
            //    //    list.Add(item);
            //    //}
            //    //else
            //    //{
            //    //    others.Add(item);
            //    //}
            //}
            string dbs = key + "Hour";
            SaveListData(list, tablename, dbs);
            //string keydate = "PERP" + "ALLCoinALLExchange" + "Date";
            //string resultdate = keydate + dt.ToString("yyyy-MM-dd");
            //SaveListData(listdate, resultdate, keydate);
        }



        /// <summary>
        ///  PERPRealDeal2021-03-29
        ///    1.全网单币日汇总
        /// </summary>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="key">"PERPRealDeal" + dt.ToString("yyyy-MM-dd");</param>
        private void SaveRealDealDate(DateTime dt,string keys)
        {
            string key = keys + dt.ToString("yyyy-MM-dd");
            string tablename = keys +"Date"+dt.ToString("yyyy-MM-dd");
            var hourtradeMasterlist = RedisHelper.GetAllHash<TradeMaters>(key);
            if (hourtradeMasterlist.Count == 0)
            {
                return;
            }
            List<TradeMaters> list = new List<TradeMaters>();
            List<TradeMaters> others = new List<TradeMaters>();
            foreach (var item in hourtradeMasterlist)
            {
                if (item.Key.Contains(dt.ToString("yyyy-MM-dd")))
                {
                    list.Add(item.Value);
                }
                else
                {
                    others.Add(item.Value);
                }
            }
            string dbs = keys+"Date";
            SaveListData(list, tablename, dbs);
        }

        /// <summary>
        /// 处理全网单币小时汇总
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="keys"></param>
        private void SaveRadlDealHour(DateTime dt, string keys)
        {
            string key = keys + dt.ToString("yyyy-MM-dd");
            string tablename = keys  + dt.ToString("yyyy-MM-dd");
            var hourtradeMasterlist = RedisHelper.GetAllHash<TradeMaters>(key);
            if (hourtradeMasterlist.Count == 0)
            {
                return;
            }
            List<TradeMaters> list = new List<TradeMaters>();
            List<TradeMaters> others = new List<TradeMaters>();
            foreach (var item in hourtradeMasterlist)
            {
                if (item.Key.Contains(dt.ToString("yyyy-MM-dd")))
                {
                    list.Add(item.Value);
                }
                else
                {
                    others.Add(item.Value);
                }
            }
            string dbs = keys;
            SaveListData(list, tablename, dbs);
        }

        /// <summary>
        /// 保存list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="maxlist"></param>
        /// <param name="tablename"></param>
        /// <param name="db"></param>
        private void SaveListData<T>(List<T> maxlist, string tablename, string db) where T : BaseEntity
        {
            MongoDbHelper<T> max = new MongoDbHelper<T>(db, tablename);

            if (max.GetRecordCount() > 0)
            {
                Console.WriteLine("表已经存在：" + tablename+"，无法保存！");
                return;
            }

            if (maxlist != null && maxlist.Count() > 0)
            {
                max.InsertBatch(maxlist);
            }
            Console.WriteLine("保存key完成："+tablename);
        }




        public void Runall()
        {
            MongoDbHelper<TradeMaters> max = new MongoDbHelper<TradeMaters>(CommandEnum.RedisKey.PERPRealDeal+"Date");
            var lists = max.db.ListCollections().ToList();
            var datelist = lists.SelectMany(a => a.Elements.Where(b => b.Name == "name").Select(w => w.Value.AsString)).ToList();
            var date = datelist.OrderBy(p => p).Last().Replace(CommandEnum.RedisKey.PERPRealDeal+ "Date", "");
            var time = Convert.ToDateTime(date);
            DateTime st = time;
            DateTime end = DateTime.Now.AddDays(-1);
            var list = TimeCore.GetDate(st, end);

            //var PERPLIST= RedisHelper.GetAllHash<TradeMaters>("PERPRealDeal").Values.ToList();
            //var DELIVERLIST = RedisHelper.GetAllHash<TradeMaters>("DeliverRealDeal").Values.ToList();
            //var SPOTLIST= RedisHelper.GetAllHash<TradeMaters>("DeliverRealDeal").Values.ToList();
            foreach (var item in list)
            {
                Console.WriteLine("处理日期：" + item.ToString());
                SaveALLCoinALLExchange(item, "DeliverALLCoinALLExchange");
                SaveRadlDealHour(item, "DeliverRealDealHour");
                SaveRealDealDate(item, "DeliverRealDeal");

                SaveALLCoinALLExchange(item, "PERPALLCoinALLExchange");         
                SaveRadlDealHour(item, "PERPRealDealHour");
                SaveRealDealDate(item, "PERPRealDeal");

                SaveALLCoinALLExchange(item, "SPOTALLCoinALLExchange");                
                SaveRadlDealHour(item, "SPOTRealDealHour");
                SaveRealDealDate(item, "SPOTRealDeal");
            }
        }


        public void DeletedAll()
        {
            MongoDbHelper<TradeMaters> max = new MongoDbHelper<TradeMaters>(CommandEnum.RedisKey.PERPRealDeal + "Date");
            var lists = max.db.ListCollections().ToList();
            var datelist = lists.SelectMany(a => a.Elements.Where(b => b.Name == "name").Select(w => w.Value.AsString)).ToList();
            //var date = datelist.OrderBy(p => p).Last().Replace(CommandEnum.RedisKey.PERPRealDeal + "Date", "");
            var date = datelist.OrderBy(p => p).First().Replace(CommandEnum.RedisKey.PERPRealDeal + "Date", "");
            var time = Convert.ToDateTime(date);

            //var time = new DateTime(2021, 04, 11);

            DateTime st = time;
            DateTime end = DateTime.Now.AddDays(-2);
            var list = TimeCore.GetDate(st, end);

          
            foreach (var item in list)
            {
                Console.WriteLine("处理日期：" + item.ToString());
                Delted<TradeMaters>(item, "DeliverALLCoinALLExchange", "DeliverALLCoinALLExchangeDate");
                Delted<TradeMaters>(item, "DeliverRealDealHour", "DeliverRealDealHour");
                Delted<TradeMaters>(item, "DeliverRealDeal", "DeliverRealDealDate");

                Delted<TradeMaters>(item, "PERPALLCoinALLExchange", "PERPALLCoinALLExchangeDate");
                Delted<TradeMaters>(item, "PERPRealDealHour", "PERPRealDealHour");
                Delted<TradeMaters>(item, "PERPRealDeal", "PERPRealDealDate");

                Delted<TradeMaters>(item, "SPOTALLCoinALLExchange", "SPOTALLCoinALLExchangeDate");
                Delted<TradeMaters>(item, "SPOTRealDealHour", "SPOTRealDealHour");
                Delted<TradeMaters>(item, "SPOTRealDeal", "SPOTRealDealDate");
            }
        }
 
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start"></param>
        /// <param name="key"></param>
        /// <param name="mongodbkey"></param>
        public void Delted<T>(DateTime start, string key,string mongodbkey) where T : BaseEntity
        {


            List<T> maxlist = new List<T>();
            var tablename = mongodbkey + start.ToString("yyyy-MM-dd");
            var deletename= key + start.ToString("yyyy-MM-dd");
            MongoDbHelper<T> max = new MongoDbHelper<T>(key, tablename);

            if (max.GetRecordCount() > 0)
            {
                Console.WriteLine(deletename + "已经存在数据，进行删除！"+"数量："+max.GetRecordCount());

                bool results = RedisHelper.DeleteKeys(deletename);
                if (results)
                {
                    Console.WriteLine("删除key成功：" + deletename);
                    LogHelper.WriteLog(typeof(DownExchangeData), "删除key成功：" + deletename);
                }
            }
            else
            {
                return;
            }


        }

        /// <summary>
        /// 改变 Date Hour key名称
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ChangeKeyname(string key)
        {
            return "";
        }

    }
}
//public void Runhour()
//{

//    DateTime st = new DateTime(2021, 04, 16);
//    DateTime end = new DateTime(2021, 04, 21);

//    var list = TimeCore.GetDate(st, end);

//    foreach (var item in list)
//    {
//        Console.WriteLine("处理日期：" + item.ToString());

//        SaveRadlDealHour(item, "DeliverRealDealHour");

//        SaveRadlDealHour(item, "PERPRealDealHour");

//        SaveRadlDealHour(item, "SPOTRealDealHour");

//    }
//}