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
    public class OkexWebscoket : IDisposable
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
        /// Okex USD永续合约（包含币本位和usdt）
        /// </summary>
        /// <param name="url"></param>
        public OkexWebscoket(string url, string symbol)
        {
            this.symbol = symbol;

            symbollist = CommandEnum.OkexMessage.GetSwapContract_size();
            //SendMessage("");
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
                LogHelpers.Info(" Okex USD永续合约启动失败：" + ex.ToString());
                Console.WriteLine(" Okex USD永续合约启动失败：" + ex.ToString());

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

           //Console.WriteLine(" Received:" + data);
            //LogHelpers.Info(" Received:" + data);

            //Task.Run(() =>
            //{
            //Savedata(data);
            //});
            //MessageReceived?.Invoke(e.Data);

        
            Savedata(data);
            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, data, CommandEnum.RedisKey.okex, CommandEnum.RedisKey.okex + "永续币本位和USDT");
            });

        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            LogHelpers.Info(" Okex USD永续合约websocket_Closed：" + e.ToString());

            Console.WriteLine(" Okex USD永续websocket_Closed：" + e.ToString());
   
        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {
            LogHelpers.Info(" Okex USD永续合约websocket_Error：" + e.ToString());

            Console.WriteLine(" Okex USD永续websocket_Error：" + e.ToString());
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
                        LogHelpers.Info("ok永续合约正在重连");
                        Console.WriteLine("ok永续合约正在重连");
                    }

                    else
                    {
                        LogHelpers.Info("ok永续合约" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive+DateTime.Now.ToString());
                        Console.WriteLine("ok永续合约" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ok永续正在重连异常" + ex.ToString());
                    LogHelpers.Error("ok永续重连websocket_Error:" + ex.ToString());

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


                            if (coin.ToString().ToUpper() == "USDT")
                            {

                                model = USDTMapping(item, pair,coin);
                            }
                            else
                            {
                                model = Mapping(item, pair,coin);
                            }
                            model.kind = CommandEnum.RedisKey.PERP;

                            list.Add(model);

                            //if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
                            //{
                            //    RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder+ DateTime.Now.ToString("yyyy-MM-dd"));
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


                        RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UPermanentFutures);

                       
                        RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.PERPQueueList, list.ToJson());
                    
                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("ok永续信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("ok永续信息转化错误，错误的信息源：" + message);
                    Console.WriteLine("ok永续信息转化错误，错误信息：" + e.Message.ToString());
                    //throw e;
                }
            }
            else
            {
                LogHelpers.Info("ok永续信息为空，无法保存");
                Console.WriteLine("ok永续信息为空，无法保存");
            }
        }

        /// <summary>
        /// USD转化类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(Okex model, string pair, string coin)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.trade_id;
            o.exchange = CommandEnum.RedisKey.okex;
            o.market = model.instrument_id;
            o.pair = model.instrument_id;
            o.price = model.price;
            o.qty = model.size;

            o.vol = ((Convert.ToInt64(model.size) * swich(model.instrument_id))).ToString();
            o.types = "OK USD永续合约:" + pair;
            o.Unit = "张";
            o.contractcode = pair;

            //if (pair == CommandEnum.RedisKey.coin)
            //{
            //    if (coin.ToString().ToUpper() == "USDT")
            //    {
            //        o.vol = ((Convert.ToInt64(model.size) * 0.01M) * Convert.ToDecimal(model.price)).ToString();
            //    }
            //    else
            //    {
            //        o.vol = ((Convert.ToInt64(model.size) * 100)).ToString();//bbenwei
            //    }
            //}
            //else
            //{
            //    o.vol = model.size.ToString();
            //}
            o.side = model.side;
            o.times = model.actcualtime;
            o.timestamp = model.timestamp;
            o.utctime = model.actcualtime;
            //o.utctime = model.timestamp;
            //o.times = model.actcualtime;
            //o.timestamp = model.trade_time_ms;

            return o;
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

            o.vol = ((Convert.ToInt64(model.size) * swich(model.instrument_id)) * Convert.ToDecimal(model.price)).ToString();
            o.types = "OK USDT永续合约:" + pair;
            o.Unit = "张";



            //if (pair.ToUpper() == CommandEnum.RedisKey.coin)
            //{
            //    o.vol = ((Convert.ToInt64(model.qty) * 100)).ToString();
            //}
            //else
            //{ 
            //o.vol= ((Convert.ToInt64(model.qty))).ToString();
            //}
            o.side = model.side;
            o.times = model.actcualtime;
            o.timestamp = model.timestamp;
            o.utctime = model.actcualtime;
            //o.utctime = model.timestamp;
            //o.times = model.actcualtime;
            //o.timestamp = model.trade_time_ms;

            return o;
        }




        public decimal swich(string coin)
        {
            decimal salary = 0;

            var result = this.symbollist.Where(p => p.instrument_id == coin).FirstOrDefault();
            if (result != null)
            {
                salary = Convert.ToDecimal(result.contract_val);
            }
            else
            {
                salary = 0.00001m;
                LogHelpers.Info("Okex 交割合约价值信息为空，价值计算错误！");
                Console.WriteLine("Okex 交割合约价值信息为空，价值计算错误！");
            }


            return salary;

        }





        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string Message)
        {

            //if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //{
            //    this._webSocket.Send(Message);
            //}
            string list = string.Empty;
            foreach (var item in this.symbollist)
            {

                string topic = item.instrument_id;

                list += "\"swap/trade:" + topic + "\",";



            }

            list = list.Remove(list.Length - 1);
            //var ops = "{ \"op\": \"subscribe\", \"args\": [\"swap/trade:" + topic + "\"]}";



            //string topic = "\"swap/trade:" + "BTC-USD-SWAP" + "\"";
            var ops = "{ \"op\": \"subscribe\", \"args\": ["+ list + "]}";


            if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            {
                this._webSocket.Send(ops);
            }


            //string[] arr1 = symbol.Split(',');

            //foreach (var item in arr1)
            //{

            //    string topic = $"{item}-USD-SWAP";

            //    var ops = "{ \"op\": \"subscribe\", \"args\": [\"swap/trade:" + topic + "\"]}";

            //    if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //    {
            //        this._webSocket.Send(ops);
            //    }
            //}

            //foreach (var item in arr1)
            //{

            //    string topic = $"{item}-USDT-SWAP";


            //    var ops = "{ \"op\": \"subscribe\", \"args\": [\"swap/trade:" + topic + "\"]}";

            //    if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //    {
            //        this._webSocket.Send(ops);
            //    }
            //}


        }

        public string GetSendData()
        {


            //string tradecoin = "BTC-USDT-210326";

            //var results = "{ \"op\": \"subscribe\", \"args\": [\"futures/trade:" + tradecoin + "\"]}";


            return "1";

            //币本位永续
            //{ "op": "subscribe", "args": ["swap/trade:BTC-USD-SWAP"]}
            //U本位永续
            //{ "op": "subscribe", "args": ["swap/trade:BTC-USDT-SWAP"]}
        }

        public void Dispose()
        {
            LogHelpers.Error(" Okex USD永续合约DISPOSE：" + DateTime.Now.ToString()); 
            Console.WriteLine(" Okex USD永续合约DISPOSE：" + DateTime.Now.ToString());


            //this._isRunning = false;
            //try
            //{
            //    _thread.Abort();

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
