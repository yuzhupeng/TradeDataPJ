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
    public class OkexWebscoketDeliveryFutures : IDisposable
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


        string delivery_date;

        ILog LogHelpers;

        List<CommandEnum.OkexDeliveryModel> symbollist;


        string symbol;
        /// <summary>
        /// Okex USDT and usd交割合约
        /// </summary>
        /// <param name="url"></param>
        public OkexWebscoketDeliveryFutures(string url, string symbol,string datecode)
        {

            try
            {

                this.symbol = symbol;

                this.delivery_date = datecode;

                symbollist = CommandEnum.OkexMessage.GetDeliveryContract_size();

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
            catch (Exception ex)
            {
                LogHelpers.Info(" Okex USDT交割合约初始化启动失败：" + ex.ToString());

                Console.WriteLine(" Okex USDT交割合约初始化启动失败：" + ex.ToString());

                
            }

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
               
                LogHelpers.Info(" Okex USDT交割合约启动失败：" + ex.ToString());

               Console.WriteLine(" Okex USDT交割合约启动失败：" + ex.ToString());

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

           
            Savedata(data);
            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, data, CommandEnum.RedisKey.okex, CommandEnum.RedisKey.okex + "交割");
            });


        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            LogHelpers.Info(" Okex USDT交割合约websocket_Closed：" + e.ToString());

            Console.WriteLine(" Okex USDT交割websocket_Closed：" + e.ToString());
 
           
        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {
           
            LogHelpers.Info(" Okex USDT交割合约websocket_Error：" + e.ToString());

            Console.WriteLine(" Okex USDT交割websocket_Error：" + e.ToString());



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
                        LogHelpers.Info("Okex USDT交割合约正在重连" + DateTime.Now.ToString());
                        Console.WriteLine("Okex USDT交割合约正在重连" + DateTime.Now.ToString());
                    }
                    else
                    {
                        LogHelpers.Info("Okex USDT交割合约当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        Console.WriteLine("Okex USDT交割合约当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Info(" Okex USDT交割合约正在重连异常：" + e.ToString());
                    Console.WriteLine(" Okex USDT交割正在重连异常：" + e.ToString());

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
                    var resultdata = (((object)results.data).ToString()).ToList<Okex>();
                    List<string> maxlist = new List<string>();
                    if (resultdata != null && resultdata.Count > 0)
                    {
                        //var tradeIDlist = resultdata.GroupBy(a => a.cross_seq).ToList();
                        List<UPermanentFutures> list = new List<UPermanentFutures>();

                        string[] arr3 = (resultdata.FirstOrDefault().instrument_id).ToString().Split('-');

                        var pair = arr3[0];
                        var coin = arr3[1];
                        if (coin.ToString().ToUpper() == "USDT")
                        {

                            SaveUSDT(resultdata, list, pair);
                        }
                        else
                        {
                            SaveUSD(resultdata, list, pair);
                        }

                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("Okex USDT交割合约信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("Okex USDT交割合约信息转化错误，错误的信息源：" + message);
                    Console.WriteLine("Okex USDT交割合约信息转化错误，错误信息：" + e.Message.ToString());
                    //throw e;
                }
            }
            else
            {
                LogHelpers.Info("Okex USDT交割合约信息为空，无法保存");
                Console.WriteLine("Okex USDT交割合约信息为空，无法保存");
            }
        }

        private void SaveUSDT(List<Okex> resultdata, List<UPermanentFutures> list, string pair)
        {
            foreach (var item in resultdata)
            {
                var times = Convert.ToDateTime(item.timestamp);
                var key = times.ToString("g");
                var keys = times.ToString("d");
                item.actcualtime = key;
                UPermanentFutures model = new UPermanentFutures();
                model = Mapping(item, pair);
                model.kind = CommandEnum.RedisKey.DELIVERY;
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


            RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UDeliveryFutures);
          
            RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.DeliverQueueList, list.ToJson());
            

        }


        private void SaveUSD(List<Okex> resultdata, List<UPermanentFutures> list, string pair)
        {
            foreach (var item in resultdata)
            {
                var times = Convert.ToDateTime(item.timestamp);
                var key = times.ToString("g");
                var keys = times.ToString("d");
                item.actcualtime = key;
                UPermanentFutures model = new UPermanentFutures();
                model = USDMapping(item, pair);
                model.kind = CommandEnum.RedisKey.DELIVERY;
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


            RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UDeliveryFutures);

            
            RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.DeliverQueueList, list.ToJson());
            
        }



        /// <summary>
        /// 转化类 usdt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(Okex model, string pair)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.trade_id;
            o.exchange = CommandEnum.RedisKey.okex;
            o.market = model.instrument_id;
            o.pair = model.instrument_id;
            o.price = model.price;
            o.qty = model.qty;
            //o.vol = model.qty;
            //  o.vol = ((Convert.ToInt64(model.qty) * 0.01M) * Convert.ToDecimal(model.price)).ToString();
            o.contractcode = pair;

            o.vol = ((Convert.ToInt64(model.qty) * swich(model.instrument_id)) * Convert.ToDecimal(model.price)).ToString();
            o.types = "OK USDT交割合约:" + model.instrument_id;
            o.Unit = "张";

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
        /// 转化类 bi benwei 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures USDMapping(Okex model, string pair)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.trade_id;
            o.exchange = CommandEnum.RedisKey.okex;
            o.market = model.instrument_id;
            o.pair = model.instrument_id;
            o.price = model.price;
            o.qty = model.qty;
            //o.vol = model.qty.ToString();
            o.contractcode = pair;
            o.vol = ((Convert.ToInt64(model.qty) * swich(model.instrument_id))).ToString();
            o.types = "OK USD交割合约:" + model.instrument_id;
            o.Unit = "张";


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
            try
            {


                //foreach (var item in this.symbollist)
                //{

                //    string topic = item.instrument_id;

                //    var ops = "{ \"op\": \"subscribe\", \"args\": [\"futures/trade:" + topic + "\"]}";

                //    if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
                //    {
                //        this._webSocket.Send(ops);
                //    }
                //}

                //foreach (var item in this.symbollist)
                //{

                //    string topic = item.instrument_id;

                //    var ops = "{ \"op\": \"subscribe\", \"args\": [\"futures/trade:" + topic + "\"]}";

                //    if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
                //    {
                //        this._webSocket.Send(ops);
                //    }
                //}



                string list = string.Empty;
                foreach (var item in this.symbollist)
                {
                    string topic = item.instrument_id;
                    list += "\"futures/trade:" + topic + "\",";
                }
                list = list.Remove(list.Length - 1);          
                var ops = "{ \"op\": \"subscribe\", \"args\": [" + list + "]}";
                if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
                {
                    this._webSocket.Send(ops);
                }




            }
            catch (Exception e)
            {
                LogHelpers.Info("Okex USDT交割合约发送异常，异常信息：" + e.Message.ToString());
                Console.WriteLine("Okex USDT交割合约发送异常，异常信息：" + e.Message.ToString());
            }



            //try
            //{
            //    string[] arr1 = symbol.Split(',');

            //    foreach (var item in arr1)
            //    {

            //        string topic = $"{item}-USD-{delivery_date}";

            //        var ops = "{ \"op\": \"subscribe\", \"args\": [\"futures/trade:" + topic + "\"]}";

            //        if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //        {
            //            this._webSocket.Send(ops);
            //        }
            //    }

            //    foreach (var item in arr1)
            //    {

            //        string topic = $"{item}-USDT-{delivery_date}";

            //        var ops = "{ \"op\": \"subscribe\", \"args\": [\"futures/trade:" + topic + "\"]}";

            //        if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //        {
            //            this._webSocket.Send(ops);
            //        } 
            //    }

            //}
            //catch (Exception e)
            //{
            //    LogHelpers.Info("Okex USDT交割合约发送异常，异常信息："+e.Message.ToString());
            //    Console.WriteLine("Okex USDT交割合约发送异常，异常信息：" + e.Message.ToString());
            //}

        }
        public string GetSendData()
        {


            string tradecoin = "BTC-USDT-210326";

            var results = "{ \"op\": \"subscribe\", \"args\": [\"futures/trade:" + tradecoin + "\"]}";


            return results;

            //币本位永续
            //{ "op": "subscribe", "args": ["swap/trade:BTC-USD-SWAP"]}
            //U本位永续
            //{ "op": "subscribe", "args": ["swap/trade:BTC-USDT-SWAP"]}
        }

        public void sendping(string op)
        {

            if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            {
                this._webSocket.Send(op);
            }
        }


        public void Dispose()
        {

            LogHelpers.Error("Okex USDT交割合约发送异常断开连接异常" + DateTime.Now.ToString());
            LogHelpers.Info("Okex USDT交割合约发送异常断开连接异常" + DateTime.Now.ToString());
            Console.WriteLine("Okex USDT交割合约发送异常连接异常" + DateTime.Now.ToString());
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
