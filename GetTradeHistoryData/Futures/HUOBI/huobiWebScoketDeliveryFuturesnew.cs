using log4net;
using Newtonsoft.Json;
using QPP.Core;
using SuperSocket.ClientEngine.Proxy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;


namespace GetTradeHistoryData
{
    public class huobiWebScoketDeliveryFuturesnew : IDisposable
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
        public string ServerPath { get; set; }

        /// <summary>
        /// 重试发出的消息
        /// </summary>
        public string Sendata { get; set; }

        ILog LogHelpers;

        /// <summary>
        /// 交割单号
        /// </summary>
       string symbol;

        /// <summary>
        /// B代号-ETC/BCH/LTC
        /// </summary>
        string coin;
        /// <summary>
        /// 火币U交割合约webscoket
        /// 合约站行情请求以及订阅地址为：wss://api.hbdm.com/ws
        /// 合约站行情请求以及订阅地址为：wss://api.btcgateway.pro/ws
        /// </summary>
        /// <param name="url"></param>
        /// <param name="symbol"></param>
        public huobiWebScoketDeliveryFuturesnew(string url, string symbol)
        {
            this.symbol = symbol;
       
            LogHelpers = LogHelper.CreateInstance();
            if (url != null && url != "")
            {
                ServerPath = url;
            }
            else
            {
                ServerPath = "wss://api.btcgateway.com/ws";
            }
            this._webSocket = new WebSocket(url);
            //this._webSocket.OnOpen += WebSocket_Opened;
            //this._webSocket.OnError += WebSocket_Error;
            //this._webSocket.OnClose += WebSocket_Closed;
            //this._webSocket.OnMessage += WebSocket_MessageReceived;
            //this._webSocket.SetProxy("http://127.0.0.1:1080", "", "");
            this._webSocket.DataReceived += (o, s) => WebSocket_MessageReceived(s);
            this._webSocket.Proxy = new HttpConnectProxy(new DnsEndPoint("127.0.0.1", 1080));


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
                this._webSocket.Open();

                this._isRunning = true;
                this._thread = new Thread(new ThreadStart(CheckConnection));
                this._thread.Start();
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(ex.ToString());
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 消息收到事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WebSocket_MessageReceived(DataReceivedEventArgs e)
        {
            string data = null;
          
             data = GZipDecompresser.Decompress(e.Data);
          
         
            //Console.WriteLine(" Received:" + data);
            //LogHelpers.Info(" Received:" + data);
             Savedata(data);
            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, data, CommandEnum.RedisKey.huobi, CommandEnum.RedisKey.huobi+"交割");
            });

            //MessageReceived?.Invoke(e.Data);
        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("火币季度websocket_Closed" + e.ToString());
            LogHelpers.Info("websocket_Closed");
            //_webSocket.Close();
        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {
            Console.WriteLine("火币季度websocket_Error" + e.ToString());
            LogHelpers.Info("websocket_Error:" + e.ToString());

            //LogHelper.WriteLog(e.Exception.ToString());
        }
        /// <summary>
        /// Socket打开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Opened(object sender, EventArgs e)
        {


            LogHelpers.Info(" websocket_Opened");
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
                    if (this._webSocket.State != WebSocketState.Connecting && this._webSocket.State != WebSocketState.Open)
                    {


                        //LogHelper.WriteLog(" Reconnect websocket WebSocketState:" + this._webSocket.State);
                        this._webSocket.Close();
                        this._webSocket.Open();
                        if (Sendata != null && Sendata != "")
                        {
                            SendMessage(Sendata);
                        }
                        LogHelpers.Info("火币季度正在重连");
                        Console.WriteLine("火币季度正在重连");
                    }
                    else
                    {
                        LogHelpers.Info("火币季度当前状态"+ this._webSocket.State+"-----"+ this._webSocket.State + DateTime.Now.ToString());
                        Console.WriteLine("火币季度当前状态" + this._webSocket.State + "-----" + this._webSocket.State + DateTime.Now.ToString());
                        //sendping("{\"pong\":" + "1" + "}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("火币季度正在重连异常" + ex.ToString());
                    LogHelpers.Error("火币季度重连websocket_Error:" + ex.ToString());

                }
                System.Threading.Thread.Sleep(5000);
            } while (this._isRunning);
        }
        #endregion



        
        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="message"></param>
        public void Savedata(string message)
        {
            if (message != null && message != "")
            {
                try
                {
                    var results = JsonConvert.DeserializeObject<dynamic>(message.ToString());

                    if ((object)results.tick == null)
                    {
                        if ((object)results.ping == null)
                        {
                            Console.WriteLine("火币季度无返回值！！！！" + ((object)results).ToString());
                            return;
                        }
                        Console.WriteLine("火币季度回应ping！！！！" + (object)results.ping);
                        string ping = ((object)results.ping).ToString();
                        sendping("{\"pong\":" + ping + "}");
                        return;
                    }

                    var resultdata = (((object)results.tick.data).ToString()).ToList<huobi>();
                    List<string> maxlist = new List<string>();
                    if (resultdata != null && resultdata.Count > 0)
                    {
                        //var tradeIDlist = resultdata.GroupBy(a => a.cross_seq).ToList();
                        List<UPermanentFutures> list = new List<UPermanentFutures>();
                        string[] arr3 = ((object)results.ch).ToString().Split('.');

                        var pair = arr3[1];


                        foreach (var item in resultdata)
                        {
                            var times = GZipDecompresser.GetTimeFromUnixTimestamps(item.ts.ToString());
                            var key = times.ToString("g");
                            var keys = times.ToString("d");
                            item.actcualtime = times;
                            UPermanentFutures model = new UPermanentFutures();
                            model = Mapping(item, pair);

                            list.Add(model);
               
                            if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
                            {
                                 RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder+ DateTime.Now.ToString("yyyy-MM-dd"));
                                
                            }
                        }

                        RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UDeliveryFutures);
                        Task.Run(() =>
                        {
                            RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.DeliverQueueList, list.ToJson());
                        });
                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("火币季度转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("火币季度转化错误，错误的信息源：" + message);
                    Console.WriteLine("火币季度转化错误，错误信息：" + e.Message.ToString());
                    //throw e;
                }
            }
            else
            {
                LogHelpers.Info("火币季度信息为空，无法保存");
                Console.WriteLine("火币季度信息为空，无法保存");
            }
        }

        /// <summary>
        /// 转化类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(huobi model,string pair)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.id;
            o.exchange = CommandEnum.RedisKey.huobi;
            o.market = pair;
            o.pair = pair;
            o.price = model.price;
            o.qty = model.amount;
            o.Unit = "张";
        
                o.types = "U本位合约:" + pair;

                if (pair.Contains("BTC"))
                {
                    o.vol = ((Convert.ToInt64(model.amount) * 100)).ToString();
                }
                else
                {
                    o.vol = ((Convert.ToInt64(model.amount) * 10)).ToString();
                }

            o.side = model.direction;
            o.times = model.actcualtime.AddHours(8).ToString();
            o.timestamp = model.ts.ToString();
            o.utctime= model.actcualtime.ToString();
            //o.utctime = model.timestamp;
            //o.times = model.actcualtime;
            //o.timestamp = model.trade_time_ms;

            return o;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string Message)
        {

            string[] arr1 = symbol.Split(',');

            foreach (var item in arr1)
            {
                var symbols = $"{item}_CW";
                string topic = $"market.{symbols}.trade.detail";
                string clientId = "id7";
                var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");


                if (_webSocket != null && _webSocket.State == WebSocketState.Open)
                {
                    this._webSocket.Send(op);
                }
            }

            foreach (var item in arr1)
            {
                var symbols = $"{item}_NW";
                string topic = $"market.{symbols}.trade.detail";
                string clientId = "id7";
                var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");


                if (_webSocket != null && _webSocket.State == WebSocketState.Open)
                {
                    this._webSocket.Send(op);
                }
            }
            foreach (var item in arr1)
            {
                var symbols = $"{item}_CQ";
                string topic = $"market.{symbols}.trade.detail";
                string clientId = "id7";
                var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");


                if (_webSocket != null && _webSocket.State == WebSocketState.Open)
                {
                    this._webSocket.Send(op);
                }
            }

            foreach (var item in arr1)
            {
                var symbols = $"{item}_NQ";
                string topic = $"market.{symbols}.trade.detail";
                string clientId = "id7";
                var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");


                if (_webSocket != null && _webSocket.State == WebSocketState.Open)
                {
                    this._webSocket.Send(op);
                }
            }


               //string topic = $"market.{symbols}.trade.detail";
               // string clientId = "id7";
               // var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");



            //if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //{
            //    this._webSocket.Send(Message);
            //}

        }
        public string GetSendData()
        {
            //支持大小写， 交易对,"BTC_CW"表示BTC当周合约，"BTC_NW"表示BTC次周合约，"BTC_CQ"表示BTC当季合约, "BTC_NQ"表示次季度合约"，支持使用合约code来订阅 例如："BTC200918"(当周)，"BTC200925"(次周)，"BTC201225"(季度)，"BTC210326"(次季度)。
            //symbol = "BTC_CQ";
            //string clientId = "id7";
            //string topic = $"market.{symbol}.trade.detail";
            //var  results=($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");

            return "1";
            
        }

        public void sendping(string op)
        {

            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                this._webSocket.Send(op);
            }
        }



        public void Dispose()
        {


            LogHelpers.Error(" 火币季度合约异常DISPOSE：" + DateTime.Now.ToString());
            Console.WriteLine("  火币季度合约异常DISPOSE：" + DateTime.Now.ToString());

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
