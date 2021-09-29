using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GetTradeHistoryData
{
   public class BitmexMarket
    {

        private const string bitmexFUTURESUSDTMARKET_DEFAULT_HOST = "https://www.bitmex.com/api/v1";

        private const string exchangeInfo = "/instrument/active";


        /// <summary>
        /// 获取交易对列表和详细信息
        /// </summary>
        /// <returns></returns>
        public static List<Instrument> GetSymbolFutureList()
        {
            string url = bitmexFUTURESUSDTMARKET_DEFAULT_HOST + exchangeInfo;
            List<Instrument> results = null;
            while (true)
            {
                try
                {
                    var list = ApiHelper.GetExtbinance(url);
                    results = ((object)list).ToString().ToList<Instrument>();
                }
                catch (Exception e)
                {
                    Console.WriteLine("获取bitmex数据异常，异常信息：" + e.ToString());
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
            decimal btvalue = 0.00000001m;
            decimal BTCVOL = 60000;
            string messagetype = "Bitmex费率和持仓";
            List<FundRate> FundRatelist = new List<FundRate>();
            List<OpenInterest> OpenInterestlist = new List<OpenInterest>();
            List<Instrument> instrumentslist = new List<Instrument>();
              instrumentslist = GetSymbolFutureList();


            BTCVOL = instrumentslist.Where(a => a.Symbol == "XBTUSD").FirstOrDefault().MarkPrice.Value;
           
            if (instrumentslist.Count > 0)
            {
                foreach (var item in instrumentslist)
                {            
                    if (item.FundingRate != null)
                    {
                        FundRate i = new FundRate();
                        i.market = item.Symbol;
                        i.exchange = CommandEnum.RedisKey.bitmex;
                        i.FundingRate = item.FundingRate.ToString(); ;
                        i.symbol = item.Symbol;
                        if (i.symbol.Contains("XBT"))
                        {
                            i.symbol = i.symbol.Replace("XBT", "BTC");
                        }

                        i.times = DateTime.Now.ToString();
                        i.NextFundingRate = i.NextFundingRate;
                        FundRatelist.Add(i);
                    }

                    OpenInterest o = new OpenInterest();
                    o.market = item.Symbol;
                    o.exchange = CommandEnum.RedisKey.bitmex;
                    o.SumOpenInterest = item.OpenInterest.Value;
                    o.symbol = item.Symbol;
                    o.times = DateTime.Now.ToString();
                    if (o.symbol.Contains("XBT"))
                    {
                        o.symbol = o.symbol.Replace("XBT", "BTC");
                    }
                    o.type = CommandEnum.RedisKey.coin;
                    o.desc = "币";
                    if (item.MarkPrice != null)
                    {
                        o.price = item.MarkPrice.Value;
                    }
                    else
                    {
                        continue;
                    }
                    
             
                    if (item.Typ == "FFCCSX")//交割
                    {
                        o.SumOpenInterestValue = item.OpenValue.Value * btvalue * BTCVOL;  
                        o.volumeUsd24h = item.Turnover24h.Value* btvalue* BTCVOL;
                        if (item.QuoteCurrency == "XBT")
                        {
                            o.coin = o.SumOpenInterestValue / (item.MarkPrice.Value * BTCVOL);
                        }
                        else
                        {
                            o.coin = o.SumOpenInterestValue / (item.MarkPrice.Value );
                        }
                        o.kind = CommandEnum.RedisKey.DELIVERY;
                    }
                    else//永续
                    {
                        o.SumOpenInterestValue = item.OpenValue.Value * btvalue * BTCVOL;
                        o.volumeUsd24h = item.Turnover24h.Value * btvalue * BTCVOL;
                        if (item.MarkPrice != null&& item.MarkPrice!=0)
                        {
                            o.coin = o.SumOpenInterestValue / item.MarkPrice.Value;
                        }
                        o.kind = CommandEnum.RedisKey.PERP;
                    }
                    OpenInterestlist.Add(o);
                }
            }else
            {

                Console.WriteLine("bitmex 持仓和费率数据获取为0：" + CommandEnum.RedisKey.bitmex + DateTime.Now.ToString() + messagetype);

            }

                



            RedisHelper.Pushdata(FundRatelist.ToJson(), "11", CommandEnum.RedisKey.FundingRate + DateTime.Now.ToString("yyyy-MM-dd"));
            RedisHelper.Pushdata(OpenInterestlist.ToJson(), "11", CommandEnum.RedisKey.OpenInterest + DateTime.Now.ToString("yyyy-MM-dd"));

            Console.WriteLine("bitmex 结束持仓和费率数据获取：" + CommandEnum.RedisKey.bitmex + DateTime.Now.ToString()+ messagetype);

        }



    }
}
