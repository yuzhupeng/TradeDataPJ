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


 
namespace GetTradeHistoryData
{
    public class HuobiSpotWebscoket : IDisposable
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
        /// 火币现货
        /// </summary>
        /// <param name="url">
        //wss://api.huobi.pro/ws
        //wss://api-aws.huobi.pro/ws
        /// <param name="symbol"></param>
        public HuobiSpotWebscoket(string url, string type)
        {
            LogHelpers = LogHelper.CreateInstance();


            symbollist = CommandEnum.PerpHUOBI.GetContract_sizeapi();
            if (type.ToUpper() == "Local")
            { 
                ServerPath = "wss://api.huobi.pro/ws";
            }
            else
            {
                
                ServerPath = "wss://api-aws.huobi.pro/ws";
            
            }
            this._webSocket = new WebSocket(ServerPath);
            this._webSocket.OnOpen += WebSocket_Opened;
            this._webSocket.OnError += WebSocket_Error;
            this._webSocket.OnClose += WebSocket_Closed;
            this._webSocket.OnMessage += WebSocket_MessageReceived;
            this._webSocket.SetProxy("http://127.0.0.1:1080", "", "");
            //if (type.ToUpper() != "Local")
            //{
            //    this._webSocket.SetProxy("http://127.0.0.1:1080", "", "");
            //}
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
  
            //Console.WriteLine("数据："+data);
            Savedata(data);

 

            Task.Run(() =>
            {               
                CommandEnum.WriteSaveMessage(DateTime.Now, data, CommandEnum.RedisKey.huobi, CommandEnum.RedisKey.huobi + "现货");
            });

 
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
                        LogHelpers.Info("火币现货当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        Console.WriteLine("火币现货当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("火币现货正在重连异常" + ex.ToString());
                    LogHelpers.Error("火币现货重连websocket_Error:" + ex.ToString());

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
                            Console.WriteLine("huobi信息转化为空！！！！" + (object)results.ping);
                            return;
                        }                      
                        string ping = ((object)results.ping).ToString();
                        sendping("{\"pong\":" + ping + "}");
                        return;
                    }


                    var resultdata = (((object)results.tick.data).ToString()).ToList<huobiSpot>();
                    List<string> maxlist = new List<string>();
                    if (resultdata != null && resultdata.Count > 0)
                    {
                        //var tradeIDlist = resultdata.GroupBy(a => a.cross_seq).ToList();
                        List<UPermanentFutures> list = new List<UPermanentFutures>();
                        string[] arr3 = ((object)results.ch).ToString().Split('.');
                        var pair = arr3[1];                    
                        var coin = "USDT";
                        var symbol = pair.Replace("usdt","");
                        foreach (var item in resultdata)
                        {
                            var times = GZipDecompresser.GetTimeFromUnixTimestamps(item.ts.ToString());
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

                }
                catch (Exception e)
                {
                    LogHelpers.Error("huobi现货信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("huobi现货信息转化错误，错误的信息源：" + message);
                    Console.WriteLine("huobi现货信息转化错误，错误信息：" + e.Message.ToString());
                    //throw e;
                }
            }
            else
            {
                LogHelpers.Info("huobi现货信息为空，无法保存");
                Console.WriteLine("huobi现货信息为空，无法保存");
            }
        }

        /// <summary>
        /// 转化类 价值计算
        /// 币本位：https://support.hbfile.net/hc/zh-cn/articles/900000106046-%E5%B8%81%E6%9C%AC%E4%BD%8D%E6%B0%B8%E7%BB%AD%E5%90%88%E7%BA%A6%E5%93%81%E7%A7%8D%E8%A6%81%E7%B4%A0
        /// USDT本位：https://support.hbfile.net/hc/zh-cn/articles/900001326646-%E5%90%88%E7%BA%A6%E5%93%81%E7%A7%8D%E8%A6%81%E7%B4%A0
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(huobiSpot model, string pair, string coin, string symbol)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.cross_seq = model.tradeId.ToString();
            o.exchange = CommandEnum.RedisKey.huobi;
            o.market = pair.ToUpper();
            o.pair = pair.ToUpper();
            o.contractcode = symbol.ToUpper();
            o.price = model.price.ToString();
            o.qty = model.amount.ToString();
            o.Unit = "币";     
            o.vol = (Convert.ToDecimal(model.price) * Convert.ToDecimal(model.amount)).ToString();
            o.types = "huobi 现货:" + pair;       
            o.side = model.direction;
            o.times = model.actcualtime.AddHours(8).ToString();
            o.timestamp = model.ts.ToString();
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

            foreach (var item in symbollist)
            {
                var symbols = $"{item.symbol.ToLower()+"usdt"}";
                string topic = $"market.{symbols}.trade.detail";
                string clientId = "id7";
                var op = ($"{{ \"sub\": \"{topic}\",\"id\": \"{clientId}\" }}");
                if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
                {
                    this._webSocket.Send(op);
                }
            }


            //string symbollisst = "";
            //foreach (var item in symbollist.Where(p => p.symbol.Contains("usdt") && p.vol > 20000000 && !(p.symbol.Contains("3"))))
            //{
            //    var symbols = $"{item.symbol}";
            //    string topic = $"market.{symbols}.trade.detail";
            //    string clientId = item.symbol;
            //    var op = ($" \"sub\": \"{topic}\",\"id\": \"{clientId}\",");
            //    symbollisst += op;
            //}
            //symbollisst = symbollisst.Remove(symbollisst.Length - 1);
            //symbollisst = "{" + symbollisst + "}";

            //if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            //{
            //    this._webSocket.Send(symbollisst);
            //}



        }
        public string GetSendData()
        {           
            return "1";
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


            
        }
    }
}
