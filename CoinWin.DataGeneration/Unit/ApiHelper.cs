using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CoinWin.DataGeneration
{
    public class ApiHelper
    {

        public static string fileToString(String filePath)
        {
            string strData = "";
            try
            {
                string line;
                // 创建一个 StreamReader 的实例来读取文件 ,using 语句也能关闭 StreamReader
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
                {
                    // 从文件读取并显示行，直到文件的末尾
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        strData = line;
                    }
                }
            }
            catch (Exception e)
            {
                // 向用户显示出错消息
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return strData;
        }


        public static dynamic GetExtbinance(string apiUrl, CookieCollection ticketCookie = null, string token = "", string username = "", string password = "")
        {

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (var client = new HttpClient(handler))
            {
            tryagain:
                try
                {

                    Uri uri = new Uri(apiUrl);
                    var response = client.GetAsync(uri).Result;

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;

                        return result.ToJson();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine("httpclient出错，错误信息:" + e.ToString());
                    goto tryagain;
                }
            }
        }

        /// <summary>
        /// 原始获取api接口的字符串数据
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="ticketCookie"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static dynamic GetExt(string apiUrl, CookieCollection ticketCookie = null, string token = "", string username = "", string password = "")
        {
            HttpClientHandler handler = new HttpClientHandler();
            //handler.UseCookies = true;
            //handler.AllowAutoRedirect = true;
            //handler.ClientCertificateOptions = ClientCertificateOption.Automatic;

            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //附加身份验证信息
            //if (ticketCookie != null)
            //{
            //    var cookie = new CookieContainer();
            //    cookie.Add(ticketCookie);
            //    handler.CookieContainer = cookie;
            //}

            Uri uri = new Uri(apiUrl);
            using (var client = new HttpClient(handler))
            {
                //附加身份验证信息
                if (!string.IsNullOrWhiteSpace(token))
                {

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                {
                    string usernamePassword = username + ":" + password;
                    string ticket = Convert.ToBase64String(Encoding.UTF8.GetBytes(usernamePassword));
                    //client.DefaultRequestHeaders.Authorization("Authorization", "Basic " + ticket);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", ticket);
                }
                //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36");

                //client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");// 
                //client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,fil;q=0.7,zh-TW;q=0.6");// 



                var response = client.GetAsync(uri).Result;
                if (response.IsSuccessStatusCode && response != null)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    return result.ToJson();
                    ////處理result
                    //ApiResult<TResult> resultDto = result.ToObject<ApiResult<TResult>>();
                    //if (resultDto.Success == false)
                    //{
                    //    throw new Exception(resultDto.Message);
                    //}
                    //return resultDto;
                }
                else
                {
                    //沒有權限
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new System.Security.Authentication.AuthenticationException(response.ReasonPhrase);
                    }
                    string responseText = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(responseText);
                }
            }
        }



        public static string PostExt(string apiUrl, object data, CookieCollection ticketCookie = null, string token = "", string username = "", string password = "")
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = true;
            handler.AllowAutoRedirect = false;

            //附加身份認證信息
            if (ticketCookie != null)
            {
                var cookie = new CookieContainer();
                cookie.Add(ticketCookie);
                handler.CookieContainer = cookie;
            }
            Uri uri = new Uri(apiUrl);
            using (var client = new HttpClient(handler))
            {
                //附加身份验证信息
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    string usernamePassword = username + ":" + password;
                    string ticket = Convert.ToBase64String(Encoding.UTF8.GetBytes(usernamePassword));
                    //client.DefaultRequestHeaders.Authorization("Authorization", "Basic " + ticket);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", ticket);
                }


                //*注意*，此處已駝峰方式來序列化提交的數據命名即首字母小寫
                string tempData = data.ToJson();
                HttpContent content = new StringContent(tempData);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = client.PostAsync(uri, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    return result;
                }
                else
                {
                    //沒有權限
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new System.Security.Authentication.AuthenticationException(response.ReasonPhrase);
                    }
                    string responseText = response.Content.ReadAsStringAsync().Result;
                    throw new Exception(responseText);
                }
            }
        }

    }
}
