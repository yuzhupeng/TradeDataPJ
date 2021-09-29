using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    /// <summary>
    /// bitmex model
    /// </summary>
    public class BitmexLQ
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string orderID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public string side { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public decimal leavesQty { get; set; }

        /// <summary>
        /// 价值
        /// </summary>
        public decimal price { get; set; }


        public string actcualtime { get; set; }

    }
}