using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

/// <summary>
///  日誌處理類
/// </summary>
/// <remarks>
/// ******************************************
/// 創建人：IT_FISH
/// 創建時間：2018/09/25 09:39:43
/// ******************************************
/// </remarks>

namespace GetTradeHistoryData
{
    public class LogHelper
    {
       
        private static ILog _Singleton = null;
        public static ILog CreateInstance()
        {
            if (_Singleton == null)
            {
              
                var log4netRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(log4netRepository, new FileInfo("log4net.config"));

                _Singleton = LogManager.GetLogger(log4netRepository.Name, "NETCorelog4net");

            }
            return _Singleton;
        }

    }
}
