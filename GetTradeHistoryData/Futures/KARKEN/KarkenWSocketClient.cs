using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class KarkenFutureWSocketClient : IDisposable
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



        public KarkenFutureWSocketClient(string url)
        {
            LogHelpers = LogHelper.CreateInstance();

            ServerPath = url;
            if (url == null || url == "")
            {
                url = "wss://futures.kraken.com/ws/v1";
            }


            this._webSocket = new WebSocket(url);
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
                Console.WriteLine(ex.ToString()); ;
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
        public void WebSocket_MessageReceived(object sender, MessageEventArgs e)
        {
            //Console.WriteLine(" Received:" + e.Data + DateTime.Now.ToString());

            Savedata(e.Data);

            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.Karken, CommandEnum.RedisKey.Karken + "期货");
            });
        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
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
                    if (this._webSocket.IsAlive != true && this._webSocket.ReadyState != WebSocketState.Open)
                    {
                        //LogHelper.WriteLog(" Reconnect websocket WebSocketState:" + this._webSocket.State);
                        this._webSocket.Close();
                        this._webSocket.Connect();
                        if (Sendata != null && Sendata != "")
                        {
                            SendMessage(Sendata);
                        }
                        LogHelpers.Info("正在重连");
                        Console.WriteLine("正在重连");
                    }
                    else
                    {
                        sendping();
                        //LogHelpers.Info("Karken 期货 当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        Console.WriteLine("Karken 期货 度当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                    }
                }
                catch (Exception ex)
                {

                }
                System.Threading.Thread.Sleep(5000);
            } while (this._isRunning);
        }
        #endregion

        public void sendping()
        {
            var results = ($"{{ \"event\": \"ping\",\"reqid\": 42 }}");
            if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            {
                this._webSocket.Send(results);
            }
        }

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

                    if ((object)results.feed == null || ((object)results.feed).ToString() != "trade" || (object)results.uid == null)
                    {
                        return;
                        LogHelpers.Info("karken信息为空，无法转换");
                        Console.WriteLine("karken信息为空，无法转换");
                        Console.WriteLine("karken 信息转化错误，错误的信息源：" + message);
                        return;
                    }
                    var item = (((object)results).ToString()).ToObject<Karken>();
                    if (item == null)
                    {
                        return;
                    }
                    string types = CommandEnum.RedisKey.PERP;
                    //string[] arr3 = ((object)results.product_id).ToString().Split('_');//trade.BTCUSDM21 trade.BTCUSD
                    string[] arr3 = item.product_id.ToString().Split('_'); ;

                    var pair = arr3[1];
                    var COIN = arr3[0];

                    if (COIN.ToUpper().Contains("PI"))
                    {
                        types = CommandEnum.RedisKey.PERP;
                    }
                    else
                    {
                        types = CommandEnum.RedisKey.DELIVERY;
                    }

                    if (pair.ToUpper().Contains("USD"))
                    {
                        COIN = pair.Replace("USD", "");
                    }
                    else
                    {
                        COIN = pair.Replace("USD", "");
                    }
                    List<UPermanentFutures> list = new List<UPermanentFutures>();
                    //foreach (var item in resultdata)
                    //{
                    var times = GZipDecompresser.GetTimeFromUnixTimestampthree(item.time.ToString());
                    item.actcualtime = times;
                    UPermanentFutures model = new UPermanentFutures();
                    model = Mapping(item, pair, COIN);
                    if (types == CommandEnum.RedisKey.DELIVERY)
                    {
                        model.kind = CommandEnum.RedisKey.DELIVERY;
                    }
                    else
                    {
                        model.kind = CommandEnum.RedisKey.PERP;
                    }
                    list.Add(model);
                
                    var sizes = list.Sum(p => Convert.ToDecimal(p.vol));
                    if (sizes >= 1000000)
                    {
                        UPermanentFutures models = new UPermanentFutures();
                        ObjectUtil.MapTo(list.FirstOrDefault(), models);
                        models.vol = sizes.ToString();
                        models.qty = (sizes / Convert.ToDecimal(models.price)).ToString();
                        RedisHelper.Pushdata(models.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
                        Console.WriteLine(models.ToJson());
                    }

                    if (types == CommandEnum.RedisKey.PERP)
                    {
                        RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UPermanentFutures);
                        RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.PERPQueueList, list.ToJson());
                    }
                    else
                    {
                        RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UDeliveryFutures);
                        RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.DeliverQueueList, list.ToJson());
                    }


                }
                catch (Exception e)
                {
                    LogHelpers.Error("karken信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("karken 信息转化错误，错误的信息源：" + message);
                    Console.WriteLine("karken信息转化错误，错误信息：" + e.Message.ToString());
                }
            }
            else
            {
                LogHelpers.Info("karken信息为空，无法保存");
                Console.WriteLine("karken信息为空，无法保存");
            }
        }

        /// <summary>
        /// 转化类
        /// BTC-USDT:使用币结算
        /// BTC-USD:使用USDT结算
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(Karken model, string pair, string coin)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.uid;
            o.exchange = CommandEnum.RedisKey.Karken;
            o.market = model.product_id;
            o.pair = model.product_id;
            o.price = model.price.ToString();
            o.side = model.side;
            o.Unit = "USD";
            o.types = "Karken U本位结算合约:" + pair;
            o.vol = model.qty.ToString();
            o.qty = ((Convert.ToDecimal(model.qty)) / Convert.ToDecimal(model.price)).ToString();
            o.utctime = model.time.ToString();
            o.times = model.actcualtime.ToString(); ;
            o.timestamp = model.time.ToString();
            if (coin == "XBT")
            {
                o.contractcode = "BTC";
            }
            else
            {
                o.contractcode = coin;
            }
            return o;
        }





        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(KrakenTrade model, string pair, string coin, string symbol)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = "";
            o.exchange = CommandEnum.RedisKey.Karken;
            o.market = pair;
            o.pair = pair;
            if (symbol == "XBT")
            {
                o.contractcode = CommandEnum.RedisKey.COIN;
            }
            else
            {
                o.contractcode = symbol;
            }

            o.price = model.Price.ToString();
            o.qty = model.Quantity.ToString();
            o.Unit = "币";
            o.types = " kraken 期货:" + pair;
            o.vol = (Convert.ToDecimal(model.Price) * Convert.ToDecimal(model.Quantity)).ToString();
            o.side = model.Side == "s" ? "sell" : "buy";
            o.times = model.actcualtime.ToString();
            o.timestamp = model.Timestamp.ToString();
            o.utctime = model.actcualtime.ToString();
            o.kind = CommandEnum.RedisKey.SPOT;

            return o;
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string Message)
        {
            var list = CommandEnum.Karkens.GetSymbolList().Where(a => a.tradeable == "true").Select(p => p.symbol).ToList();
            var op = ($"{{\"event\":\"subscribe\", \"product_ids\":{list.ToJson()},\"feed\":\"trade\"}}");

            if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            {
                this._webSocket.Send(op);
            }

        }

        public string GetSendData()
        {
            var list = CommandEnum.Karkens.GetSymbolList().Where(a => a.tradeable == "true").Select(p => p.symbol).ToList();
            var op = ($"{{\"event\":\"subscribe\", \"product_ids\":{list.ToJson()},\"feed\":\"trade\"}}");
            return op;

        }
        public void Dispose()
        {
            //this._isRunning = false;
            //try
            //{
            //    _thread.Abort();
            //}
            //catch
            //{

            //}
            //this._webSocket.Close();

            //this._webSocket = null;
        }

    }
}
