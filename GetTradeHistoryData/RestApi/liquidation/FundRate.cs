using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData 
{
  public  class FundRate
    {
        /// <summary>
        /// 交易对
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 市场
        /// </summary>
        public string market { get; set; }
        /// <summary>
        /// 当前费率
        /// </summary>
        public string FundingRate { get; set; }

        /// <summary>
        /// 下一期费率
        /// </summary>
        public string NextFundingRate { get; set; }


        /// <summary>
        /// 下一期费率
        /// </summary>
        public string NextFundingRateTime { get; set; }


        /// <summary>
        /// 交易所
        /// </summary>
        public string exchange { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }


        /// <summary>
        /// 时间
        /// </summary>
        public string times { get; set; }

 

        //public string kind { get; set; }
    }
}
