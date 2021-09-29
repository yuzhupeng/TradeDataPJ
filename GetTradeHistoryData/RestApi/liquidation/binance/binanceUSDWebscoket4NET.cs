using log4net;
using Newtonsoft.Json;
using QPP.Core;
using SuperSocket.ClientEngine.Proxy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace GetTradeHistoryData
{
    /// <summary>
    /// 币安   
    /// </summary>
    public class binanceUSDWebscoket4NET : IDisposable
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
        public binanceUSDWebscoket4NET(string url,string code)
        {
            LogHelpers = LogHelper.CreateInstance();

            this.symbol = code;

            if (url != null && url != "")
            {
                ServerPath = url;
            }
            //else
            //{
            //    ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";
            //}

            if (code == "USD")
            {
                ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
            }
            else
            {
                ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
            }


            //string uls = GetSendData(symbol);
            //ServerPath = ServerPath + uls;

            this._webSocket = new WebSocket(ServerPath);
            this._webSocket.Opened += WebSocket_Opened;
            //this._webSocket.Error += (o, s) => Handle(errorHandlers, s.Exception);
            this._webSocket.Closed += WebSocket_Closed;
            this._webSocket.MessageReceived += (o, s) => WebSocket_MessageReceived(s.Message);
            
            //_webSocket.Security 

            //this._webSocket.SetProxy("http://127.0.0.1:1080", "", "");
            //MessageReceived = WebSocket_MessageReceived;
            //this._webSocket.Proxy=new  
            this._webSocket.Proxy =  new HttpConnectProxy(new DnsEndPoint("127.0.0.1", 1080));


        }

        //protected readonly List<Action<Exception>> errorHandlers = new List<Action<Exception>>();
        //public event Action<Exception> OnError
        //{
        //    add => errorHandlers.Add(value);
        //    remove => errorHandlers.Remove(value);
        //}
        //protected void Handle<T>(List<Action<T>> handlers, T data)
        //{
        //    LastActionTime = DateTime.UtcNow;
        //    foreach (var handle in new List<Action<T>>(handlers))
        //        handle?.Invoke(data);
        //}


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
                this._webSocket.Open();
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
        public void WebSocket_MessageReceived(string Message)
        {

            //Console.WriteLine(" Received:" + e.Data);
            //LogHelpers.Info(" Received:" + e.Data);

            //var results = JsonConvert.DeserializeObject<dynamic>(e.Data.ToString());
            //var datas = (((object)results.data).ToString());

            //Console.WriteLine(" data:" + datas);



            var results = JsonConvert.DeserializeObject<dynamic>(Message);

            var op = ((object)results.e).ToString();
            switch (op)
            {
                case "aggTrade":
                    //Savedatas(e.Data);
                    MessageReceived?.Invoke(results);
                    break;

                case "forceOrder":
                    //Console.WriteLine(" data:" + results);
                    SaveforceOrder(results);
                    break;

                default:
                    MessageReceived?.Invoke(results);
                    break;

            }

            //Task.Run(() =>
            //{
            //    CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.binance, this.ServerPath + "币本位");
            //});

            //MessageReceived?.Invoke(e.Data);
        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            if (symbol == "USD")
            {
                //ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
                Console.WriteLine("binance U本位 WebSocket_Closed" + e.ToString());
                LogHelpers.Error("binance U本位 WebSocket_Closed");
            }
            else
            {
                //ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
                Console.WriteLine("binance 币本位 WebSocket_Closed" + e.ToString());
                LogHelpers.Error("binance 币本位 WebSocket_Closed");
            }

     


        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error( ErrorEventArgs e)
        {

            if (symbol == "USD")
            {
                //ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
                Console.WriteLine("binance U本位 websocket_Error" + e.ToString());
                LogHelpers.Error("binance U本位 websocket_Error");
            }
            else
            {
                //ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
                Console.WriteLine("binance 币本位 websocket_Error" + e.ToString());
                LogHelpers.Error("binance 币本位 websocket_Error");
            }


            //LogHelper.WriteLog(e.Exception.ToString());
        }
        /// <summary>
        /// Socket打开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Opened(object sender, EventArgs e)
        {


            if (symbol == "USD")
            {
                //ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
                Console.WriteLine("binance U本位 WebSocket_Opened" + e.ToString());
                LogHelpers.Error("binance U本位 WebSocket_Opened");
            }
            else
            {
                //ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
                Console.WriteLine("binance 币本位 WebSocket_Opened" + e.ToString());
                LogHelpers.Error("binance 币本位 WebSocket_Opened");
            }
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
                    if ( this._webSocket.State != WebSocketState.Open&& this._webSocket.State != WebSocketState.Connecting)
                    {
                        //LogHelper.WriteLog(" Reconnect websocket WebSocketState:" + this._webSocket.State);
                        this._webSocket.Close();
                        this._webSocket.Open();
                        if (Sendata != null && Sendata != "")
                        {
                            SendMessage(Sendata);
                        }
                        LogHelpers.Info("binance 币本位季度和永续 正在重连");
                        Console.WriteLine("binance 币本位季度和永续 正在重连");

                        if (symbol == "USD")
                        {
                            //ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
                            LogHelpers.Info("binance U本位 正在重连");
                            Console.WriteLine("binance U本位 正在重连");
                        }
                        else
                        {
                            //ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
                            LogHelpers.Info("binance 币本位 正在重连");
                            Console.WriteLine("binance 币本位 正在重连");
                        }

                    }
                    else
                    {
                        //LogHelpers.Info("binance 币本位季度和永续 当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        //Console.WriteLine("binance 币本位季度和永续 度当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        if (symbol == "USD")
                        {
                            //ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
                            Console.WriteLine("binance U本位 度当前状态" + this._webSocket.State + "-----"   + DateTime.Now.ToString());
                        }
                        else
                        {
                            //ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
                            Console.WriteLine("binance 币本位 度当前状态" + this._webSocket.State + "-----"   + DateTime.Now.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
       
                    if (symbol == "USD")
                    {
                        //ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
                        Console.WriteLine("binance U本位 重连websocket_Error" + this._webSocket.State + "-----" + DateTime.Now.ToString());
                    }
                    else
                    {
                        //ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
                        Console.WriteLine("binance 币本位 重连websocket_Error" + this._webSocket.State + "-----" + DateTime.Now.ToString());
                    }


                }
                System.Threading.Thread.Sleep(5000);
            } while (this._isRunning);
        }
        #endregion



       
 

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="message"></param>
        public void SaveforceOrder(dynamic results)
        {
            if (results != null)
            {
                try
                {
                    //var results = JsonConvert.DeserializeObject<dynamic>(message.ToString());
                    var item = (((object)results.o).ToString()).ToObject<binanceLQ>();

                    if (item == null)
                    {
                        return;
                    }

                    List<Liquidation> list = new List<Liquidation>();
                    if (item != null)
                    {
                        //var datas = (((object)results.stream).ToString());
                        //string[] arr3 = item.s.Split('_');
                        //var market = arr3[0].Replace("USD", "");
                        var times = GZipDecompresser.GetTimeFromUnixTimestamps(item.T.ToString());
                        var key = times.ToString("g");
                        var keys = times.ToString("d");
                        item.actcualtime = times;
                   

                        Liquidation model = new Liquidation();

                        model = Mapping(item);


                        //Liquidation LQ = new Liquidation();
                        //ObjectUtil.MapTo(model, LQ);

                        list.Add(model);




                        if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
                        {
                            Console.WriteLine(model.ToJson());
                            RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                        }



                        RedisHelper.Pushdata(list.ToJson().ToString(), "11", CommandEnum.RedisKey.liquidation + DateTime.Now.ToString("yyyy-MM-dd"));

                       
                        RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.liquidationList, list.ToJson());
                       
                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("binance 币本位季度和永续 信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("binance 币本位季度和永续 信息转化错误，错误的信息源：" + ((object)results).ToString());
                    Console.WriteLine("binance 币本位季度和永续 信息转化错误，错误信息：" + e.Message.ToString());

                    if (symbol == "USD")
                    {
                        //ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
                  
                        LogHelpers.Error("binance U本位 信息转化错误，错误信息：" + e.Message.ToString());
                        LogHelpers.Error("binance U本位 信息转化错误，错误的信息源：" + ((object)results).ToString());
                        Console.WriteLine("binance U本位 信息转化错误，错误信息：" + e.Message.ToString());
                    }
                    else
                    {
                        //ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
                   
                        LogHelpers.Error("binance 币本位 信息转化错误，错误信息：" + e.Message.ToString());
                        LogHelpers.Error("binance 币本位 信息转化错误，错误的信息源：" + ((object)results).ToString());
                        Console.WriteLine("binance 币本位 信息转化错误，错误信息：" + e.Message.ToString());
                    }
                }


            }
            else
            {
               

                if (symbol == "USD")
                {
                    //ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
                    LogHelpers.Info("binance U本位 信息为空，无法保存");
                    Console.WriteLine("binance U本位 信息为空，无法保存");
                }
                else
                {
                    //ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
                    LogHelpers.Info("binance 币本位 信息为空，无法保存");
                    Console.WriteLine("binance 币本位 信息为空，无法保存");
                }
            }
        }


        /// <summary>
        /// 转化类
        /// </summary>
        /// <param name="model">
        /// 币本位合约，单位为张，BTC 100美元  其他都为10美元
        /// </param>
        /// <returns></returns>
        public Liquidation Mapping(binanceLQ model)
        {
            Liquidation o = new Liquidation();
            //o.cross_seq = model.a;
            o.exchange = CommandEnum.RedisKey.binance;

            if (symbol == "USD")
            {
                o.market = model.s;
                o.Unit = "币";
                o.vol = ((Convert.ToDecimal(model.p)) * Convert.ToDecimal(model.q)).ToString();
                o.types = "U本位合约:" + model.s;
                o.contractcode = model.s.ToUpper().Replace("USDT", "");
                ////ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
                //LogHelpers.Info("binance U本位 信息为空，无法保存");
                //Console.WriteLine("binance U本位 信息为空，无法保存");
            }
            else
            {
                string[] arr3 = model.s.Split('_');
                var market = arr3[0].Replace("USD", "");
                o.market = model.ps;
                o.Unit = "张";
                o.contractcode = market;
                o.types = "币本位合约:" + model.s;
                if (model.s.ToLower().Contains("btc"))
                {
                    o.vol = ((Convert.ToDecimal(100)) * Convert.ToDecimal(model.q)).ToString();
                }
                else
                {
                    o.vol = ((Convert.ToDecimal(10)) * Convert.ToDecimal(model.q)).ToString();
                }
                ////ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
                //LogHelpers.Info("binance 币本位 信息为空，无法保存");
                //Console.WriteLine("binance 币本位 信息为空，无法保存");
            }




            o.pair = model.s;
            o.price = model.p;
            o.qty = model.q;
          
            


            o.side = model.S; 
            o.times = model.actcualtime.AddHours(8).ToString();
            o.timestamp = model.T.ToString();
            o.utctime = model.actcualtime.ToString() ;
      

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

          //  string[] arr1 = symbol.Split(',');
          //int  a = 0;
          //  string symbollist = "";
          //  foreach (var item in arr1)
          //  {
          //      if (a != 0)
          //      {
          //          symbollist = symbollist + "/";
          //      }
          //      //return "{\"op\":\"subscribe\",\"args\":[\"trade.BTCUSD\"]}";
          //        symbollist += $"{item}usdt@aggTrade";
          //      a += 1;
          //  }

             string topic = "subscribe";
            var op = ("{{ \"method\": \"{topic}\",\"params\": [\"!forceOrder@arr\"],\"id\": 1}}");



            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                this._webSocket.Send(op);
            }
        }


        public string GetSendData(string symbols)
        {

            string[] pairs = symbols.Split(',');
            string pairParams = "";
            if (pairs != null)
            {
                if (pairs.Count() > 0)
                {
                    //foreach (var pair in pairs)
                    //{
                    //    pairParams += $"{pair.ToLower()}usd_210326@aggTrade/";
                    //}
                    //foreach (var pair in pairs)
                    //{
                    //    pairParams += $"{pair.ToLower()}usd_210625@aggTrade/";
                    //}
                    //foreach (var pair in pairs)
                    //{
                    //    pairParams += $"{pair.ToLower()}usd_PERP@aggTrade/";
                    //}

                    pairParams += "!forceOrder@arr";
                    //pairParams = pairParams.Remove(pairParams.Length - 1);
                }
            }

            return pairParams;
        }
        public void Dispose()
        {

 


            if (symbol == "USD")
            {
                //ServerPath = "wss://fstream.binance.com/ws/!forceOrder@arr";//U本位
                LogHelpers.Error("  binance U本位 发送异常断开连接异常" + DateTime.Now.ToString());
                LogHelpers.Info("  binance U本位 发送异常断开连接异常" + DateTime.Now.ToString());
                Console.WriteLine("  binance U本位 约发送异常连接异常" + DateTime.Now.ToString());
            }
            else
            {
                //ServerPath = "wss://dstream.binance.com/ws/!forceOrder@arr";//币本位
                LogHelpers.Error("  binance 币本位 发送异常断开连接异常" + DateTime.Now.ToString());
                LogHelpers.Info("  binance 币本位 发送异常断开连接异常" + DateTime.Now.ToString());
                Console.WriteLine("  binance 币本位 约发送异常连接异常" + DateTime.Now.ToString());
            }
        }


    }
}
