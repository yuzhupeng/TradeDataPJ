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
    public class bitmexWebscoket : IDisposable
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

        List<Instrument> symbollis;



        public string GetUrls()
        {
            symbollis = BitmexMarket.GetSymbolFutureList().Where(p=>p.QuoteCurrency!="XBT").ToList();
            string pairParams ="subscribe=";
            if (symbollis != null)
            {
                if (symbollis.Count() > 0)
                {
                    foreach (var pair in symbollis)
                    {
                        pairParams += $"trade:{pair.Symbol},liquidation:{pair.Symbol},";
                    }

                    pairParams = pairParams.Remove(pairParams.Length - 1);
                }
            }
            return pairParams;     
        }



        public bitmexWebscoket(string url)
        {
            LogHelpers = LogHelper.CreateInstance();
            if (url != null && url != "")
            {
                ServerPath = url;
            }
            else
            {
                ServerPath = "wss://www.bitmex.com/realtime?subscribe=trade:XBTUSD,trade:LTCUSD,trade:ETHUSD,liquidation:XBTUSD,liquidation:ETHUSD,liquidation:LTCUSD";
            }
            ServerPath = "wss://www.bitmex.com/realtime?";
            string uls = GetUrls();
            ServerPath = ServerPath + uls;

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
        public void WebSocket_MessageReceived(object sender, MessageEventArgs e)
        {

            //Console.WriteLine(" Received:" + e.Data);
            //LogHelpers.Info(" Received:" + e.Data);

            dynamic results = JsonConvert.DeserializeObject<dynamic>(e.Data);
            if ((object)results.table == null)
            {
                return;
            }
 
            if ((((object)results.table).ToString()) == "trade")
            {
                //Console.WriteLine(((object)results.table).ToString());
                Savedata(results);

            }
            else if ((((object)results.table).ToString()) == "liquidation" && (((object)results.action).ToString())== "insert")
            {
                liquidationData(results);
            }
            else
            {
                Console.WriteLine(" 消息类型无法识别:" + e.Data);
            }


            //Task.Run(() =>
            //{
            //Savedata(e.Data);
            Task.Run(() =>
            {
                CommandEnum.WriteSaveMessage(DateTime.Now, e.Data, CommandEnum.RedisKey.bitmex, CommandEnum.RedisKey.bitmex);
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
                    if (this._webSocket.IsAlive != true || this._webSocket.ReadyState != WebSocketState.Open)
                    {


                        //LogHelper.WriteLog(" Reconnect websocket WebSocketState:" + this._webSocket.State);
                        this._webSocket.Close();
                        this._webSocket.Connect();
                        if (Sendata != null && Sendata != "")
                        {
                            SendMessage(Sendata);
                        }
                        LogHelpers.Info("bitmex正在重连");
                        Console.WriteLine("bitmex正在重连");
                    }
                    else
                    {
                        LogHelpers.Info("bitmex当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());
                        Console.WriteLine("bitmex当前状态" + this._webSocket.ReadyState + "-----" + this._webSocket.IsAlive + DateTime.Now.ToString());

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
            if (results != null )
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
                        List<string> maxlist = new List<string>();
                        string types = "";
                        types = resultdata.FirstOrDefault().symbol.Contains("21") ? CommandEnum.RedisKey.DELIVERY : CommandEnum.RedisKey.PERP;
                        foreach (var item in resultdata)
                        {
                            var times = Convert.ToDateTime(item.timestamp);
                            var key = times.ToString("g");
                            item.actcualtime = key;
                            UPermanentFutures model = new UPermanentFutures();
                            model = Mapping(item);
                            list.Add(model);                          
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

                            item.actcualtime = DateTime.Now.ToString(); ;
                            Liquidation LQ = new Liquidation();
                            LQ.types = "BITMEX USD永续合约";
                            LQ.Unit = "USDT";
                            LQ.utctime = DateTime.Now.ToString();
                            LQ.uuuid = item.orderID;
                            //LQ.kinds = CommandEnum.RedisKey.PERP;
                            LQ.kinds = item.symbol.Contains("21") ? CommandEnum.RedisKey.DELIVERY : CommandEnum.RedisKey.PERP;

                            LQ.contractcode = symbollis.FirstOrDefault(p => p.Symbol == item.symbol).RootSymbol;

                            LQ.pair = item.symbol;
                            LQ.market = item.symbol;

                            if (LQ.contractcode == "XBT")
                            {
                                //LQ.pair = "BTC";
                                LQ.contractcode = "BTC";
                            }

                            //if (item.symbol.ToUpper().Contains("BT"))
                            //{
                            //    //LQ.market = "BTC";
                            //    LQ.pair = "BTC";
                            //    LQ.contractcode = "BTC";
                            //}
                            //else
                            //{
                            //    //LQ.market = item.symbol;
                            //    LQ.pair = item.symbol;
                            //    LQ.contractcode = item.symbol.Replace("USD", "");
                        //}

                            LQ.market = item.symbol;
                            LQ.qty = item.leavesQty.ToString();
                            LQ.side = item.side;
                            LQ.SYS_CreateDate = DateTime.Now.ToString();
                            LQ.timestamp = DateTime.Now.ToString();
                            LQ.times = DateTime.Now.ToString();
                            LQ.vol = item.leavesQty.ToString(); 
                            LQ.exchange = CommandEnum.RedisKey.bitmex;
                            LQ.price = item.price.ToString();
                            LQ.amount = item.leavesQty.ToString();
                            list.Add(LQ);

                        
                            //大单统计
                            if (Convert.ToDecimal(LQ.vol) >= 1000000)
                            {
                                RedisHelper.Pushdata(LQ.ToJson().ToString(), "11", CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                            }
                        }

                        RedisHelper.Pushdata(list.ToJson().ToString(), "11", CommandEnum.RedisKey.liquidation + DateTime.Now.ToString("yyyy-MM-dd"));

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

            o.contractcode = symbollis.FirstOrDefault(p => p.Symbol == model.symbol).RootSymbol;

            o.pair = model.symbol;
            o.market = model.symbol;

            if (o.contractcode == "XBT")
            {
                //o.pair = "BTC";
                o.contractcode = "BTC";
            }


            //if (model.symbol.ToUpper().Contains("BT"))
            //{
            //    //o.market = "BTC";
            //    o.pair = "BTC";
            //    o.contractcode = "BTC";
            //}
            //else
            //{
            //    //o.market = model.symbol;
            //    //o.pair = model.symbol;
            //    //o.contractcode = model.symbol.Replace("USD", "");
            //}

        
            o.price = model.price;
            o.qty = (Convert.ToDecimal(model.size) / Convert.ToDecimal(model.price)).ToString();
            o.vol = model.size;
            o.side = model.side;
            o.times = model.actcualtime;
            o.timestamp = model.timestamp;
            o.utctime = model.actcualtime;
            o.kind = model.symbol.Contains("21") ? CommandEnum.RedisKey.DELIVERY : CommandEnum.RedisKey.PERP;

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