using log4net;
using Newtonsoft.Json;
using QPP.Core;
using SuperSocket.ClientEngine.Proxy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace GetTradeHistoryData
{
    /// <summary>
    /// bybit   
    /// </summary>
    public class bybitUSDWebscoketCore : IDisposable
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
        public string ServerPath
        {
            get;
            set;

        }

        /// <summary>
        /// 重试发出的消息
        /// </summary>
        public string Sendata { get; set; }

        ILog LogHelpers;

        string symbol;

        /// <summary>
        /// bybit usd爆仓数据合约
        /// </summary>
        /// <param name="url"></param>
        /// <param name="code">proxy or  no proxy</param>
        public bybitUSDWebscoketCore(string url,string code)
        {
            LogHelpers = LogHelper.CreateInstance();

            this.symbol = code;

            if (url != null && url != "")
            {
                ServerPath = url;
            }
            //if (code == "proxy")
            //{
            //    ServerPath = "wss://stream.bytick.com/realtime";
            //}
            //else
            //{
            //    ServerPath = "wss://stream.bybit.com/realtime";
            //}
        


            this._webSocket = new WebSocket(ServerPath);
            this._webSocket.OnOpen += WebSocket_Opened;
            this._webSocket.OnClose += WebSocket_Closed;
            this._webSocket.OnMessage += (o, s) => WebSocket_MessageReceived(s.Data);
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
                Console.WriteLine("bybit 爆仓数据 连接失败:" + ex.ToString());
                LogHelpers.Error("bybit 爆仓数据 连接失败:" + ex.ToString());
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
            LogHelpers.Error("bybit 爆仓数据 "+Message.ToString());
            LogHelpers.Info("bybit 爆仓数据 " + Message.ToString());
            var results = JsonConvert.DeserializeObject<dynamic>(Message);
            if (((object)results.data) == null)
            {
                Console.WriteLine("获取bybit爆仓信息为空！");
             /*   string demodatas = "{\"topic\":\"liquidation.BTCUSD\",\"data\":{\"symbol\":\"BTCUSD\",\"side\":\"Sell\",\"price\":\"54524.00\",\"qty\":\"839\",\"time\":1633656435895}}"*/;
                return;
            }
           
            SaveforceOrder(results);
           


        }
        /// <summary>
        /// Socket关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Closed(object sender, EventArgs e)
        {
            if (symbol == "USD")
            {
                Console.WriteLine("bybit USD爆仓数据 WebSocket_Closed" + e.ToString());
                LogHelpers.Error("bybit USD爆仓数据 WebSocket_Closed");
            }
            else
            {
                //ServerPath = "wss://dstream.bybit.com/ws/!forceOrder@arr";//爆仓数据
                Console.WriteLine("bybit USDT爆仓数据 WebSocket_Closed" + e.ToString());
                LogHelpers.Error("bybit USDT爆仓数据 WebSocket_Closed");
            }

     


        }
        /// <summary>
        /// Socket报错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Error(object sender, EventArgs e)
        {

            if (symbol == "USD")
            {
            
                Console.WriteLine("bybit 爆仓数据 websocket_Error" + e.ToString());
                LogHelpers.Error("bybit 爆仓数据 websocket_Error");
            }
            else
            {
             
                Console.WriteLine("bybit 爆仓数据 websocket_Error" + e.ToString());
                LogHelpers.Error("bybit 爆仓数据 websocket_Error");
            }
 
        }
        /// <summary>
        /// Socket打开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WebSocket_Opened(object sender, EventArgs e)
        {


            if (symbol == "USD")
            {
               
                Console.WriteLine("bybit 爆仓数据 WebSocket_Opened" + e.ToString());
                LogHelpers.Error("bybit 爆仓数据 WebSocket_Opened");
            }
            else
            {
                //ServerPath = "wss://dstream.bybit.com/ws/!forceOrder@arr";//爆仓数据
                Console.WriteLine("bybit 爆仓数据 WebSocket_Opened" + e.ToString());
                LogHelpers.Error("bybit 爆仓数据 WebSocket_Opened");
            }
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
                        LogHelpers.Info("bybit 爆仓数据 正在重连");
                        Console.WriteLine("bybit 爆仓数据 正在重连");

                        if (symbol == "USD")
                        {
                            //ServerPath = "wss://fstream.bybit.com/ws/!forceOrder@arr";//爆仓数据
                            LogHelpers.Info("bybit USD爆仓数据 正在重连");
                            Console.WriteLine("bybit USD 爆仓数据 正在重连");
                        }
                        else
                        {
                            //ServerPath = "wss://dstream.bybit.com/ws/!forceOrder@arr";//爆仓数据
                            LogHelpers.Info("bybit USDT爆仓数据 正在重连");
                            Console.WriteLine("bybit USDT爆仓数据 正在重连");
                        }

                    }
                    else
                    {
                        
                        if (symbol == "USD")
                        {
                           
                            Console.WriteLine("bybit USD爆仓数据 度当前状态" + this._webSocket.ReadyState + "-----"   + DateTime.Now.ToString());
                        }
                        else
                        {
                             
                            Console.WriteLine("bybit USDT爆仓数据 度当前状态" + this._webSocket.ReadyState + "-----"   + DateTime.Now.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
       
                    if (symbol == "USD")
                    {
                        
                        Console.WriteLine("bybit USD爆仓数据 重连websocket_Error" + this._webSocket.ReadyState + "-----" + DateTime.Now.ToString());
                    }
                    else
                    {
                        
                        Console.WriteLine("bybit USDT爆仓数据 重连websocket_Error" + this._webSocket.ReadyState + "-----" + DateTime.Now.ToString());
                    }


                }
                System.Threading.Thread.Sleep(5000);
            } while (this._isRunning);
        }
        #endregion



       
 

        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="message"></param>
        public void SaveforceOrder(dynamic results)
        {
            if (results != null)
            {
                try
                {                    
                    var item = (((object)results.data).ToString()).ToObject<BybitLQData>();
                    if (item == null)
                    {
                        return;
                    }

                    List<Liquidation> list = new List<Liquidation>();
                    if (item != null)
                    {
 
                        var times = GZipDecompresser.GetTimeFromUnixTimestampthree(item.time.ToString());
                        var key = times.ToString("g");
                        var keys = times.ToString("d");
                        item.actcualtime = times;                 
                        Liquidation model = new Liquidation();
                          Mapping(item);

                        //list.Add(model);
                        //if (model.vol != null && Convert.ToDecimal(model.vol) >= 1000000)
                        //{
                        //    Console.WriteLine(model.ToJson());
                        //    RedisHelper.Pushdata(model.ToJson().ToString(), "11", CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
                        //}
                        //RedisHelper.Pushdata(list.ToJson().ToString(), "11", CommandEnum.RedisKey.liquidation + DateTime.Now.ToString("yyyy-MM-dd"));                     
                        //RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.liquidationList, list.ToJson());
                       
                    }

                }
                catch (Exception e)
                {
                    LogHelpers.Error("bybit 爆仓数据 信息转化错误，错误信息：" + e.Message.ToString());
                    LogHelpers.Error("bybit 爆仓数据 信息转化错误，错误的信息源：" + ((object)results).ToString());
                    Console.WriteLine("bybit 爆仓数据 信息转化错误，错误信息：" + e.Message.ToString());

                    if (symbol == "USD")
                    {
                        LogHelpers.Error("bybit USD爆仓数据 信息转化错误，错误信息：" + e.Message.ToString());
                        LogHelpers.Error("bybit USD爆仓数据 信息转化错误，错误的信息源：" + ((object)results).ToString());
                        Console.WriteLine("bybit USD爆仓数据 信息转化错误，错误信息：" + e.Message.ToString());
                    }
                    else
                    {
                        LogHelpers.Error("bybit USDt爆仓数据 信息转化错误，错误信息：" + e.Message.ToString());
                        LogHelpers.Error("bybit USDt爆仓数据 信息转化错误，错误的信息源：" + ((object)results).ToString());
                        Console.WriteLine("bybit USDt爆仓数据 信息转化错误，错误信息：" + e.Message.ToString());
                    }
                }

            }
            else
            {
               
                if (symbol == "USD")
                {
                    LogHelpers.Info("bybit 爆仓数据 信息为空，无法保存");
                    Console.WriteLine("bybit 爆仓数据 信息为空，无法保存");
                }
                else
                {
                    LogHelpers.Info("bybit 爆仓数据 信息为空，无法保存");
                    Console.WriteLine("bybit 爆仓数据 信息为空，无法保存");
                }
            }
        }


        /// <summary>
        /// 转化类
        /// </summary>
        /// <param name="model">
        /// 爆仓数据合约，单位为张，BTC 100美元  其他都为10美元
        /// </param>
        /// <returns></returns>
        public void Mapping(BybitLQData item)
        {
            Liquidation o = new Liquidation();
            o.amount = item.qty.ToString();
            o.timestamp = item.time.ToString();
            o.times = GZipDecompresser.GetTimeFromUnixTimestamps(item.time.ToString()).AddHours(8).ToString();
            o.Unit = "";
            o.utctime = GZipDecompresser.GetTimeFromUnixTimestamps(item.time.ToString()).ToString();

            if (item.symbol.ToUpper().Contains("USDT"))
            {
                o.vol = (item.price * item.qty).ToString();
            }
            else
            {
                o.vol = item.qty.ToString();
            }
            o.side = item.side;
            o.exchange = CommandEnum.RedisKey.bybit;
            o.pair = item.symbol;
            o.price = item.price.ToString();
            o.qty = item.qty.ToString();
            o.market = item.symbol;
            if (item.symbol.ToUpper().Contains("USDT"))
            {
                o.kinds = CommandEnum.RedisKey.PERP;
                o.types = item.symbol + "--币本位USDT永续";
                o.Unit = "币";
                o.contractcode = item.symbol.Replace("USDT", "");

            }
            else
            {
                o.kinds = CommandEnum.RedisKey.PERP;
                o.types = item.symbol + "--反向USD永续";
                o.Unit = "USDT";
                o.contractcode = item.symbol.Replace("USD", "");
            }
  
            //大单统计
            if (Convert.ToDecimal(o.vol) >= 1000000)
            {
                Console.WriteLine(o.ToJson());
                RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.BIGMaxLQ + DateTime.Now.ToString("yyyy-MM-dd"));
            }

            RedisHelper.Pushdata(o.ToJson().ToString(), "11", CommandEnum.RedisKey.liquidation + DateTime.Now.ToString("yyyy-MM-dd"));

            RedisMsgQueueHelper.EnQueue(CommandEnum.RedisKey.liquidationList, o.ToJson());


        }
        //"e": "aggTrade",  // 事件类型
        //"E": 123456789,   // 事件时间
        //"s": "BNBUSDT",    // 交易对
        //"a": "",
        //"p": "0.001",     // 成交价格
        //"q": "100",       // 成交笔数
        //"f": 100,         // 被归集的首个交易ID
        //"l": 105,         // 被归集的末次交易ID
        //"T": 123456785,   // 成交时间
        //"m": true         // 买方是否是做市方。如true，则此次成交是一个主动卖出单，否则是一个主动买入单。


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessage(string Message)
        {     
            string op = "{\"op\":\"subscribe\",\"args\":[\"liquidation\"]}";
            if (this.symbol.ToUpper() == "USDT")
            {
                string ops = "";
                var symbollist = CommandEnum.BybitData.GetContract_size().Where(p => p.quote_currency == "USDT").ToList();
                foreach (var item in symbollist)
                {
                    ops += $"liquidation.{item.base_currency},";
                }
                ops = ops.Remove(ops.Length - 1);
                string topic = "subscribe";
                  op = ($"{{ \"op\": \"{topic}\",\"args\": [\"{ops}\"]}}");
            }
            Sendata = op;
            if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open)
            {
                this._webSocket.Send(op);
            }
        }


        public string GetSendData(string symbols)
        {
            string[] pairs = symbols.Split(',');
            string pairParams = "";
            if (pairs != null)
            {
                if (pairs.Count() > 0)
                {
                    //foreach (var pair in pairs)
                    //{
                    //    pairParams += $"{pair.ToLower()}usd_210326@aggTrade/";
                    //}
                    //foreach (var pair in pairs)
                    //{
                    //    pairParams += $"{pair.ToLower()}usd_210625@aggTrade/";
                    //}
                    //foreach (var pair in pairs)
                    //{
                    //    pairParams += $"{pair.ToLower()}usd_PERP@aggTrade/";
                    //}

                    pairParams += "!forceOrder@arr";
                    //pairParams = pairParams.Remove(pairParams.Length - 1);
                }
            }

            return pairParams;
        }
        public void Dispose()
        {

 


            if (symbol == "USD")
            {
                //ServerPath = "wss://fstream.bybit.com/ws/!forceOrder@arr";//爆仓数据
                LogHelpers.Error("  bybit 爆仓数据 发送异常断开连接异常" + DateTime.Now.ToString());
                LogHelpers.Info("  bybit 爆仓数据 发送异常断开连接异常" + DateTime.Now.ToString());
                Console.WriteLine("  bybit 爆仓数据 约发送异常连接异常" + DateTime.Now.ToString());
            }
            else
            {
                //ServerPath = "wss://dstream.bybit.com/ws/!forceOrder@arr";//爆仓数据
                LogHelpers.Error("  bybit 爆仓数据 发送异常断开连接异常" + DateTime.Now.ToString());
                LogHelpers.Info("  bybit 爆仓数据 发送异常断开连接异常" + DateTime.Now.ToString());
                Console.WriteLine("  bybit 爆仓数据 约发送异常连接异常" + DateTime.Now.ToString());
            }
        }


    }
}
