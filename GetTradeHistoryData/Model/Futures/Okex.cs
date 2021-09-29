using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData 
{
   public class Okex
    {
        /// <summary>
        /// 	成交id
        /// </summary>
        public string instrument_id { get; set; }
        /// <summary>
        /// 	成交价格
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 	成交方向
        /// </summary>
        public string side { get; set; }
        /// <summary>
        /// 	成交数量
        /// </summary>
        public string qty { get; set; }

        /// <summary>
        /// 	成交数量
        /// </summary>
        public string size { get; set; }

        /// <summary>
        /// 	推送时间
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 代号
        /// </summary>
        public string trade_id { get; set; }

        /// <summary>
        /// 实际时间
        /// </summary>
        public string actcualtime { get; set; }
    }
}


