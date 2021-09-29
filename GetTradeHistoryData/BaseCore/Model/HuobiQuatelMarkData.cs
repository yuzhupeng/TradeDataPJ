using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GetTradeHistoryData
{
    public class HuobiQuatelMarkData
    {
        /// <summary>
        /// 
        /// </summary>
        public long mrid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<decimal> bid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<decimal> ask { get; set; }
        /// <summary>

        public long ts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ch { get; set; }

        public DateTime times { get; set; }


        public decimal sellcount { get; set; }
        public decimal buycount { get; set; }
        public decimal sellprice { get; set; }
        public decimal buyprice { get; set; }





    }

}
