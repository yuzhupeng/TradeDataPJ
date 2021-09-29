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
    public class OkexWebscoketSpot : IDisposable
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



        List<CommandEnum.OkexSwapModel> symbollist;

        string symbol;
        /// <summary>
        /// Okex  现货
        /// </summary>
        /// <param name="url"></param>
        public OkexWebscoketSpot(string url, string symbol)
        {
            this.symbol = symbol;

            //symbollist = CommonProcess.GetOKEXSymbolticket().Where(a => a.instrument_id.Contains("USDT") && Convert.ToDecimal(a.quote_volume_24h) > 20000000).ToList();
            symbollist = CommandEnum.OkexMessage.GetSwapContract_size().Where(P=>P.quote_currency=="USDT").ToList();


            LogHelpers = LogHelper.CreateInstance();
            if (url != null && url != "")
            {
                ServerPath = url;
            }
            else
            {
                ServerPath = "wss://real.okex.com:8443/ws/v3";
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
                LogHelpers.Info(" Okex 现货启动失败：" + ex.ToString());
                Console.WriteLine(" Okex 现货启动失败：" + ex.ToString());

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
            string data = null;
            if (e.IsPing)
            {
                return;
            }
            if (e.IsBinary)
            {
                data = GZipDecompresser.DecompressData(e.RawData);
            }
            else
            {
                data = e.Data;
            }

  
            Savedata(data);
            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, data, CommandEnum.RedisKey.okex, CommandEnum.RedisKey.okex + "现货");
            });

        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            LogHelpers.Info(" Okex 现货websocket_Closed：" + e.ToString());

            Console.WriteLine(" Okex 现货websocket_Closed：" + e.ToString());

        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {
            LogHelpers.Info(" Okex 现货websocket_Error：" + e.ToString());

            Console.WriteLine(" Okex 现货websocket_Error：" + e.ToString());
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
                        LogHelpers.Info("ok现货 正在重连");
                        Console.WriteLine("ok现货 正在重连");
                    }

                    else
                    {
                        LogHelpers.Info("ok现货 " + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        Console.WriteLine("ok现货 " + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ok现货正在重连异常" + ex.ToString());
                    LogHelpers.Error("ok现货重连websocket_Error:" + ex.ToString());

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
                    if (((object)results.data) == null)
                    {
                        return;
                    }
                    var resultdata = (((object)results.data).ToString()).ToList<Okex>();
                    List<string> maxlist = new List<string>();
                    if (resultdata != null && resultdata.Count > 0)
                    {

                        List<UPermanentFutures> list = new List<UPermanentFutures>();

                        string[] arr3 = (resultdata.FirstOrDefault().instrument_id).ToString().Split('-');

                        var pair = arr3[0];
                        var coin = arr3[1];
                        foreach (var item in resultdata)
                        {
                            var times = Convert.ToDateTime(item.timestamp);
                            var key = times.ToString("g");
                            var keys = times.ToString("d");
                            item.actcualtime = key;
                            UPermanentFutures model = new UPermanentFutures();

                            model = USDTMapping(item, pair, coin);
                                         
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



                       
                         RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.SpotQueueList, list.ToJson());
                      

                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("ok现货信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("ok现货信息转化错误，错误的信息源：" + message);
                    Console.WriteLine("ok现货信息转化错误，错误信息：" + e.Message.ToString());
                    //throw e;
                }
            }
            else
            {
                LogHelpers.Info("ok现货信息为空，无法保存");
                Console.WriteLine("ok现货信息为空，无法保存");
            }
        }

     
        /// <summary>
        /// USDT转化类-- 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures USDTMapping(Okex model, string pair, string coin)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.trade_id;
            o.exchange = CommandEnum.RedisKey.okex;
            o.market = model.instrument_id;
            o.pair = model.instrument_id;
            o.price = model.price;
            o.qty = model.size;
            //o.vol = model.qty.ToString();
            o.contractcode = pair;
            o.vol = ((Convert.ToDecimal(model.price)) * Convert.ToDecimal(model.size)).ToString();
            o.types = "OK现货:" + pair;
            o.Unit = "币";
            o.side = model.side;
            o.times = model.actcualtime;
            o.timestamp = model.timestamp;
            o.utctime = model.actcualtime;
            o.kind = CommandEnum.RedisKey.SPOT;
            return o;
        }


 




        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string Message)
        {      
            string list = string.Empty;
            foreach (var item in this.symbollist)
            {

                string topic = item.underlying_index+"-"+item.quote_currency;

                list += "\"spot/trade:" + topic + "\",";

            }

            list = list.Remove(list.Length - 1);
           

            //string topic = "\"swap/trade:" + "BTC-USD-SWAP" + "\"";
            var ops = "{ \"op\": \"subscribe\", \"args\": [" + list + "]}";


            if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            {
                this._webSocket.Send(ops);
            }

       
        }
        public string GetSendData()
        {
            return "1";          
        }
        public void Dispose()
        {
            LogHelpers.Error(" Okex 现货DISPOSE：" + DateTime.Now.ToString());
            Console.WriteLine(" Okex 现货DISPOSE：" + DateTime.Now.ToString());
        }
    }
}
