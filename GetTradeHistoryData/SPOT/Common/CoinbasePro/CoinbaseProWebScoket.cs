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
    public class CoinbaseProWebScoket : IDisposable
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

        string symbol;

        string delivery_date;


        /// <summary>
        /// coinbase
        /// </summary>
        /// <param name="url"></param>
        /// <param name="symbol"></param>
        /// <param name="datecode"></param>
        public CoinbaseProWebScoket(string url)
        {        
            LogHelpers = LogHelper.CreateInstance();
            if (url != null && url != "")
            {
                ServerPath = url;
            }
            else
            {
                ServerPath = "wss://ws-feed.pro.coinbase.com";
            }
            this._webSocket = new WebSocket(ServerPath);
            this._webSocket.OnOpen += WebSocket_Opened;
            this._webSocket.OnError += WebSocket_Error;
            this._webSocket.OnClose += WebSocket_Closed;
            this._webSocket.OnMessage += WebSocket_MessageReceived;
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
                LogHelpers.Info(" coinbase现货启动失败：" + ex.ToString());

                Console.WriteLine(" coinbase现货启动失败：" + ex.ToString());
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
             
            if (e.IsPing)
            {
                Console.WriteLine(" coinbase现货消息ping");
                return;
            }
            //Console.WriteLine(" Received:" + e.Data);
            //LogHelpers.Info(" Received:" + e.Data);
            //MessageReceived?.Invoke(e.Data);
            //Task.Run(() =>
            //{
                Savedata(e.Data);

            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.CoinbasePro, CommandEnum.RedisKey.CoinbasePro+"现货");
            });

        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            LogHelpers.Info(" coinbase现货websocket_Closed：" + e.ToString());

            Console.WriteLine(" coinbase现货websocket_Closed：" + e.ToString());
            //_webSocket.Close();
        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {
           
            LogHelpers.Info(" coinbase现货websocket_Error：" + e.ToString());

            Console.WriteLine(" coinbase现货websocket_Error：" + e.ToString());
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
                    if (this._webSocket.IsAlive != true || this._webSocket.ReadyState != WebSocketState.Open)
                    {


                        //LogHelper.WriteLog(" Reconnect websocket WebSocketState:" + this._webSocket.State);
                        this._webSocket.Close();
                        this._webSocket.Connect();
                        if (Sendata != null && Sendata != "")
                        {
                            SendMessage(Sendata);
                        }
                        LogHelpers.Info("coinbase 正在重连");
                        Console.WriteLine("coinbase 正在重连");
                    }
                    else
                    {
                        LogHelpers.Info("coinbase 现货" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive+DateTime.Now.ToString());
                        Console.WriteLine("coinbase 现货" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("coinbase 现货正在重连异常" + ex.ToString());
                    LogHelpers.Error("coinbase 现货重连websocket_Error:" + ex.ToString());

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
                    //var results = JsonConvert.DeserializeObject<dynamic>(message.ToString());
                    //if (results == null)
                    //{
                    //    Console.WriteLine("coinbase 无返回值！！！！" + ((object)results).ToString());
                    //    return;
                    //}
                    //var resultdata = (((object)results).ToString()).ToList<CoinbaseProSpot>();
                    //List<string> maxlist = new List<string>();
                    //if (resultdata != null && resultdata.Count > 0)
                    //{
                    //    List<UPermanentFutures> list = new List<UPermanentFutures>();
                    //    string markert = resultdata.FirstOrDefault().product_id;
                    //    string[] arr3 = markert.Split('-');

                    //    var pair = arr3[0];//ETH
                    //    var coin = arr3[1];//0326/PERE

                      
                    //     PERPFuture(resultdata, list, pair, markert);                      
                    //}



                    var results = JsonConvert.DeserializeObject<dynamic>(message.ToString());
                    var item = (((object)results).ToString()).ToObject<CoinbaseProSpot>();

                    if (results == null)
                    {
                        Console.WriteLine("coinbase 无返回值！！！！" + ((object)results).ToString());
                        return;
                    }
                    List<UPermanentFutures> list = new List<UPermanentFutures>();
                    if (item != null)
                    {
                        string[] arr3 = item.product_id.Split('-');

                        var pair = arr3[0];//ETH
                        var coin = arr3[1];//0326/PERE
                                           //var times = GZipDecompresser.GetTimeFromUnixTimestamps(item.time.ToString());

                        var keys = item.time.ToString();
                        item.actcualtime = Convert.ToDateTime(item.time);
                        UPermanentFutures model = new UPermanentFutures();
                        model = Mapping(item, pair,coin);
                        list.Add(model);
                        if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
                        {
                            RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
                            Console.WriteLine(model.ToJson());
                        }
                        RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.SpotQueueList, list.ToJson());
                    }






                }
                catch (Exception e)
                {
                    LogHelpers.Error("coinbase 信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("coinbase 信息转化错误，错误的信息源：" + message);
                    Console.WriteLine("coinbase 信息转化错误，错误信息：" + e.Message.ToString());
                    //throw e;
                }
            }
            else
            {
                LogHelpers.Info("信息为空，无法保存");
                Console.WriteLine("信息为空，无法保存");
            }
        }
 
        /// <summary>
        /// 转化类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(CoinbaseProSpot model,string pair,string market)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.trade_id.ToString();
            o.exchange = CommandEnum.RedisKey.CoinbasePro;
            o.market = model.product_id;
            o.pair = model.product_id;
            o.price = model.price.ToString();
            o.qty = model.size.ToString();
            o.side = model.side;
            //o.vol = ((Convert.ToDecimal(model.price)) * Convert.ToDecimal(model.amount)).ToString();
            o.vol = ((Convert.ToDecimal(model.price)) * Convert.ToDecimal(model.size)).ToString();
            o.utctime = model.time.ToString();
            o.times = model.actcualtime.ToString();
            o.timestamp = model.time.ToString(); ;
            o.contractcode = pair;
            o.Unit = "USDT";
            o.types = " Coinbasepro 现货:" + market;
            o.kind = CommandEnum.RedisKey.SPOT;
            return o;
        }






        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string message)
        {
            //{ "type":"subscribe","channels":[{ "name":"matches","product_ids":["BTC-USD"]}]}

            var list = CommandEnum.coinbasepro.GetSymbolList().Where(a => a.quote_currency == "USD"&&a.status== "online").Select(p => p.id).ToList();
            //var op = ($"{{\"type\":\"subscribe\", \"channels\":[\"name\":\"matches\",\"product_ids\":{list.ToJson()}]}}");
            var op = ("{\"type\":\"subscribe\", \"channels\":[{\"name\":\"matches\",\"product_ids\":" + list.ToJson() + "}]}");
            if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            {
                this._webSocket.Send(op);
            }
        }
        public string GetSendData()
        {
            //var list = CommandEnum.Karkens.GetSymbolList().Where(a => a.tradeable == "true").Select(p => p.symbol).ToList();
            //var op = ($"{{\"event\":\"subscribe\", \"product_ids\":{list.ToJson()},\"feed\":\"trade\"}}");       
            return "1";          
        }
        public void Dispose()
        { 
        }
    }
}
