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



//install-package WebSocket4Net
//install-package nlog
//install-package nlog.config
namespace GetTradeHistoryData
{
    public class FTXWebScoket : IDisposable
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

        List<Future> list;

        public FTXWebScoket(string url,string symbol,string datecode)
        {
            this.symbol = symbol;
            this.delivery_date = datecode;

            LogHelpers = LogHelper.CreateInstance();

            if (url != null && url != "")
            {
                ServerPath = url;
            }
            else
            {
                ServerPath = "wss://ftx.com/ws/";
            }

            list= CommandEnum.FTx.GetFutureSymbolList().Where(p => p.volumeUsd24h > 10000000).ToList();

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
            Sendata = symbol;
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
                LogHelpers.Info(" FTX永续合约启动失败：" + ex.ToString());

                Console.WriteLine(" FTX永续合约启动失败：" + ex.ToString());
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
                Console.WriteLine(" FTX永续合约消息ping");
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
                CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.ftx, CommandEnum.RedisKey.ftx);
            });

        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            LogHelpers.Info(" FTX永续合约websocket_Closed：" + e.ToString());

            Console.WriteLine(" FTX永续websocket_Closed：" + e.ToString());
            //_webSocket.Close();
        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {
           
            LogHelpers.Info(" FTX永续合约websocket_Error：" + e.ToString());

            Console.WriteLine(" FTX永续合约websocket_Error：" + e.ToString());
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
                        LogHelpers.Info("ftx正在重连");
                        Console.WriteLine("ftx正在重连");
                    }
                    else
                    {
                        LogHelpers.Info("ftx合约" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive+DateTime.Now.ToString());
                        Console.WriteLine("ftx合约" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ftx合约正在重连异常" + ex.ToString());
                    LogHelpers.Error("ftx合约重连websocket_Error:" + ex.ToString());

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
                    if (results.data == null)
                    {
                        Console.WriteLine("ftx无返回值！！！！" + ((object)results).ToString());
                        return;
                    }
                    var resultdata = (((object)results.data).ToString()).ToList<Ftx>();
                    List<string> maxlist = new List<string>();
                    if (resultdata != null && resultdata.Count > 0)
                    {
                        var tradeIDlist = resultdata.GroupBy(a => a.id).ToList();
                        List<UPermanentFutures> list = new List<UPermanentFutures>();
                        string markert = ((object)results.market).ToString();
                        string[] arr3 = markert.Split('-');

                        var pair = arr3[0];//ETH
                        var coin = arr3[1];//0326/PERE
  
                         PERPFuture(resultdata, list, pair, markert);                      
                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("ftx信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("ftx信息转化错误，错误的信息源：" + message);
                    Console.WriteLine("ftx信息转化错误，错误信息：" + e.Message.ToString());
                    //throw e;
                }
            }
            else
            {
                LogHelpers.Info("信息为空，无法保存");
                Console.WriteLine("信息为空，无法保存");
            }
        }

        private void PERPFuture(List<Ftx> resultdata, List<UPermanentFutures> list,string pair,string market)
        {
            List<string> maxlist = new List<string>();
            foreach (var item in resultdata)
            {
                var times = Convert.ToDateTime(item.time);
                var key = times.ToString("g");
                var keys = times.ToString("d");
                item.actcualtime = key;
                UPermanentFutures model = new UPermanentFutures();
                model = Mapping(item, pair, market);

                if (market.Contains("PERP"))
                {
                    model.kind = CommandEnum.RedisKey.PERP;
                }
                else
                {
                    model.kind = CommandEnum.RedisKey.DELIVERY;
                }
                list.Add(model);               
                    //var sizes = resultdata.Where(p => p.time == item.time).ToList().Sum(p => ((Convert.ToDecimal(p.price)) * Convert.ToDecimal(p.size)));

                    //if (sizes >= 1000000 && maxlist.FirstOrDefault(a => a == item.time) == null)
                    //{
                    //    UPermanentFutures models = new UPermanentFutures();
                    //    ObjectUtil.MapTo(model, models);
                    //    models.vol = sizes.ToString();
                    //    models.qty = (sizes / Convert.ToDecimal(models.price)).ToString();
                    //   RedisHelper.Pushdata(models.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
                    //    maxlist.Add(item.time);
                    //Console.WriteLine(models.ToJson());
                    //}
                 

                //if (model.vol != null &&Convert.ToDecimal(model.vol) >= 1000000)
                //{
                //    //item.size = model.vol;
                //    RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder+ DateTime.Now.ToString("yyyy-MM-dd"));
                //}
                if (Convert.ToBoolean(item.liquidation))
                {
                    Liquidation LQ = new Liquidation();
                    ObjectUtil.MapTo(model, LQ);
                    if (market.Contains("PERP"))
                    {
                        LQ.kinds = CommandEnum.RedisKey.PERP; 
                    }
                    else
                    {
                        LQ.kinds = CommandEnum.RedisKey.DELIVERY;
                    }
                    LQ.contractcode = pair;
                    RedisHelper.Pushdata(LQ.ToJson().ToString(), "11", CommandEnum.RedisKey.liquidation + DateTime.Now.ToString("yyyy-MM-dd"));
                    RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.liquidationList, LQ.ToJson());


                    if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
                    {
                     
                        RedisHelper.Pushdata(LQ.ToJson().ToString(), "11", CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                }
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



            if (market.Contains("PERP"))
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

        /// <summary>
        /// 转化类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(Ftx model,string pair,string market)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.id.ToString();
            o.exchange = CommandEnum.RedisKey.ftx;
            o.market = market;
            o.pair = pair;
            o.contractcode = pair;
            o.price = model.price.ToString();
            o.qty = model.size.ToString();
            o.side = model.side;
            o.vol = ((Convert.ToDecimal(model.price)) * Convert.ToDecimal(model.size)).ToString();
            o.utctime = model.actcualtime;
            o.times = model.actcualtime;
            o.timestamp = model.time;
            if (market.Contains("PERP"))
            {
                o.Unit = "币";
                o.types = "永续币本位合约:" + market;
            }
            else
            {
                o.Unit = "币";
                o.types = "交割币本位合约:" + market;
            }

                return o;
        }






        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string message)
        {
 

            //foreach (var item in arr1)
            //{


            //    string topic = "subscribe";

            //    string clientId = "trades";

         
            //    string market = $"{item}-{delivery_date}";


            //    var ops = ($"{{ \"op\": \"{topic}\",\"channel\": \"{clientId}\",\"market\": \"{market}\" }}");

            //    if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //    {
            //        this._webSocket.Send(ops);
            //    }

            //}

            //foreach (var item in arr1)
            //{

            //    string topic = "subscribe";

            //    string clientId = "trades";


            //    string market = $"{item}-PERP";


            //    var ops = ($"{{ \"op\": \"{topic}\",\"channel\": \"{clientId}\",\"market\": \"{market}\" }}");

            //    if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //    {
            //        this._webSocket.Send(ops);
            //    }
            //}

            //var list = CommandEnum.FTx.GetFutureSymbolList().Where(p => p.volumeUsd24h > 10000000);     
            

            foreach (var item in list)
            {
                string topic = "subscribe";
                string clientId = "trades";
                string market = item.Name;
                var ops = ($"{{ \"op\": \"{topic}\",\"channel\": \"{clientId}\",\"market\": \"{market}\" }}");

                if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
                {
                    this._webSocket.Send(ops);
                }
            }



        }
        public string GetSendData()
        {
            string topic = "subscribe";

            string clientId = "trades";

            string market = "BTC-PERP";

            var results = ($"{{ \"op\": \"{topic}\",\"channel\": \"{clientId}\",\"market\": \"{market}\" }}");

            return results;
           
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
            ////this._webSocket.Dispose();
            //this._webSocket = null;
        }
    }
}
