using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration.Core
{
    /// <summary>
    /// 1.多线程处理数据下载基类
    /// </summary>
   public abstract class DownDataBase
    {

        /// <summary>
        /// Api地址
        /// </summary>
        public abstract string ApiUrl { get; set; }


        /// <summary>
        /// 获取最后一次更新的时间
        /// </summary>
        /// <returns></returns>
        public abstract string GetLastUpdateTime();

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
