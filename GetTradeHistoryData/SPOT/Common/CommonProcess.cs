using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GetTradeHistoryData
{
   public class CommonProcess
    {
        public static IEnumerable<BinanceSymbol> GetBinanceSymbol()
        {
            //var request = (HttpWebRequest)WebRequest.Create("https://api.binance.com/api/v3/exchangeInfo");



            string url = string.Format("https://api.binance.com/api/v3/exchangeInfo");
            var list = ApiHelper.GetExt(url);
            var results = ((object)list).ToString().ToObject<ExchangeInfo>();
            return results.Symbols;


            //using (var response = (HttpWebResponse)request.GetResponse())
            //using (var stream = response.GetResponseStream())

            //using (var reader = new StreamReader(stream))
            //{
            //    var exchangeInfo = JsonConvert.DeserializeObject<ExchangeInfo>(reader.ReadToEnd());

            //    return exchangeInfo.Symbols;
            //    //foreach (var symbol in exchangeInfo.Symbols.OrderBy(x => x.Name))
            //    //{
            //    //    if (!symbol.IsSpotTradingAllowed && !symbol.IsMarginTradingAllowed)
            //    //    {
            //    //        // exclude derivatives
            //    //        continue;
            //    //    }
            //    //    var priceFilter = symbol.Filters
            //    //        .First(f => f.GetValue("filterType").ToString() == "PRICE_FILTER")
            //    //        .GetValue("tickSize");

            //    //    var lotFilter = symbol.Filters
            //    //        .First(f => f.GetValue("filterType").ToString() == "LOT_SIZE")
            //    //        .GetValue("stepSize");

            //    //    yield return
            //    //  yield return $"binance,{symbol.Name},crypto,{symbol.Name},{symbol.QuoteAsset},1,{priceFilter},{lotFilter},{symbol.Name}";
            //    //}
            //}


        }




        public static IEnumerable<KrakenAssetPair> GetKrakenAssetPair()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.kraken.com/0/public/AssetPairs");

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())

            using (var reader = new StreamReader(stream))
            {               
                return JsonConvert.DeserializeObject<KrakenAssetPair[]>(reader.ReadToEnd(), new KrakenGetAssetPairsJsonConverter());
            }
        }

        public static List<huobiSymbol> GetHuobiSymbol()
        {
            string url = string.Format("https://api.huobi.pro/market/tickers");
            var list = ApiHelper.GetExt(url);
            var results = ((object)list.data).ToString().ToList<huobiSymbol>();
            return results;
        }


        public static List<huobiticket> GetHuobiSymbolticket()
        {
            string url = string.Format("https://api.huobi.pro/v1/common/symbols");
            var list = ApiHelper.GetExt(url);
            var results = ((object)list.data).ToString().ToList<huobiticket>();
            return results;
        }


        public static List<OkexTicket> GetOKEXSymbolticket()
        {
            string url = string.Format("https://www.okex.com/api/spot/v3/instruments/ticker");
            var list = ApiHelper.GetExt(url);
            var results = ((object)list).ToString().ToList<OkexTicket>();
            return results;
        }


    }
}
