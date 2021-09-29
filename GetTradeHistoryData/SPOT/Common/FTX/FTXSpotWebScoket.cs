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
    public class FTXSpotWebScoket : IDisposable
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

        List<FtxMarket> marklist;

        /// <summary>
        /// FTX现货
        /// </summary>
        /// <param name="url"></param>
        /// <param name="symbol"></param>
        /// <param name="datecode"></param>
        public FTXSpotWebScoket(string url)
        {
          

            LogHelpers = LogHelper.CreateInstance();
            if (url != null && url != "")
            {
                ServerPath = url;
            }
            else
            {
                ServerPath = "wss://ftx.com/ws/";
            }
            marklist = CommandEnum.FTx.GetSpotSymbolList().Where(p => p.Type == "spot" && p.VolumeUsd24H > 1000000 && (p.QuoteCurrency == "USD" || p.QuoteCurrency == "USDT")).ToList();
            if (marklist == null || marklist.Count == 0)
            {
                throw new Exception("ftx 现货获取交易对出错！！！");
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
            Sendata = "1";
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
                LogHelpers.Info(" FTX现货启动失败：" + ex.ToString());
                Console.WriteLine(" FTX现货启动失败：" + ex.ToString());
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
                Console.WriteLine(" FTX现货消息ping");
                return;
            }
 
            Savedata(e.Data);

            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.ftx, CommandEnum.RedisKey.ftx+"现货");
            });

        }

        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            LogHelpers.Info(" FTX现货websocket_Closed：" + e.ToString());

            Console.WriteLine(" FTX现货websocket_Closed：" + e.ToString());
            //_webSocket.Close();
        }

        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {

            LogHelpers.Info(" FTX现货websocket_Error：" + e.ToString());
            Console.WriteLine(" FTX现货websocket_Error：" + e.ToString());
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
                        LogHelpers.Info("ftx现货" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        Console.WriteLine("ftx现货" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ftx现货正在重连异常" + ex.ToString());
                    LogHelpers.Error("ftx现货重连websocket_Error:" + ex.ToString());

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
 
                        List<UPermanentFutures> list = new List<UPermanentFutures>();
                        string markert = ((object)results.market).ToString();
                        string[] arr3 = markert.Split('/');

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

        private void PERPFuture(List<Ftx> resultdata, List<UPermanentFutures> list, string pair, string market)
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
                list.Add(model);
           
            }

            //RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.USpot);



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

         
             RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.SpotQueueList, list.ToJson());
           


        }

        /// <summary>
        /// 转化类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(Ftx model, string pair, string market)
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
            o.Unit = "币";
            o.types = "FTX 现货:" + market;
            o.kind = CommandEnum.RedisKey.SPOT;
            return o;
        }






        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string message)
        {
            //var list = CommandEnum.FTx.GetFutureSymbolList().Where(p => p.volumeUsd24h > 10000000);
 
            //var list = CommandEnum.FTx.GetSpotSymbolList().Where(p => p.Type == "spot" && p.VolumeUsd24H > 1000000 && (  p.QuoteCurrency == "USD" || p.QuoteCurrency == "USDT"));

            foreach (var item in marklist)
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
 
        }
    }
}
