using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    public  class FtxMarket
    {
        [JsonProperty("ask")]
        public decimal Ask { get; set; }

        [JsonProperty("baseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonProperty("bid")]
        public decimal Bid { get; set; }

        [JsonProperty("change1h")]
        public decimal Change1H { get; set; }

        [JsonProperty("change24h")]
        public decimal Change24H { get; set; }

        [JsonProperty("changeBod")]
        public decimal ChangeBod { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("last")]
        public decimal? Last { get; set; }

        [JsonProperty("minProvideSize")]
        public decimal MinProvideSize { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("postOnly")]
        public bool PostOnly { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("priceIncrement")]
        public decimal PriceIncrement { get; set; }

        [JsonProperty("quoteCurrency")]
        public string QuoteCurrency { get; set; }

        [JsonProperty("quoteVolume24h")]
        public decimal QuoteVolume24H { get; set; }

        [JsonProperty("restricted")]
        public bool Restricted { get; set; }

        [JsonProperty("sizeIncrement")]
        public decimal SizeIncrement { get; set; }

        [JsonProperty("tokenizedEquity", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TokenizedEquity { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("underlying")]
        public string Underlying { get; set; }

        [JsonProperty("volumeUsd24h")]
        public decimal VolumeUsd24H { get; set; }
    }
}
