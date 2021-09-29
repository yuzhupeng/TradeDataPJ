using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    public class BinanceFuturesSymbol
    {

        /// <summary>
        /// Contract type
        /// </summary>  
        public string ContractType { get; set; }
        /// <summary>
        /// The maintenance margin percent
        /// </summary>
        public decimal MaintMarginPercent { get; set; }
        /// <summary>
        /// The price Precision
        /// </summary>
        public int PricePrecision { get; set; }
        /// <summary>
        /// The quantity precision
        /// </summary>
        public int QuantityPrecision { get; set; }
        /// <summary>
        /// The required margin percent
        /// </summary>
        public decimal RequiredMarginPercent { get; set; }
        /// <summary>
        /// The base asset
        /// </summary>
        public string BaseAsset { get; set; } = "";
        /// <summary>
        /// Margin asset
        /// </summary>
        public string MarginAsset { get; set; } = "";
        /// <summary>
        /// The quote asset
        /// </summary>
        public string QuoteAsset { get; set; } = "";
        /// <summary>
        /// The precision of the base asset
        /// </summary>
        public int BaseAssetPrecision { get; set; }
        /// <summary>
        /// The precision of the quote asset
        /// </summary>

        public int quotePrecision { get; set; }

        /// <summary>
        /// The symbol
        /// </summary>
        public string symbol { get; set; } = "";

    }
}
