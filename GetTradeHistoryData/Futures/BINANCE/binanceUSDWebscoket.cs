using log4net;
using Newtonsoft.Json;
using QPP.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace GetTradeHistoryData
{
    /// <summary>
    /// 币安   
    /// </summary>
    public class binanceUSDWebscoket : IDisposable
    {
        //日志管理


        #region 向外传递数据事件
        public event Action<string> MessageReceived;



        #endregion


        WebSocket _webSocket;
        /// <summary>
        /// 检查重连线程
        /// </summary>
        Thread _thread;
        bool _isRunning = true;
        /// <summary>
        /// WebSocket连接地址
        /// </summary>
        public string ServerPath
        {
            get;
            set;

        }

        /// <summary>
        /// 重试发出的消息
        /// </summary>
        public string Sendata { get; set; }

        ILog LogHelpers;

        string symbol;

        /// <summary>
        /// 币安 usd币本位合约
        /// </summary>
        /// <param name="url"></param>
        /// <param name="symbol"></param>
        public binanceUSDWebscoket(string url, string symbol)
        {
            LogHelpers = LogHelper.CreateInstance();

            this.symbol = symbol;

            if (url != null && url != "")
            {
                ServerPath = url;
            }
            else
            {
                ServerPath = "wss://dstream.binance.com/stream?streams=";
            }

            string uls = GetSendData(symbol);
            ServerPath = ServerPath + uls;

            this._webSocket = new WebSocket(ServerPath);
            this._webSocket.OnOpen += WebSocket_Opened;
            this._webSocket.OnError += WebSocket_Error;
            this._webSocket.OnClose += WebSocket_Closed;
            this._webSocket.OnMessage += WebSocket_MessageReceived;
            this._webSocket.SetProxy("http://127.0.0.1:1080", "", "");
            //MessageReceived = Savedatas;
        }




        #region "web socket "
        /// <summary>
        /// 连接方法
        /// <returns></returns>
        public bool Start(string senddata)
        {
            Sendata = senddata;
            bool result = true;
            try
            {
                this._webSocket.Connect();
                this._isRunning = true;
                this._thread = new Thread(new ThreadStart(CheckConnection));
                this._thread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("binance 币本位季度和永续 连接失败:" + ex.ToString());
                LogHelpers.Error("binance 币本位季度和永续 连接失败:" + ex.ToString());
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 消息收到事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WebSocket_MessageReceived(object sender, MessageEventArgs e)
        {

            //Console.WriteLine(" Received:" + e.Data);
            //LogHelpers.Info(" Received:" + e.Data);

            //var results = JsonConvert.DeserializeObject<dynamic>(e.Data.ToString());
            //var datas = (((object)results.data).ToString());

            //Console.WriteLine(" data:" + datas);


            Savedatas(e.Data);
            // var results = JsonConvert.DeserializeObject<dynamic>(e.Data.ToString());

            //var op= ((object)results.data.e).ToString();
            // switch (op)
            // {
            //     case "aggTrade":
            //         //Savedatas(e.Data);
            //         MessageReceived?.Invoke(results);
            //         break;

            //     case "forceOrder":
            //         Console.WriteLine(" data:" + results);
            //         break;

            //     default:
            //         MessageReceived?.Invoke(results);
            //         break;

            // }

            //Task.Run(() =>
            //{
            //    CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.binance, this.ServerPath + "币本位");
            //});

            //MessageReceived?.Invoke(e.Data);
            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.binance, CommandEnum.RedisKey.binance + "币本位");
            });
        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("binance 币本位季度和永续binancewebsocket_Closed" + e.ToString());
            LogHelpers.Error("binance 币本位季度和永续binancewebsocket_Closed");
        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {

            Console.WriteLine("binance 币本位季度和永续binance websocket_Error" + e.ToString());
            LogHelpers.Error("binance 币本位季度和永续binance websocket_Error");



            //LogHelper.WriteLog(e.Exception.ToString());
        }
        /// <summary>
        /// Socket打开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Opened(object sender, EventArgs e)
        {
            LogHelpers.Info(" binance 币本位季度和永续websocket_Opened");
        }
        /// <summary>
        /// 检查重连线程
        /// </summary>
        private void CheckConnection()
        {
            do
            {
                try
                {
                    if (this._webSocket.IsAlive != true || this._webSocket.ReadyState != WebSocketState.Open)
                    {
                        //LogHelper.WriteLog(" Reconnect websocket WebSocketState:" + this._webSocket.State);
                        this._webSocket.Close();
                        this._webSocket.Connect();
                        if (Sendata != null && Sendata != "")
                        {
                            SendMessage(Sendata);
                        }
                        LogHelpers.Info("binance 币本位季度和永续 正在重连");
                        Console.WriteLine("binance 币本位季度和永续 正在重连");

                    }
                    else
                    {
                        LogHelpers.Info("binance 币本位季度和永续 当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        Console.WriteLine("binance 币本位季度和永续 度当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("binance 币本位季度和永续 正在重连异常" + ex.ToString());
                    LogHelpers.Error("binance 币本位季度和永续 重连websocket_Error:" + ex.ToString());

                }
                System.Threading.Thread.Sleep(5000);
            } while (this._isRunning);
        }
        #endregion





        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="message"></param>
        public void Savedatas(string message)
        {
            if (message != null && message != "")
            {
                try
                {
                    var results = JsonConvert.DeserializeObject<dynamic>(message.ToString());
                    var item = (((object)results.data).ToString()).ToObject<Binance>();

                    if (item == null)
                    {
                        return;
                    }

                    List<UPermanentFutures> list = new List<UPermanentFutures>();
                    if (item != null)
                    {
                        var datas = (((object)results.stream).ToString());
                        string[] arr3 = item.s.Split('_');
                        var market = arr3[0].Replace("USD", "");
                        var times = GZipDecompresser.GetTimeFromUnixTimestamps(item.E.ToString());
                        //var key = times.ToString("g");
                        //var keys = times.ToString("d");
                        item.actcualtime = times;
                        UPermanentFutures model = new UPermanentFutures();
                        model = Mapping(item, market);
                        list.Add(model);
                        if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
                        {
                            RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
                            Console.WriteLine(model.ToJson());
                        }

                        if (datas.Contains("perp"))
                        {
                            list.FirstOrDefault().kind = CommandEnum.RedisKey.PERP;
                            RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UPermanentFutures);                     
                            RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.PERPQueueList, list.ToJson());
                             
                        }
                        else
                        {
                            list.FirstOrDefault().kind = CommandEnum.RedisKey.DELIVERY;
                            RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UDeliveryFutures);
                         
                            RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.DeliverQueueList, list.ToJson());
                            
                        }
                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("binance 币本位季度和永续 信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("binance 币本位季度和永续 信息转化错误，错误的信息源：" + message.ToJson());
                    Console.WriteLine("binance 币本位季度和永续 信息转化错误，错误信息：" + e.Message.ToString());
                }


            }
            else
            {
                LogHelpers.Info("binance 币本位季度和永续 信息为空，无法保存");
                Console.WriteLine("binance 币本位季度和永续 信息为空，无法保存");
            }
        }


        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="message"></param>
        public void SaveforceOrder(dynamic results)
        {
            if (results != null && results != "")
            {
                try
                {
                    //var results = JsonConvert.DeserializeObject<dynamic>(message.ToString());
                    var item = (((object)results.data).ToString()).ToObject<Binance>();

                    if (item == null)
                    {
                        return;
                    }

                    List<UPermanentFutures> list = new List<UPermanentFutures>();
                    if (item != null)
                    {
                        var datas = (((object)results.stream).ToString());



                        string[] arr3 = item.s.Split('_');

                        var market = arr3[0].Replace("USD", "");


                        var times = GZipDecompresser.GetTimeFromUnixTimestamps(item.E.ToString());
                        var key = times.ToString("g");
                        var keys = times.ToString("d");
                        item.actcualtime = times;
                        item.actcualtime = times;

                        UPermanentFutures model = new UPermanentFutures();

                        model = Mapping(item, market);

                        list.Add(model);

                        //if (model.vol != null && Convert.ToDecimal(model.vol) >= 100000)
                        //{
                        //    RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxLQ + DateTime.Now.ToString("yyyy-MM-dd HH"));
                        //}



                        if (datas.Contains("perp"))
                            RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UPermanentFutures);
                        else
                            RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UDeliveryFutures);
                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("binance 币本位季度和永续 信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("binance 币本位季度和永续 信息转化错误，错误的信息源：" + results.ToJson());
                    Console.WriteLine("binance 币本位季度和永续 信息转化错误，错误信息：" + e.Message.ToString());
                }


            }
            else
            {
                LogHelpers.Info("binance 币本位季度和永续 信息为空，无法保存");
                Console.WriteLine("binance 币本位季度和永续 信息为空，无法保存");
            }
        }


        /// <summary>
        /// 转化类
        /// </summary>
        /// <param name="model">
        /// 币本位合约，单位为张，BTC 100美元  其他都为10美元
        /// </param>
        /// <returns></returns>
        public UPermanentFutures Mapping(Binance model, string type)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.a;
            o.exchange = CommandEnum.RedisKey.binance;
            o.market = model.s;
            o.contractcode = type;
            o.pair = model.s;
            o.price = model.p;

            o.Unit = "张";
            o.types = "币本位合约:" + model.s;
            if (model.s.ToLower().Contains("btc"))
            {
                o.vol = ((Convert.ToDecimal(100)) * Convert.ToDecimal(model.q)).ToString();
            }
            else
            {
                o.vol = ((Convert.ToDecimal(10)) * Convert.ToDecimal(model.q)).ToString();
            }

            o.qty = (Convert.ToDecimal(o.vol) / Convert.ToDecimal(model.p)).ToString();
            o.side = model.m ? "sell" : "buy";
            o.times = model.actcualtime.AddHours(8).ToString();
            o.timestamp = model.T.ToString();
            o.utctime = model.actcualtime.ToString();
            //o.utctime = model.timestamp;
            //o.times = model.actcualtime;
            //o.timestamp = model.trade_time_ms;

            return o;
        }
        //"e": "aggTrade",  // 事件类型
        //"E": 123456789,   // 事件时间
        //"s": "BNBUSDT",    // 交易对
        //"a": "",
        //"p": "0.001",     // 成交价格
        //"q": "100",       // 成交笔数
        //"f": 100,         // 被归集的首个交易ID
        //"l": 105,         // 被归集的末次交易ID
        //"T": 123456785,   // 成交时间
        //"m": true         // 买方是否是做市方。如true，则此次成交是一个主动卖出单，否则是一个主动买入单。


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string Message)
        {

            string[] arr1 = symbol.Split(',');
            int a = 0;
            string symbollist = "";
            foreach (var item in arr1)
            {
                if (a != 0)
                {
                    symbollist = symbollist + "/";
                }
                //return "{\"op\":\"subscribe\",\"args\":[\"trade.BTCUSD\"]}";
                symbollist += $"{item}usdt@aggTrade";
                a += 1;
            }

            string topic = "subscribe";
            var op = ($"{{ \"method\": \"{topic}\",\"params\": [\"{symbollist}\"],\"id\": 1}}");



            //if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //{
            //    this._webSocket.Send(op);
            //}            
        }


        public string GetSendData(string symbols)
        {

            var list = CommandEnum.BinanceMessage.GetSymoblUSDContract_sizeapi();
            string pairParams = "";
            if (list != null)
            {
                if (list.Count() > 0)
                {
                    foreach (var pair in list)
                    {
                        pairParams += $"{pair.symbol.ToLower()}@aggTrade/";
                    }

                    pairParams = pairParams.Remove(pairParams.Length - 1);
                }
            }
            return pairParams;


            //string[] pairs = symbols.Split(',');
            //string pairParams = "";
            //if (pairs != null)
            //{
            //    if (pairs.Count() > 0)
            //    {
            //        foreach (var pair in pairs)
            //        {
            //            pairParams += $"{pair.ToLower()}usd_210326@aggTrade/";
            //        }
            //        foreach (var pair in pairs)
            //        {
            //            pairParams += $"{pair.ToLower()}usd_210625@aggTrade/";
            //        }
            //        foreach (var pair in pairs)
            //        {
            //            pairParams += $"{pair.ToLower()}usd_PERP@aggTrade/";
            //        }

            //        //pairParams += "!forceOrder@arr";
            //        pairParams = pairParams.Remove(pairParams.Length - 1);
            //    }
            //}

            //return pairParams;
        }
        public void Dispose()
        {

            LogHelpers.Error("  binance 币本位季度和永续 发送异常断开连接异常" + DateTime.Now.ToString());
            LogHelpers.Info("  binance 币本位季度和永续 发送异常断开连接异常" + DateTime.Now.ToString());
            Console.WriteLine("  binance 币本位季度和永续 约发送异常连接异常" + DateTime.Now.ToString());
            //this._isRunning = false;
            //try
            //{
            //    _thread.Abort();
            //}
            //catch
            //{

            //}
            //this._webSocket.Close();
            ////this._webSocket.Dispose();
            //this._webSocket = null;
        }


    }
}
