using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    public class PerpDataSaver
    {
        public void Runall()
        {
            //SavePERPALLCoinALLExchange(new DateTime(2021, 03, 29));


            DateTime st = new DateTime(2021, 03, 22);
            DateTime end = new DateTime(2021, 04, 01);
            var list = TimeCore.GetDate(st, end);
            foreach (var item in list)
            {
                SavePERPALLCoinALLExchange(item);
                SavePERPRealDeal(item);
                SavePERPRealDealDate(item);
            }


        }




        /// <summary>
        ///   .逐个交易所 币本位和U本位数据 天汇总
        ///   .逐个交易所 币本位和U本位数据 小时汇总
        ///   PERPALLCoinALLExchange2021-04-06
        ///   ftx_2021-04-06 08_HOT-PERP
        ///   ftx_2021-04-06_ATOM-PERP
        /// </summary>
        public void SavePERPALLCoinALLExchange(DateTime dt)
        {
            string dbs = "PERP" + "ALLCoinALLExchange";
            string tablename= dbs + dt.ToString("yyyy-MM-dd");

            var hourtradeMasterlist = RedisHelper.GetAllHash<PermanentFuture>(tablename);
            List<PermanentFuture> others = new List<PermanentFuture>();
            List<PermanentFuture> listhour = new List<PermanentFuture>();
            List<PermanentFuture> listdate = new List<PermanentFuture>();
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
            SaveListData(listhour, tablename, dbs);


            string dbs2 = "PERP" + "ALLCoinALLExchange"+"Date";
            string tablename2 = dbs2 + dt.ToString("yyyy-MM-dd");
            SaveListData(listdate, tablename2, dbs2);
        }

        /// <summary>
        /// PERPRealDeal
        /// 全网单币小时汇总(包含币本位和U本位)
        ///   
        /// </summary>
        public void SavePERPRealDeal(DateTime dt)
        {
            string key = "PERPRealDeal";
            string tablename = key + dt.ToString("yyyy");
            var hourtradeMasterlist = RedisHelper.GetAllHash<PermanentFuture>(key);   
            List<PermanentFuture> list = new List<PermanentFuture>();
            List<PermanentFuture> others = new List<PermanentFuture>();
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
        public void SavePERPRealDealDate(DateTime dt)
        {
            string key = "PERPRealDeal" + dt.ToString("yyyy-MM-dd"); 
            string tablename = key + dt.ToString("yyyy");
            var hourtradeMasterlist = RedisHelper.GetAllHash<PermanentFuture>(key);
            List<PermanentFuture> list = new List<PermanentFuture>();
            List<PermanentFuture> others = new List<PermanentFuture>();
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
            string dbs = "PERPRealDealDate";
            SaveListData(list, tablename, dbs);
        }
    


        public void SaveListData <T>(List<T> maxlist, string tablename, string db) where T : BaseEntity
        {
            MongoDbHelper<T> max = new MongoDbHelper<T>(db, tablename);
            if (maxlist != null && maxlist.Count() > 0)
            {
                max.InsertBatch(maxlist);
            }
        }

        public bool DeletedRecord(string key, string filed)
        {
            return true;
        }


    }
}


//public void SaveData<T>(DateTime start, string key) where T : BaseEntity
//{
//    DateTime st = start;
//    DateTime end = DateTime.Now;
//    var list = TimeCore.GetDate(st, end);
//    string table = key;
//    foreach (var item in list)
//    {
//        List<T> maxlist = new List<T>();
//        var tablename = key;
//        CRDataOut geter = new CRDataOut();
//        maxlist = geter.GetDataObject<T>(tablename);
//        MongoDbHelper<T> max = new MongoDbHelper<T>(key, table);
//        if (maxlist != null && maxlist.Count() > 0)
//        {
//            max.InsertBatch(maxlist);
//        }
//    }

//}

