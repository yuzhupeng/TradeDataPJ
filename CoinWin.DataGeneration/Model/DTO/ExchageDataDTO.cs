using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    /// <summary>
    /// api获取数据DTO
    /// </summary>
    public class ResultsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string close { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("cbuy")]
        public int count_buy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("csell")]
        public int count_sell { get; set; }
        /// <summary>
        /// 
        /// </summ
        [JsonProperty("market")]
        public string exchange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double high { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("lbuy")]
        public decimal  liquidation_buy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("lsell")]
        public decimal liquidation_sell { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal low { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal open { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string pair { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? vol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("vbuy")]
        public decimal vol_buy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("vsell")]
        public decimal vol_sell { get; set; }
    }

    public class ExchageDataDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public string format { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ResultsItem> results { get; set; }
    }

}
