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
    public class bybitWebScoket : IDisposable
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

        string symbol { get; set; }

        List<CommandEnum.BybitSymbol> symbollist;

        /// <summary>
        /// BYBIT 
        /// 反向 wss://stream.bybit.com/realtime  USD  USDT结算
        /// 和
        /// 永续 wss://stream.bybit.com/realtime_public  币本位 usdt数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sybmols"></param>
        public bybitWebScoket(string url, string sybmols)
        {
            symbol = sybmols;
 
            LogHelpers = LogHelper.CreateInstance();

            if (url != null && url != "")
            {
                ServerPath = url;
            }
            else
            {
                ServerPath = url;
            }

            if (this.ServerPath == "wss://stream.bybit.com/realtime")
            {//USD
                symbollist = CommandEnum.BybitData.GetContract_size().Where(p => p.quote_currency == "USD").ToList();
            }
            else
            {//USDT
                symbollist = CommandEnum.BybitData.GetContract_size().Where(p => p.quote_currency == "USDT").ToList() ;
            }
            this._webSocket = new WebSocket(ServerPath);
            this._webSocket.OnOpen += WebSocket_Opened;
            this._webSocket.OnError += WebSocket_Error;
            this._webSocket.OnClose += WebSocket_Closed;
            this._webSocket.OnMessage += WebSocket_MessageReceived;
            this._webSocket.SetProxy("http://127.0.0.1:1080", "", "");
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
                Console.WriteLine("bybit连接失败:" + ex.ToString());
                LogHelpers.Info("bybit连接失败:" + ex.ToString());
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
            //Savedata(e.Data);
            //Task.Run(() =>
            //{
            Savedata(e.Data);

            Task.Run(() =>
            {
                string code = this.ServerPath == "wss://stream.bybit.com/realtime" ? "USD" : "USDT";
                CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.bybit, CommandEnum.RedisKey.bybit+ code);
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

            Console.WriteLine("bybit websocket_Closed:" + e.ToString());
            LogHelpers.Info("bybit websocket_Closed:" + e.ToString());
    
        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {

            Console.WriteLine("bybit websocket_Error:" + e.ToString());
            LogHelpers.Info("bybit websocket_Error:" + e.ToString());
            //LogHelper.WriteLog(e.Exception.ToString());
        }
        /// <summary>
        /// Socket打开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Opened(object sender, EventArgs e)
        {

            Console.WriteLine("bybit websocket_Opened:" + e.ToString());
            LogHelpers.Info("bybit websocket_Opened:" + e.ToString());

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

                        if (this.ServerPath == "wss://stream.bybit.com/realtime")
                        {
                            LogHelpers.Info("bybit USD 正在重连" + DateTime.Now.ToString());
                            Console.WriteLine("bybit USD 正在重连" + DateTime.Now.ToString());

                        }
                        else
                        {
                            LogHelpers.Info("bybit USDT 正在重连" + DateTime.Now.ToString());
                            Console.WriteLine("bybit USDT 正在重连" + DateTime.Now.ToString());
                        }
                    }
                    else
                    {
                        if (this.ServerPath == "wss://stream.bybit.com/realtime")
                        {


                            LogHelpers.Info("bybit USD 美元结算当前状态：" + this._webSocket.IsAlive + "--------" + this._webSocket.ReadyState + DateTime.Now.ToString());
                            Console.WriteLine("bybit USD 美元结算当前状态：" + this._webSocket.IsAlive + "--------" + this._webSocket.ReadyState + DateTime.Now.ToString());
                        }
                        else
                        {
                            LogHelpers.Info("bybit 币本位结算当前状态：" + this._webSocket.IsAlive + "--------" + this._webSocket.ReadyState + DateTime.Now.ToString());
                            Console.WriteLine("bybit币本位结算当前状态：" + this._webSocket.IsAlive + "--------" + this._webSocket.ReadyState + DateTime.Now.ToString());
                        }
                    }

                    
                }
                catch (Exception ex)
                {
                    LogHelpers.Error("bybit正在重连发生错误" + ex.Message.ToString() + DateTime.Now.ToString());
                    Console.WriteLine("bybit正在重连发生错误" + ex.Message.ToString() + DateTime.Now.ToString());

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

                    if ((object)results.data == null)
                    {
                        LogHelpers.Info("bybit信息为空，无法转换");
                        Console.WriteLine("bybit信息为空，无法转换");
                        Console.WriteLine("bybit 信息转化错误，错误的信息源：" + message);
                        return;
                    }

                     var resultdata = (((object)results.data).ToString()).ToList<Bybit>();
                    List<string> maxlist = new List<string>();
                    if (resultdata != null && resultdata.Count > 0)
                    {
                        string types = CommandEnum.RedisKey.PERP;
                        string[] arr3 = ((object)results.topic).ToString().Split('.');//trade.BTCUSDM21 trade.BTCUSD

                        var pair = arr3[1];
                        var COIN = String.Empty;

                        if (pair.Contains("21"))
                        {
                            if (pair.Contains("U21"))
                            {
                                pair = pair.Replace("U21", "");
                            }
                            else
                            {
                                pair = pair.Replace("M21", "");
                            }
                            types = CommandEnum.RedisKey.DELIVERY;
                        }


                       

                        if (pair.Contains("USDT"))
                        {
                            COIN = pair.Replace("USDT", "");
                        }
                        else
                        {
                            COIN = pair.Replace("USD", "");
                        }


                        var tradeIDlist = resultdata.GroupBy(a => a.cross_seq).ToList();
                        List<UPermanentFutures> list = new List<UPermanentFutures>();

                        foreach (var item in resultdata)
                        {
                            var times = Convert.ToDateTime(item.timestamp);
                            var key = times.ToString("g");
                            var keys = times.ToString("d");
                            item.actcualtime = key;
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

                            //UPermanentFutures oneS = new UPermanentFutures();
                            //ObjectUtil.MapTo(model, oneS);
                     
                            //if (pair.Contains("USDT"))//根据时间戳相同
                            //{
                            //    var sizes = resultdata.Where(p => p.timestamp == item.timestamp).ToList().Sum(p => ((Convert.ToDecimal(p.price)) * Convert.ToDecimal(p.size)));

                            //    if (sizes != null && sizes >= 1000000 && maxlist.FirstOrDefault(a => a == item.timestamp) == null)
                            //    {
                            //        //LogHelpers.Error("bybit USDT大单：" + resultdata.ToJson());

                            //        UPermanentFutures one = new UPermanentFutures();
                            //        ObjectUtil.MapTo(model, one);
                            //        one.vol = sizes.ToString();
                            //        one.qty = (sizes / Convert.ToDecimal(one.price)).ToString();
                            //        RedisHelper.Pushdata(one.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
                            //        maxlist.Add(item.timestamp);
                            //        Console.WriteLine(one.ToJson());
                            //    }
                            //}
                            //else//根据聚合ID
                            //{

                            //    var sizes = resultdata.Where(p => p.cross_seq == item.cross_seq).ToList().Sum(p => Convert.ToDecimal(p.size));
                            //    if (sizes != null && sizes >= 1000000 && maxlist.FirstOrDefault(a => a == item.cross_seq) == null)
                            //    {
                            //        //LogHelpers.Error("bybit USD大单：" + resultdata.ToJson());

                            //        UPermanentFutures one = new UPermanentFutures();
                            //        ObjectUtil.MapTo(model, one);
                            //        one.vol = sizes.ToString();
                            //        one.qty = (sizes / Convert.ToDecimal(one.price)).ToString();
                            //        RedisHelper.Pushdata(one.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
                            //        maxlist.Add(item.cross_seq);
                            //        Console.WriteLine(one.ToJson());
                            //    }
                            //}
                        }

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

                }
                catch (Exception e)
                {
                    LogHelpers.Error("bybit信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("bybit 信息转化错误，错误的信息源：" + message);
                    Console.WriteLine("bybit信息转化错误，错误信息：" + e.Message.ToString());
                    //throw e;
                }
            }
            else
            {
                LogHelpers.Info("bybit信息为空，无法保存");
                Console.WriteLine("bybit信息为空，无法保存");
            }
        }

        /// <summary>
        /// 转化类
        /// BTC-USDT:使用币结算
        /// BTC-USD:使用USDT结算
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(Bybit model, string pair, string coin)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.cross_seq;
            o.exchange = CommandEnum.RedisKey.bybit;
            o.market = model.symbol;
            o.pair = model.symbol;
            o.price = model.price;
            
            o.side = model.side;
            if (pair.Contains("USDT"))
            {
                o.Unit = coin;
                o.types = "币本位结算合约:" + pair;
                o.vol = ((Convert.ToDecimal(model.price)) * Convert.ToDecimal(model.size)).ToString();
                o.qty = model.size.ToString();
            }
            else
            {
                o.Unit = "USDT";
                o.types = "U本位结算合约:" + pair;
                o.vol = model.size.ToString();
                o.qty = ((Convert.ToDecimal(model.size)) / Convert.ToDecimal(model.price)).ToString();
            }
            o.utctime = model.timestamp;
            o.times = model.actcualtime;
            o.timestamp = model.trade_time_ms;
            o.contractcode = coin;
            
            return o;
        }

        public void sendping()
        {

            var results = ("{ \"op\": \"ping\" }");
            if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            {
                this._webSocket.Send(results);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string message)
        {

            //if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //{
            //    this._webSocket.Send(Message);
            //}
            //string[] arr1 = symbol.Split(',');

            foreach (var item in symbollist)
            {
                //var symbols = $"{item}_CQ";
                //string topic = $"market.{symbols}.trade.detail";
                //string clientId = "id7";
                //var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");


                var ops= $"{{\"op\":\"subscribe\",\"args\":[\"trade.{item.name}\"]}}";



                if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
                {
                    this._webSocket.Send(ops);
                }
            }


        }
        public string GetSendData()
        {
            //      return "{\"op\":\"subscribe\",\"args\":[\"trade.BTCUSD\"]}";
            return "{\"op\":\"subscribe\",\"args\":[\"trade.BTCUSD\"]}";
        }
        public void Dispose()
        {
            LogHelpers.Error(" bybit 反向and永续合约异常：" + DateTime.Now.ToString()); 
            Console.WriteLine(" bybit 反向and永续合约异常：" + DateTime.Now.ToString());
            //this._isRunning = false;
            //try
            //{
            //    //_thread.Abort();
            //}
            //catch(Exception ex)
            //{
            //    LogHelpers.Error(" Okex USD永续合约启动失败：" + ex.ToString());
            //    Console.WriteLine(" Okex USD永续合约启动失败：" + ex.ToString());
            //}
            //this._webSocket.Close();
            ////this._webSocket.Dispose();
            //this._webSocket = null;
        }
    }
}
