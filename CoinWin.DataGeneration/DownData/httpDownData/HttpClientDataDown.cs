using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace CoinWin.DataGeneration
{
   public class HttpClientDataDown
    {

        public dynamic downdata(string urlss)
        {
            try
            {

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                //string urlss = "https://api.aggr.trade/historical/1629030600000/1629075501101/900000/BITFINEX:BTCUSD+BINANCE:btcusdt+OKEX:BTC-USDT+KRAKEN:XBT/USD+COINBASE:BTC-USD+POLONIEX:USDT_BTC+HUOBI:btcusdt+BITSTAMP:btcusd+BITMEX:XBTUSD+BINANCE_FUTURES:btcusdt+DERIBIT:BTC-PERPETUAL+FTX:BTC-PERP+BYBIT:BTCUSD";
                var handlers = new HttpClientHandler();
                handlers.UseCookies = true;
                //注意：登錄之後不設置跳轉
                handlers.AllowAutoRedirect = true;

                HttpClient httpClients = new HttpClient(handlers);
                //httpClients.


                //httpClients.DefaultRequestHeaders.Add(":authority", "api.aggr.trade");
                //httpClients.DefaultRequestHeaders.Add(":method", "GET");
                //httpClients.DefaultRequestHeaders.Add(":path", urlss);
                //httpClients.DefaultRequestHeaders.Add(":scheme", "https");// 


                httpClients.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                httpClients.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
                httpClients.DefaultRequestHeaders.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,fil;q=0.7,zh-TW;q=0.6");

                httpClients.DefaultRequestHeaders.Add("Origin", "https://aggr.trade");// 
                httpClients.DefaultRequestHeaders.Add("Referer", "https://aggr.trade");// 
                httpClients.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"92\", \" Not A; Brand\";v=\"99\", \"Google Chrome\";v=\"92\"");
                //登录页
                httpClients.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");
                httpClients.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");



                HttpResponseMessage responses = httpClients.GetAsync(new Uri(urlss)).Result;
                var results = responses.Content.ReadAsStringAsync().Result;

                return results;

                //var resultsmid = results.Trim('{');
                //string[] sArray = results.Split('{');





                var resut = (results.ToJson() as dynamic);
                var resultss=((object)resut.results).ToString();

               //var re=((results as dynamic).results).ToString();
                string[] sArray = resultss.Split('{');

                var minstring = sArray[0].ToString();

          

                minstring = minstring.Remove(minstring.LastIndexOf(","), 1);

                //var qwe = (minstring + "]").ToList<ResultsItemArry>();
                //var resultss = ((object)resut.results).ToString().ToList<ResultsItemArry>();



                //var resultslast = resultsmid + "]";

                return minstring+"]";

            }
            catch (Exception e)
            {
                Console.WriteLine("获取数据出错，错误信息：" + e.Message.ToString());
                return "";
            }




        }


        public List<ResultsItemArry> downdatas(string urlss)
        {
            try
            {

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                //string urlss = "https://api.aggr.trade/historical/1629030600000/1629075501101/900000/BITFINEX:BTCUSD+BINANCE:btcusdt+OKEX:BTC-USDT+KRAKEN:XBT/USD+COINBASE:BTC-USD+POLONIEX:USDT_BTC+HUOBI:btcusdt+BITSTAMP:btcusd+BITMEX:XBTUSD+BINANCE_FUTURES:btcusdt+DERIBIT:BTC-PERPETUAL+FTX:BTC-PERP+BYBIT:BTCUSD";
                var handlers = new HttpClientHandler();
                handlers.UseCookies = true;
                //注意：登錄之後不設置跳轉
                handlers.AllowAutoRedirect = true;

                HttpClient httpClients = new HttpClient(handlers);
                //httpClients.


                //httpClients.DefaultRequestHeaders.Add(":authority", "api.aggr.trade");
                //httpClients.DefaultRequestHeaders.Add(":method", "GET");
                //httpClients.DefaultRequestHeaders.Add(":path", urlss);
                //httpClients.DefaultRequestHeaders.Add(":scheme", "https");// 


                httpClients.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                httpClients.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
                httpClients.DefaultRequestHeaders.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,fil;q=0.7,zh-TW;q=0.6");

                httpClients.DefaultRequestHeaders.Add("Origin", "https://aggr.trade");// 
                httpClients.DefaultRequestHeaders.Add("Referer", "https://aggr.trade/");// 
                httpClients.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"92\", \" Not A; Brand\";v=\"99\", \"Google Chrome\";v=\"92\"");
                //登录页
                httpClients.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");
                httpClients.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");



                HttpResponseMessage responses = httpClients.GetAsync(new Uri(urlss)).Result;
                var results = responses.Content.ReadAsStringAsync().Result;

                //var resultsmid = results.Trim('{');
                //string[] sArray = results.Split('{');
                List<ResultsItemArry> re = new List<ResultsItemArry>();




                var resut = (results.ToJson() as dynamic);
                var resultss = ((object)resut.results).ToString();

                //var re=((results as dynamic).results).ToString();
                string[] sArray = resultss.Split('{');

                var minstring = sArray[0].ToString();



                

                if (sArray.Count() > 1)
                {
                    minstring = minstring.Remove(minstring.LastIndexOf(","), 1);
                    re = (minstring + "]").ToList<ResultsItemArry>();
                }
                else
                {
                    re = minstring.ToList<ResultsItemArry>();
                }


                return re;


               // var resultss = ((object)resut.results).ToString().ToList<ResultsItemArry>();


            }
            catch (Exception e)
            {
                Console.WriteLine("获取数据出错，错误信息：" + e.Message.ToString());
                return null;
            }




        }
    }
}


