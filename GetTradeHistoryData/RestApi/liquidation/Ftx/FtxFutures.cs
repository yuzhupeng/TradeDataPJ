using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace GetTradeHistoryData
{
    public class Future
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("underlying")]
        public string Underlying { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("expiry")]
        public DateTimeOffset? Expiry { get; set; }

        [JsonProperty("perpetual")]
        public bool Perpetual { get; set; }

        [JsonProperty("expired")]
        public bool Expired { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("postOnly")]
        public bool PostOnly { get; set; }

        [JsonProperty("imfFactor")]
        public double ImfFactor { get; set; }

        [JsonProperty("underlyingDescription")]
        public string UnderlyingDescription { get; set; }

        [JsonProperty("expiryDescription")]
        public string ExpiryDescription { get; set; }

        [JsonProperty("moveStart")]
        public DateTimeOffset? MoveStart { get; set; }

        [JsonProperty("positionLimitWeight")]
        public double PositionLimitWeight { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }


        [JsonProperty("volumeUsd24h")]
        public decimal volumeUsd24h { get; set; }

    }
}
