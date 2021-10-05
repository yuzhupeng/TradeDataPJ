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
    public class KarkenSpotWSocketClient : IDisposable
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



        public KarkenSpotWSocketClient(string url)
        {
            LogHelpers = LogHelper.CreateInstance();

            ServerPath = url;
            if (url == null || url == "")
            {
                url = "wss://ws.kraken.com";
            }


            this._webSocket = new WebSocket(url);
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

            savedata(e.Data);

            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.Karken, CommandEnum.RedisKey.Karken + "现货");
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
                        //LogHelpers.Info("Karken 现货 当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        Console.WriteLine("Karken 现货 度当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
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

        public void savedata(string message)
        {
            var results = JsonConvert.DeserializeObject<JToken>(message.ToString());
            //var jsonObject = JObject.Load(message);
            var resultJson = results[0];
            if (results[1] == null)
            {
                return;
            }
            var resultJsons = results[1].ToString();

            var pair = results[3].ToString();
            string[] arr4 = pair.Split('/');
            var coin = arr4[1];
            var symbol = arr4[0];


            var datalist = resultJsons.ToList<KrakenTrade>();
            List<UPermanentFutures> list = new List<UPermanentFutures>();
            foreach (var item in datalist)
            {

                var longtimestamp = Math.Round(Convert.ToDecimal(item.Timestamp)*1000, 0);

                var times = GZipDecompresser.GetTimeFromUnixTimestampthree((longtimestamp).ToString());
                var key = times.ToString("g");
                var keys = times.ToString("d");
                item.actcualtime = times;
                UPermanentFutures model = new UPermanentFutures();
                model = Mapping(item, pair, coin, symbol);

                list.Add(model);

                //if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
                //{
                //    RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
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



            //RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.USpot);


           
                RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.SpotQueueList, list.ToJson());
            

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
            else {
                o.contractcode = symbol;
            }

            o.price = model.Price.ToString();
            o.qty = model.Quantity.ToString();
            o.Unit = "币";      
            o.types = " kraken 现货:" + pair;
            o.vol = (Convert.ToDecimal(model.Price) * Convert.ToDecimal(model.Quantity)).ToString();                 
            o.side = model.Side=="s" ? "sell" : "buy";
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

            if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            {
                this._webSocket.Send(Message);
            }

        }

        public string GetSendData()
        {
            //string[] arr;
            //var lists = CommonProcess.GetKrakenAssetPair().Where(p => p.BaseName.Contains("USD")).ToList();
            //List<string> _list = new List<string>();

            //foreach (var item in lists)
            //{
            //    _list.Add(item.PairName);
            //}

            //arr = _list.ToArray();




            string[] names = { "XBT/USD", "BCH/USD", "ETH/USD", "LTC/USD", "XRP/USD", "DOT/USD", "LINK/USD", "UNI/USD" };
            //string[] names = { "XBT/USD", "BCH/USD", "ETH/USD", "LTC/USD", "XRP/USD", "DOT/USD", "LINK/USD", "UNI/USD","XBT/EUR","ETH/EUR"};
            Rootobject ONES = new Rootobject();
            ONES.pair = names;
            ONES.subscription = new Subscription() { name = "trade" };
            Dictionary<string, string> drow = new Dictionary<string, string>();
            drow.Add("event", "subscribe");
            drow.Add("pair", names.ToJson());
            drow.Add("subscription", "{name: 'trade'}");
            return ONES.ToJson();
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
