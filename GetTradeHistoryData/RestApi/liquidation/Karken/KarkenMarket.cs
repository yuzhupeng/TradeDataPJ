using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GetTradeHistoryData
{
    public class KarkenMarket
    {

        private const string KarkenFUTURESUSDTMARKET_DEFAULT_HOST = "https://futures.kraken.com/derivatives";

        private const string exchangeInfo = "/api/v3/tickers";


        /// <summary>
        /// 获取交易对列表和详细信息
        /// </summary>
        /// <returns></returns>
        public static List<KarKenTicket> GetSymbolFutureList()
        {
            string url = KarkenFUTURESUSDTMARKET_DEFAULT_HOST + exchangeInfo;
            List<KarKenTicket> results = null;
            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtbinance(url);
                    results = ((object)list.tickers).ToString().ToList<KarKenTicket>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取Karken数据异常，异常信息：" + e.ToString());
                    Console.WriteLine("再次执行");
                }
                if (results != null)
                {
                    break;
                }
            }
            return results;
        }

        /// <summary>
        /// 根据使用symbol获取费率和持仓
        /// </summary>
        public void GetFundingRateAndOpenInterest()
        {
  
            string messagetype = "Karken费率和持仓";
            List<FundRate> FundRatelist = new List<FundRate>();
            List<OpenInterest> OpenInterestlist = new List<OpenInterest>();
            List<KarKenTicket> instrumentslist = new List<KarKenTicket>();
            instrumentslist = GetSymbolFutureList().Where(p=>p.suspended== "false").ToList();


            //BTCVOL = instrumentslist.Where(a => a.Symbol == "XBTUSD").FirstOrDefault().MarkPrice.Value;

            if (instrumentslist.Count > 0)
            {
                foreach (var item in instrumentslist)
                {
                    if (item.fundingRate != 0)
                    {
                        FundRate i = new FundRate();
                        i.market = item.symbol.ToUpper();
                        i.exchange = CommandEnum.RedisKey.Karken;
                        i.FundingRate = item.fundingRate.ToString();
                        i.symbol = item.symbol.ToUpper();
                        if (i.symbol.Contains("XBT"))
                        {
                            i.symbol = i.symbol.Replace("XBT", "BTC");
                        }
                        i.times = DateTime.Now.ToString();

                        i.NextFundingRate = item.fundingRatePrediction.ToString();
                        FundRatelist.Add(i);
                    }

                    OpenInterest o = new OpenInterest();
                    o.market = item.symbol.ToUpper();
                    o.exchange = CommandEnum.RedisKey.Karken;
                    o.SumOpenInterest = item.openInterest;

                    o.symbol = item.symbol.ToUpper();
                    if (o.symbol.Contains("XBT"))
                    {
                        o.symbol = o.symbol.Replace("XBT", "BTC");
                    }
                    o.times = DateTime.Now.ToString();
                    o.type = CommandEnum.RedisKey.coin;
                    o.desc = "币";
                    o.price = item.markPrice;

                    if (item.tag != "perpetual")//交割
                    {
                        o.kind = CommandEnum.RedisKey.DELIVERY;
                    }
                    else//永续
                    {
                        o.kind = CommandEnum.RedisKey.PERP;
                    }

                    o.SumOpenInterestValue = item.openInterest;
                    o.volumeUsd24h = item.vol24h;
                    o.coin = item.openInterest / item.markPrice;
                    OpenInterestlist.Add(o);
                }
            }
            else
            {
                Console.WriteLine("Karken 持仓和费率数据获取为0：" + CommandEnum.RedisKey.Karken + DateTime.Now.ToString() + messagetype);
            }





            RedisHelper.Pushdata(FundRatelist.ToJson(), "11", CommandEnum.RedisKey.FundingRate + DateTime.Now.ToString("yyyy-MM-dd"));
            RedisHelper.Pushdata(OpenInterestlist.ToJson(), "11", CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd"));

            Console.WriteLine("Karken 结束持仓和费率数据获取：" + CommandEnum.RedisKey.Karken + DateTime.Now.ToString() + messagetype);

        }



    }
}
