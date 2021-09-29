using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{

    public class ExchangeInfo
    {
        public BinanceSymbol[] Symbols { get; set; }
    }
    public class BinanceSymbol
    {
        [JsonProperty(PropertyName = "symbol")]
        public string Name { get; set; }

        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }

        public bool IsSpotTradingAllowed { get; set; }
        public bool IsMarginTradingAllowed { get; set; }

        /// <summary>
        /// Exchange info filter defines trading rules on a symbol or an exchange
        /// https://github.com/binance-exchange/binance-official-api-docs/blob/master/rest-api.md#filters
        /// </summary>
        public JObject[] Filters { get; set; }

        public string[] Permissions { get; set; }
    }
}
