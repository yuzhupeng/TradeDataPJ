using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
   public class MaxData
    {
        public void SaveMaxData(string key)
        { 
          DateTime dt = GetLastTime();
            List<MaxOrder> list = new List<MaxOrder>();
            CRDataOut geter = new CRDataOut();
            var results = geter.GetDataObject(key);

            var targetlist = results.Where(p => Convert.ToDateTime(p.times) > dt);

            foreach (var item in targetlist)
            {
                MaxOrder one = new MaxOrder();
                ObjectUtil.MapTo(item, one);
                list.Add(one);
            }
            if (list.Count() > 0)
            {
                SavePermanentFuturesData(list);
            }
        }


        public List<MaxOrderMail> GetMaxData(string key)
        {
            //DateTime dt = GetLastTime();
            List<MaxOrderMail> list = new List<MaxOrderMail>();
            CRDataOut geter = new CRDataOut();
            var results = geter.GetDataObject<MaxOrderMail>(key);
            if (results.Count() == 0)
            {
                return null;
            }
            //var targetlist = results.Where(p => Convert.ToDateTime(p.times) > dt);

            //foreach (var item in results)
            //{
            //    MaxOrder one = new MaxOrder();
            //    ObjectUtil.MapTo(item, one);
            //    list.Add(one);
            //}
            return results;
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
        public  DateTime GetLastTime()
        {
            DateTime result = new DateTime(1901,01,01);
            try
            {
                string sql = string.Format("select top 1 * from[MaxOrder] order by times desc");

                result =Convert.ToDateTime( SqlDapperHelper.ExecuteReaderReturnList<MaxOrder>(sql).FirstOrDefault().SYS_CreateDate);
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
        public void SavePermanentFuturesData(List<MaxOrder> savelist)
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



    }
}
