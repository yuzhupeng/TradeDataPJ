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
using static GetTradeHistoryData.CommandEnum;



//install-package WebSocket4Net
//install-package nlog
//install-package nlog.config
namespace GetTradeHistoryData
{
    public class huobiWebScoket : IDisposable
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

        /// <summary>
        /// 币种
        /// </summary>
        string symbol;


        /// <summary>
        /// USDT/USD 币本位还是usdt
        /// </summary>
        string coins;

        List<USDTModel> symbollist;


        ILog LogHelpers;


        /// <summary>
        /// 火币永续（usdt和币本位）
        /// </summary>
        /// <param name="url">
        /// wss://api.btcgateway.pro/swap-ws    ---币本位jiaoge
        /// wss://api.btcgateway.pro/linear-swap-ws  ---usdt永续
        /// wss://api.btcgateway.pro/swap-ws 币本位永续
        /// </param>
        /// <param name="symbol"></param>
        public huobiWebScoket(string url, string symbol, string usdusdt)
        {
            LogHelpers = LogHelper.CreateInstance();
            this.symbol = symbol;

            this.coins = usdusdt;
            //if (url != null && url != "")
            //{
            //    ServerPath = url;
            //}
            //else
            //{         
            //      ServerPath = "wss://api.btcgateway.pro/linear-swap-ws";//USDT
            //    //wss://api.huobi.pro/ws
            //}

            if (usdusdt.ToUpper() == "USD")
            {
                symbollist = CommandEnum.PerpHUOBI.GetUSDContract_sizeapi();
                ServerPath = "wss://api.btcgateway.pro/swap-ws";
            }
            else
            {
                symbollist= CommandEnum.PerpHUOBI.GetContract_sizeapi();
                ServerPath = "wss://api.btcgateway.pro/linear-swap-ws";
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
            string data = null;
            if (e.IsBinary)
            {
                data = GZipDecompresser.Decompress(e.RawData);
            }
            else
            {
                data = e.Data;
            }
            //if (e.IsPing)
            //{
            //    Console.WriteLine(" pingzhis:" + e.RawData);
            //}
            //Console.WriteLine("数据："+data);
            Savedata(data);


            //if (coins.ToUpper() == "USD")
            //{

            //    LogHelpers.Info("火币币本位永续当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
            //    Console.WriteLine("火币币本位永续当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
            //}
            //else
            //{

            //    LogHelpers.Info("火币USDT永续当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
            //    Console.WriteLine("火币USDT永续当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
            //}

            Task.Run(() =>
            {
                string code = coins.ToUpper() == "USD" ? "币本位永续" : "USDT永续";
                CommandEnum.WriteSaveMessage(DateTime.Now, data, CommandEnum.RedisKey.huobi, CommandEnum.RedisKey.huobi+ code);
            });


            //Savedata(data);
            //Console.WriteLine(" Received:" + data);
            //LogHelpers.Info(" Received:" + data);
            //MessageReceived?.Invoke(e.Data);
        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("火币websocket_Closed" + e.ToString());
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
            Console.WriteLine("火币websocket_Closed" + e.ToString());
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
                    if (this._webSocket.IsAlive != true || this._webSocket.ReadyState != WebSocketState.Open)
                    {


                        //LogHelper.WriteLog(" Reconnect websocket WebSocketState:" + this._webSocket.State);


                        this._webSocket.Close();
                        this._webSocket.Connect();
                        if (Sendata != null && Sendata != "")
                        {
                            SendMessage(Sendata);
                        }
                        LogHelpers.Info("huobi正在重连");
                        Console.WriteLine("huobi正在重连");
                    }
                    else
                    {
                        if (coins.ToUpper() == "USD")
                        {

                            LogHelpers.Info("火币币本位永续当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                            Console.WriteLine("火币币本位永续当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        }
                        else
                        {

                            LogHelpers.Info("火币USDT永续当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                            Console.WriteLine("火币USDT永续当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("火币正在重连异常" + ex.ToString());
                    LogHelpers.Error("火币重连websocket_Error:" + ex.ToString());

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
                            return;
                        }
                        //Console.WriteLine("huobi信息转化为空！！！！" + (object)results.ping);
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
                        string[] arr4 = (pair).ToString().Split('-');
                        var coin = arr4[1];
                        var symbol = arr4[0];
                        foreach (var item in resultdata)
                        {
                            var times = GZipDecompresser.GetTimeFromUnixTimestamps(item.ts.ToString());
                            var key = times.ToString("g");
                            var keys = times.ToString("d");
                            item.actcualtime = times;
                            UPermanentFutures model = new UPermanentFutures();
                            model = Mapping(item, pair, coin, symbol);
                            model.kind = CommandEnum.RedisKey.PERP;
                            list.Add(model);

                            //if (coin.ToUpper() == "USDT")
                            //{                              
                            //    //o.vol = item.trade_turnover;
                            //    var sizes = resultdata.Where(p => p.ts == item.ts).ToList().Sum(p => (Convert.ToDecimal(p.trade_turnover)));
                            //    if (sizes >= 1000000 && maxlist.FirstOrDefault(a => a == item.ts.ToString()) == null)
                            //    {
                            //        UPermanentFutures models = new UPermanentFutures();
                            //        ObjectUtil.MapTo(model, models);
                            //        models.vol = sizes.ToString();
                            //        models.qty = (sizes / Convert.ToDecimal(models.price)).ToString();
                            //        RedisHelper.Pushdata(models.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
                            //        maxlist.Add(item.ts.ToString());
                            //        Console.WriteLine(models.ToJson());                             
                            //    }
                            //}
                            //else
                            //{                       
                            //    var sizes = resultdata.Where(p => p.ts == item.ts).ToList().Sum(p => (Convert.ToDecimal(p.price) * Convert.ToDecimal(p.quantity)));
                            //    if (sizes >= 1000000 && maxlist.FirstOrDefault(a => a == item.ts.ToString()) == null)
                            //    {
                            //        UPermanentFutures models = new UPermanentFutures();
                            //        ObjectUtil.MapTo(model, models);
                            //        models.vol = sizes.ToString();
                            //        models.qty = (sizes / Convert.ToDecimal(models.price)).ToString();
                            //        RedisHelper.Pushdata(models.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
                            //        maxlist.Add(item.ts.ToString());
                            //        Console.WriteLine(models.ToJson());
                            //    }
                            //}
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




                        RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UPermanentFutures);
                   
                        RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.PERPQueueList, list.ToJson());
                       
                    }
                }
                catch (Exception e)
                {
                    LogHelpers.Error("huobi信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("huobi信息转化错误，错误的信息源：" + message);
                    Console.WriteLine("huobi信息转化错误，错误信息：" + e.Message.ToString());
                    //throw e;
                }
            }
            else
            {
                LogHelpers.Info("huobi信息为空，无法保存");
                Console.WriteLine("huobi信息为空，无法保存");
            }
        }

        /// <summary>
        /// 转化类 价值计算
        /// 币本位：https://support.hbfile.net/hc/zh-cn/articles/900000106046-%E5%B8%81%E6%9C%AC%E4%BD%8D%E6%B0%B8%E7%BB%AD%E5%90%88%E7%BA%A6%E5%93%81%E7%A7%8D%E8%A6%81%E7%B4%A0
        /// USDT本位：https://support.hbfile.net/hc/zh-cn/articles/900001326646-%E5%90%88%E7%BA%A6%E5%93%81%E7%A7%8D%E8%A6%81%E7%B4%A0
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(huobi model, string pair, string coin, string symbol)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.id;
            o.exchange = CommandEnum.RedisKey.huobi;
            o.market = pair;
            o.pair = pair;
            o.contractcode = symbol;
            o.price = model.price.ToString();
           
            o.Unit = "币";
            if (coin.ToUpper() == "USDT")
            {
                o.types = "U本位永续合约:" + pair;
                o.vol = model.trade_turnover;

                //if (pair == "BTC-USDT")
                //{
                //    o.vol = ((Convert.ToInt64(model.amount) * 100)).ToString();
                //}
                //else
                //{
                //    o.vol = ((Convert.ToInt64(model.amount) * 10)).ToString();
                //}
            }
            else
            {
                //o.vol = ((Convert.ToInt64(model.amount) * swich(symbol)) * Convert.ToDecimal(model.price)).ToString();
                o.vol = (Convert.ToDecimal(model.price) * Convert.ToDecimal(model.quantity)).ToString();
                o.types = "币本位永续合约:" + pair;
            }

            o.qty = (Convert.ToDecimal(o.vol) / Convert.ToDecimal(o.price)).ToString();
            o.side = model.direction;
            o.times = model.actcualtime.AddHours(8).ToString();
            o.timestamp = model.ts.ToString();
            o.utctime = model.actcualtime.ToString();
            //o.utctime = model.timestamp;
            //o.times = model.actcualtime;
            //o.timestamp = model.trade_time_ms;
            return o;
        }

        public decimal swich(string coin)
        {
            decimal salary = 0;

            var result = CommandEnum.PerpHUOBI.GetContract_sizeapi().Where(p => p.symbol == coin).FirstOrDefault();
            if (result != null)
            {
                salary = result.contract_size;
            }
            else
            {
                salary = 0.00001m;
            }

            ////  BTC,LTC,BCH,ETH,EOS,BSV,ETC,LINK,DOT,XRP,UNI,DOGE,SHUSI,ETC
            //  switch (coin.ToUpper())
            //  {
            //      case "BTC":
            //          salary = 0.001m;
            //          break;
            //      case "ETH":
            //          salary =0.001M;
            //          break;
            //      case "LTC":
            //          salary = 0.1M;
            //          break;
            //      case "EOS":
            //          salary = 200;
            //          break;
            //      case "BCH":
            //          salary = 500;
            //          break;
            //      default:
            //          Console.WriteLine("输入的有误，请重新输入");
            //          salary = 0.1m;
            //          break;
            //  }
            return salary;

        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string Message)
        {
            string[] arr1 = symbol.Split(',');

            if (coins.ToUpper() == "USD")
            {

              //  var result = CommandEnum.PerpHUOBI.GetUSDContract_size();
                foreach (var item in symbollist)
                {
                    var symbols = $"{item.contract_code}";
                    string topic = $"market.{symbols}.trade.detail";
                    string clientId = "id7";
                    var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");
                    if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
                    {
                        this._webSocket.Send(op);
                    }
                }

            }
            else
            {
              //  var result = CommandEnum.PerpHUOBI.GetContract_size();
                foreach (var item in symbollist)
                {


                    var symbols = $"{item.contract_code}";
                    string topic = $"market.{symbols}.trade.detail";
                    string clientId = "id7";
                    var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");
                    if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
                    {
                        this._webSocket.Send(op);
                    }
                }
            }

            //string liquidation = "public.*.liquidation_orders";
            //string orders = "sub";
            //var mes = ($"{{ \"op\": \"{orders}\",\"topic\": \"{liquidation}\" }}");


            //if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //{
            //    this._webSocket.Send(mes);
            //}
        }
        public string GetSendData()
        {

            //string[] arr1 = symbol.Split(',');

            //string results = "";
            //string clientId = "id7";
            //string op = "";

            //foreach (var item in arr1)
            //{
            //    var symbols = $"{item}-USDT";
            //    results += $"sub:\"market.{symbols}.trade.detail\",";
            //}
            //op = ("{{" + results + "\"id\": \""+clientId+"\" }}");

            //this.symbol = "BTC-USDT";
            //string clientId = "id7";
            //string topic = $"market.{symbol}.trade.detail";

            //var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");

            return "1";



            ;
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
            LogHelpers.Error("huobi DISPOSE断开连接异常" + DateTime.Now.ToString());
            LogHelpers.Info("huobi DISPOSE断开连接异常" + DateTime.Now.ToString());
            Console.WriteLine("huobi DISPOSE断开连接异常" + DateTime.Now.ToString());


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
