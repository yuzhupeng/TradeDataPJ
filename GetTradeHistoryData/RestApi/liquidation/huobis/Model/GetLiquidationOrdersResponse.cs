
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GetTradeHistoryData
{
    public class GetLiquidationOrdersResponse
    {
        public string status { get; set; }

        [JsonProperty("err_code", NullValueHandling = NullValueHandling.Ignore)]
        public string errorCode { get; set; }

        [JsonProperty("err_msg", NullValueHandling = NullValueHandling.Ignore)]
        public string errorMessage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HUOBILQData data { get; set; }

        public long ts { get; set; }

         
    }
    public class HUOBILQData
    {
        public List<Order> orders { get; set; }

        public class Order
        {
            public string symbol { get; set; }

            [JsonProperty("contract_code")]
            public string contractCode { get; set; }

            [JsonProperty("created_at")]
            public long createdAt { get; set; }

            public string direction { get; set; }

            public string offset { get; set; }

            public decimal price { get; set; }

            public decimal volume { get; set; }

            public decimal amount { get; set; }

            public decimal trade_turnover { get; set; }
        }

        [JsonProperty("total_page")]
        public int totalPage { get; set; }

        [JsonProperty("current_page")]
        public int currentPage { get; set; }

        [JsonProperty("total_size")]
        public int totalSize { get; set; }
    }
}

