
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Huobi.SDK.Core.LinearSwap.RESTful.Response.Market
{
    public class GetContractInfoResponse
    {
        public string status { get; set; }

        [JsonProperty("err_code", NullValueHandling = NullValueHandling.Ignore)]
        public string errorCode { get; set; }

        [JsonProperty("err_msg", NullValueHandling = NullValueHandling.Ignore)]
        public string errorMessage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Data> data { get; set; }

        public long ts { get; set; }

        public class Data
        {
            public string symbol { get; set; }

            [JsonProperty("contract_code")]
            public string contractCode { get; set; }

            [JsonProperty("contract_size")]
            public double contractSize { get; set; }

            [JsonProperty("price_tick")]
            public double priceTick { get; set; }

            [JsonProperty("settlement_date")]
            public string settlementDate { get; set; }

            [JsonProperty("create_date")]
            public string createDate { get; set; }

            [JsonProperty("contract_status")]
            public int contractStatus { get; set; }
        }
    }
}
