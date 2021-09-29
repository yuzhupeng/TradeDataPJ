using System;
using System.Collections.Generic;
using System.Linq;
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

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace CoinWin.DataGeneration
{
    public class LogHelper
    {
        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        #region static void WriteLog(Type t, Exception ex)

        public static void WriteLog(Type t, Exception ex)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error("Error", ex);
        }

        public static void WriteLog(  string ex)
        {

            log4net.ILog log = log4net.LogManager.GetLogger("");
            log.Info(ex);
        }

        #endregion

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        #region static void WriteLog(Type t, string msg)

        public static void WriteLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Info(msg);
        }

        #endregion


    }
}
