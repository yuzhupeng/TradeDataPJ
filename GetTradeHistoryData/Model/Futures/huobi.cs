using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GetTradeHistoryData 
{
  public  class huobi
    {
        /// <summary>
        /// 数量 1张= 0.001大饼
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public long ts { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        [JsonConverter(typeof(DeciamlConverter))]
        public decimal price { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public string direction { get; set; }


        /// <summary>
        /// 实际时间
        /// </summary>
        public DateTime actcualtime { get; set; }

        public string quantity { get; set; }

        public string trade_turnover { get; set; }


          //"amount":120,
          //      "ts":1604386599123,
          //      "id":1138436723890000,
          //      "price":13562.5,
          //      "direction":"sell"

        //"amount":2,
        //       "ts":1603708208335,
        //       "id":1316022650000,
        //       "price":13073.3,
        //       "direction":"buy"
    }
}
