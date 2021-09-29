using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration.Core
{
    /// <summary>
    /// 1. coin数据下载基类
    /// </summary>
   public abstract class CoinGetBase
    {

        /// <summary>
        /// 币本位永续
        /// </summary>
        public abstract string BYXApiUrl { get; set; }

        /// <summary>
        /// USDT永续
        /// </summary>
        public abstract string usdtYXApiUrl { get; set; }


       /// <summary>
       /// 币本位交割
       /// </summary>
        public abstract string BDeliveryapiUrl { get; set; }


        /// <summary>
        /// Usdt交割
        /// </summary>
        public abstract string usdtDeliveryapiUrl { get; set; }


        /// <summary>
        /// 货币集合
        /// </summary>
        public List<KeyValuePair<String, String>> CoinList { get; set; }




        /// <summary>
        /// 获取最后一次更新的时间
        /// </summary>
        /// <returns></returns>
        public abstract string GetLastUpdateTime(string key,string table);

        /// <summary>
        /// 保存数据
        /// </summary>
        public abstract void SaveData();

        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void initialize();

        /// <summary>
        /// 更新缓存
        /// </summary>
        public virtual void ChangeCache(string Key,string value) { 
        
        }

        /// <summary>
        /// 根据API地址下载数据
        /// </summary>
        /// <returns></returns>
        public abstract string GetData();

        /// <summary>
        /// 下载数据进行数据换算
        /// </summary>
        public abstract void DataProcess();

        /// <summary>
        /// 处理下载数据时日志记录
        /// </summary>
        /// <param name="logmessage"></param>
        public virtual void AddLogMessage(string logmessage)
        {
            LogHelper.WriteLog(typeof(DownDataBase), logmessage);
        }
       
    }
}
