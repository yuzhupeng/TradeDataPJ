using log4net;
using Newtonsoft.Json;
using QPP.Core;
using SuperSocket.ClientEngine.Proxy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;



//install-package WebSocket4Net
//install-package nlog
//install-package nlog.config
namespace GetTradeHistoryData
{
    public class bitmexWebscoketnew : IDisposable
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






        public bitmexWebscoketnew(string url)
        {
            LogHelpers = LogHelper.CreateInstance();
            if (url != null && url != "")
            {
                ServerPath = url;
            }
            else
            {
                //ServerPath = "wss://www.bitmex.com/realtime?subscribe=trade:XBTUSD";


                ServerPath = "wss://www.bitmex.com/realtime?subscribe=trade:XBTUSD,trade:LTCUSD,trade:ETHUSD,liquidation:XBTUSD,liquidation:ETHUSD,liquidation:LTCUSD";
            }


            this._webSocket = new WebSocket(ServerPath);
            //this._webSocket.OnOpen += WebSocket_Opened;
            //this._webSocket.OnError += WebSocket_Error;
            //this._webSocket.OnClose += WebSocket_Closed;
            //this._webSocket.OnMessage += WebSocket_MessageReceived;
            this._webSocket.MessageReceived += (o, s) => WebSocket_MessageReceived(s.Message);
            this._webSocket.Proxy = new HttpConnectProxy(new DnsEndPoint("127.0.0.1", 1080));
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
                this._webSocket.Open();

                this._isRunning = true;
                this._thread = new Thread(new ThreadStart(CheckConnection));
                this._thread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("bitmex连接失败:" + ex.ToString());
                LogHelpers.Info("bitmex连接失败:" + ex.ToString());
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 消息收到事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WebSocket_MessageReceived(string Message)
        {

            //Console.WriteLine(" Received:" + e.Data);
            //LogHelpers.Info(" Received:" + e.Data);

            dynamic results = JsonConvert.DeserializeObject<dynamic>(Message);

            if (((object)results.data) == null)
            {
                Console.WriteLine(" 消息类型无法识别:" + Message);
                return;

            }
            if ((((object)results.table).ToString()) == "trade")
                {
                    Savedata(results);
                }
                else if ((((object)results.table).ToString()) == "liquidation" && (((object)results.action).ToString()) == "insert")
                {
                    liquidationData(results);
                }
                else
                {
                    Console.WriteLine(" 消息类型无法识别:" + Message);
                }
          

            //Task.Run(() =>
            //{
            //Savedata(e.Data);
            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, Message, CommandEnum.RedisKey.bitmex, CommandEnum.RedisKey.bitmex);
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

            Console.WriteLine("bitmex websocket_Closed" + e.ToString());
            LogHelpers.Error(" bitmex websocket_Closed");
            //_webSocket.Close();
        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {
            LogHelpers.Info(" bitmex websocket_Error:" + e.ToString());
            LogHelpers.Error(" bitmex websocket_Error:" + e.ToString());
            Console.WriteLine("bitmex websocket_Error" + e.ToString());

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
                    if ( this._webSocket.State != WebSocketState.Connecting && this._webSocket.State != WebSocketState.Open)
                    {


                        //LogHelper.WriteLog(" Reconnect websocket WebSocketState:" + this._webSocket.State);
                        this._webSocket.Close();
                        this._webSocket.Open();
                        if (Sendata != null && Sendata != "")
                        {
                            SendMessage(Sendata);
                        }
                        LogHelpers.Info("bitmex正在重连");
                        Console.WriteLine("bitmex正在重连");
                    }
                    else
                    {
                        LogHelpers.Info("bitmex当前状态" + this._webSocket.State + "-----" + this._webSocket.State + DateTime.Now.ToString());
                        Console.WriteLine("bitmex当前状态" + this._webSocket.State + "-----" + this._webSocket.State + DateTime.Now.ToString());

                    }
                }
                catch (Exception ex)
                {

                    LogHelpers.Error("bitmex错误信息" + ex.Message.ToString());
                    Console.WriteLine("bitmex错误信息" + ex.Message.ToString());
                }
                System.Threading.Thread.Sleep(4000);
            } while (this._isRunning);
        }
        #endregion

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="message"></param>
        public void Savedata(dynamic results)
        {
            if (results != null && results != "")
            {
                try
                {
                    //var results = JsonConvert.DeserializeObject<dynamic>(message.ToString());
                    if ((object)results.data == null)
                    {
                        return;
                    }



                    var resultdata = (((object)results.data).ToString()).ToList<BitmexModel>();

                    if (resultdata != null && resultdata.Count > 0)
                    {
                        List<UPermanentFutures> list = new List<UPermanentFutures>();
                        foreach (var item in resultdata)
                        {
                            var times = Convert.ToDateTime(item.timestamp);
                            var key = times.ToString("g");
                            var keys = times.ToString("d");
                            item.actcualtime = key;
                            //RedisHelper.Pushdata(item.ToJson().ToString(), CommandEnum.RedisKey.bitmexRedis, keys+CommandEnum.RedisKey.bitmex);
                            UPermanentFutures model = new UPermanentFutures();
                            model = Mapping(item);

                            list.Add(model);

                            if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
                            {
                                RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
                            }
                        }
                        RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UPermanentFutures);
                        Task.Run(() =>
                        {
                            RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.PERPQueueList, list.ToJson());
                        });

                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("bitmex信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("bitmex信息转化错误，错误的信息源：" + results);
                    Console.WriteLine("bitmex信息转化错误，错误信息：" + e.Message.ToString());
                    Console.WriteLine("bitmex信息转化错误，错误的信息源：" + results);
                    //throw e;
                }


            }
            else
            {
                LogHelpers.Info("bitmex信息为空，无法保存");
                Console.WriteLine("bitmex信息为空，无法保存");
            }
        }

        /// <summary>
        /// 处理强平数据
        /// </summary>
        /// <param name="results"></param>
        public void liquidationData(dynamic results)
        {
            if (results != null)
            {
                try
                {
                    //var results = JsonConvert.DeserializeObject<dynamic>(message.ToString());
                    //if ((object)results.data == null)
                    //{
                    //    return;
                    //}



                    var resultdata = (((object)results.data).ToString()).ToList<BitmexLQ>();

                    if (resultdata != null && resultdata.Count > 0)
                    {
                        List<Liquidation> list = new List<Liquidation>();
                        foreach (var item in resultdata)
                        {
                            //var times = Convert.ToDateTime(item.timestamp);
                            //var key = times.ToString("g");
                            //var keys = times.ToString("d");
                            item.actcualtime = DateTime.Now.ToString(); ;
             
                            //UPermanentFutures model = new UPermanentFutures();
                            //model = Mapping(item);


                            Liquidation LQ = new Liquidation();
                            LQ.types = "BITMEX USD永续合约";
                            LQ.Unit = "USDT";
                            LQ.utctime= DateTime.Now.ToString();
                            LQ.uuuid = item.orderID;
                            LQ.kinds = CommandEnum.RedisKey.PERP;

                            if (item.symbol.ToUpper().Contains("BT"))
                            {
                                LQ.market = "BTC";
                                LQ.pair = "BTC";
                                LQ.contractcode= "BTC";

                            }
                            else
                            {
                                LQ.market = item.symbol;
                                LQ.pair = item.symbol;
                                LQ.contractcode = item.symbol.Replace("USD","");
                            }
                            LQ.qty = item.leavesQty.ToString();
                            LQ.side = item.side;
                            LQ.SYS_CreateDate = DateTime.Now.ToString();
                            LQ.timestamp = DateTime.Now.ToString();
                            LQ.times = DateTime.Now.ToString();
                            LQ.vol = item.leavesQty.ToString(); ;
                            LQ.exchange = CommandEnum.RedisKey.bitmex;
                            list.Add(LQ);

                            //if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
                            //{
                            //    RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                            //}

                            //if (LQ.vol != null && Convert.ToDecimal(LQ.vol) >= 1000000)
                            //{
                            //    RedisHelper.Pushdata(LQ.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                            //}


                            ////大单统计
                            //if (item.trade_turnover >= 100000)
                            //{
                            //    RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                            //}
                            //大单统计
                            if (Convert.ToDecimal(LQ.vol )>= 1000000)
                            {
                                RedisHelper.Pushdata(LQ.ToJson().ToString(), "11", CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                            }
                        }
                      
                        RedisHelper.Pushdata(list.ToJson().ToString(), "11", CommandEnum.RedisKey.liquidation + DateTime.Now.ToString("yyyy-MM-dd HH"));


                        RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.liquidationList, list.ToJson());

                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("bitmex爆仓信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("bitmex爆仓信息转化错误，错误的信息源：" + results);
                    Console.WriteLine("bitmex爆仓信息转化错误，错误信息：" + e.Message.ToString());
                    Console.WriteLine("bitmex爆仓信息转化错误，错误的信息源：" + results);
                    //throw e;
                }


            }
            else
            {
                LogHelpers.Info("bitmex爆仓信息转化错误，无法保存");
                Console.WriteLine("bitmex爆仓信息转化错误，无法保存");
            }

        }




     
        /// <summary>
        /// /bitmex
        /// BTC合约的数量为Usdt
        /// 其他合约为XBT 暂时未收录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UPermanentFutures Mapping(BitmexModel model)
        {
            UPermanentFutures o = new UPermanentFutures();
            o.uuuid = model.trdMatchID;
            o.cross_seq = model.trdMatchID;
            o.exchange = CommandEnum.RedisKey.bitmex;

            o.Unit = "USDT";
            o.types = "USDT合约:" + model.symbol.ToUpper().Replace("USD", "");

            if (model.symbol.ToUpper().Contains("BT"))
            {
                o.market = "BTC";
                o.pair = "BTC";
                o.contractcode = "BTC";
            }
            else
            {
                o.market = model.symbol;
                o.pair = model.symbol;
                o.contractcode = model.symbol.Replace("USD", "");
            }
            o.price = model.price;
            o.qty = model.size;
            o.vol = model.size;
            o.side = model.side;
            o.times = model.actcualtime;
            o.timestamp = model.timestamp;
            o.utctime = model.actcualtime;
            return o;
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string Message)
        {

            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                this._webSocket.Send(Message);
            }

        }
        public string GetSendData()
        {
            return "";
        }
        public void Dispose()
        {
            LogHelpers.Error("bitmex断开连接异常" + DateTime.Now.ToString());
            LogHelpers.Info("bitmex断开连接异常" + DateTime.Now.ToString());
            Console.WriteLine("bitmex断开连接出错异常" + DateTime.Now.ToString());

            //try
            //{
            //    LogHelpers.Info("bitmex断开连接"  );
            //    Console.WriteLine("bitmex断开连接出错" );

            //}
            //catch(Exception e)
            //{
            //    LogHelpers.Info("bitmex断开连接" + e.ToString()); 
            //    Console.WriteLine("bitmex断开连接出错"+e.ToString());
            //}

        }
    }
}



///// <summary>
///// 备份保存信息
///// </summary>
///// <param name="message"></param>
//public void Savedatas(string message)
//{
//    if (message != null && message != "")
//    {
//        try
//        {
//            var results = JsonConvert.DeserializeObject<dynamic>(message.ToString());
//            if ((object)results.data == null)
//            {
//                return;
//            }



//            var resultdata = (((object)results.data).ToString()).ToList<BitmexModel>();

//            if (resultdata != null && resultdata.Count > 0)
//            {
//                List<UPermanentFutures> list = new List<UPermanentFutures>();
//                foreach (var item in resultdata)
//                {
//                    var times = Convert.ToDateTime(item.timestamp);
//                    var key = times.ToString("g");
//                    var keys = times.ToString("d");
//                    item.actcualtime = key;
//                    //RedisHelper.Pushdata(item.ToJson().ToString(), CommandEnum.RedisKey.bitmexRedis, keys+CommandEnum.RedisKey.bitmex);
//                    UPermanentFutures model = new UPermanentFutures();
//                    model = Mapping(item);

//                    list.Add(model);

//                    if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
//                    {
//                        RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.MaxOrder + DateTime.Now.ToString("yyyy-MM-dd"));
//                    }
//                }
//                RedisHelper.Pushdata(list.ToJson(), CommandEnum.RedisKey.bitmexRedis, DateTime.Now.ToString("yyyy-MM-dd HH") + CommandEnum.RedisKey.UPermanentFutures);


//            }

//        }
//        catch (Exception e)
//        {
//            LogHelpers.Error("bitmex信息转化错误，错误信息：" + e.Message.ToString());
//            LogHelpers.Error("bitmex信息转化错误，错误的信息源：" + message);
//            Console.WriteLine("bitmex信息转化错误，错误信息：" + e.Message.ToString());
//            Console.WriteLine("bitmex信息转化错误，错误的信息源：" + message);
//            //throw e;
//        }


//    }
//    else
//    {
//        LogHelpers.Info("bitmex信息为空，无法保存");
//        Console.WriteLine("bitmex信息为空，无法保存");
//    }
//}