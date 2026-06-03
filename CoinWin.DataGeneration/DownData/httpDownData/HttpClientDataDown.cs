using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var handlers = new HttpClientHandler();
                handlers.UseCookies = true;
                handlers.AllowAutoRedirect = true;

                HttpClient httpClients = new HttpClient(handlers);

                httpClients.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                httpClients.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
                httpClients.DefaultRequestHeaders.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,fil;q=0.7,zh-TW;q=0.6");

                httpClients.DefaultRequestHeaders.Add("Origin", "https://aggr.trade");
                httpClients.DefaultRequestHeaders.Add("Referer", "https://aggr.trade");
                httpClients.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"92\", \" Not A; Brand\";v=\"99\", \"Google Chrome\";v=\"92\"");
                httpClients.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");
                httpClients.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");

                HttpResponseMessage responses = httpClients.GetAsync(new Uri(urlss)).Result;
                var results = responses.Content.ReadAsStringAsync().Result;

                return results;
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
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var handlers = new HttpClientHandler();
                handlers.UseCookies = true;
                handlers.AllowAutoRedirect = true;

                HttpClient httpClients = new HttpClient(handlers);

                httpClients.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                httpClients.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
                httpClients.DefaultRequestHeaders.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,fil;q=0.7,zh-TW;q=0.6");

                httpClients.DefaultRequestHeaders.Add("Origin", "https://aggr.trade");
                httpClients.DefaultRequestHeaders.Add("Referer", "https://aggr.trade/");
                httpClients.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"92\", \" Not A; Brand\";v=\"99\", \"Google Chrome\";v=\"92\"");
                httpClients.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                httpClients.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");
                httpClients.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");

                HttpResponseMessage responses = httpClients.GetAsync(new Uri(urlss)).Result;
                var results = responses.Content.ReadAsStringAsync().Result;

                List<ResultsItemArry> re = new List<ResultsItemArry>();

                var resut = (results.ToJson() as dynamic);
                if (resut == null || resut.results == null)
                {
                    Console.WriteLine("API返回结果为空或错误: " + (results?.Length > 200 ? results.Substring(0, 200) : results));
                    return re;
                }

                // 直接用索引映射，跳过 ArrayConverter（避免复杂反序列化异常）
                JArray jarr = (JArray)resut.results;
                foreach (var item in jarr)
                {
                    if (item is JArray j && j.Count >= 12)
                    {
                        try
                        {
                            var obj = new ResultsItemArry();
                            obj.time = j[0]?.ToString();
                            obj.count_buy = j[1]?.Value<int>() ?? 0;
                            obj.close = j[2]?.ToString();
                            obj.count_sell = j[3]?.Value<int>() ?? 0;
                            obj.high = j[4]?.Value<double>() ?? 0;
                            obj.liquidation_buy = j[5]?.Value<decimal>() ?? 0;
                            obj.low = j[6]?.Value<decimal>() ?? 0;
                            obj.liquidation_sell = j[7]?.Value<decimal>() ?? 0;
                            obj.exchange = j[8]?.ToString();
                            obj.open = j[9]?.Value<decimal>() ?? 0;
                            obj.vol_buy = j[10]?.Value<decimal>() ?? 0;
                            obj.vol_sell = j[11]?.Value<decimal>() ?? 0;
                            re.Add(obj);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("跳过解析异常行: " + ex.Message);
                        }
                    }
                }

                return re;
            }
            catch (Exception e)
            {
                Console.WriteLine("获取数据出错：" + e.ToString());
                return null;
            }
        }
    }
}
