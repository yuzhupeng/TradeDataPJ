using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
   public class GetCommonData
    {
        /// <summary>
        /// 获取交易对
        /// </summary>
        /// <param name="TYPE">
        /// SPOT：币币
        //SWAP：永续合约
        //FUTURES：交割合约
        //OPTION：期权
        /// </param>
        /// <returns></returns>
        public static List<instruments> GetSwapContract_size(string TYPE)
        {
            string url = string.Format("https://www.okex.com/api/v5/public/instruments?instType={0}", TYPE);

            var list = ApiHelper.GetExtbinance(url);
            var results = ((object)list.data).ToString().ToList<instruments>();
            return results;
        }
        ///api/v5/market/tickers
        public static List<okexticket> GetTicket(string TYPE)
        {
            string url = string.Format("https://www.okex.com/api/v5/market/tickers?instType={0}", TYPE);

            var list = ApiHelper.GetExtbinance(url);
            var results = ((object)list.data).ToString().ToList<okexticket>();
            return results;
        }

        /// <summary>
        /// 获取持仓
        /// </summary>
        /// <param name="TYPE">
        //SWAP：永续合约
        //FUTURES：交割合约
        //OPTION：期权</param>
        /// <returns></returns>
        public static List<Okexopeninterest> Getopeninterest(string TYPE)
        {
            string url = string.Format("https://www.okex.com/api/v5/public/open-interest?instType={0}", TYPE);
            var list = ApiHelper.GetExtbinance(url);
            var results = ((object)list.data).ToString().ToList<Okexopeninterest>();
            return results;
        }


        /// <summary>
        /// 获取永续合约当前资金费率
        /// </summary>
        /// <param name="TYPE">
 
        /// <returns></returns>
        public static List<okexFundingRate> GetFundingRate(string instId)
        {
            string url = string.Format("https://www.okex.com/api/v5/public/funding-rate?instId={0}", instId);
            var list = ApiHelper.GetExtbinance(url);
            var results = ((object)list.data).ToString().ToList<okexFundingRate>();
            return results;
        }



        /// <summary>
        /// 获取标记价格
        /// </summary>
        /// <param name="TYPE">
        /// <returns></returns>
        public static List<MarkPrice> GetMarkPrice(string instId)
        {
            string url = string.Format("https://www.okex.com/api/v5/public/mark-price?instType={0}", instId);
            var list = ApiHelper.GetExtbinance(url);
            var results = ((object)list.data).ToString().ToList<MarkPrice>();
            return results;
        }
    }
}
