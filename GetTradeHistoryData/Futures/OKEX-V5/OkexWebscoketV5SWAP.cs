using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using static GetTradeHistoryData.CommandEnum;

namespace GetTradeHistoryData
{
    /// <summary>
    /// Responsible to handle MBP data from WebSocket
    /// </summary>
    public class OkexWebscoketV5SWAP : WebSocketClientBase
    {
        private List<OkexSwapv5> symbollist;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">websockethost</param>
        public OkexWebscoketV5SWAP(string host, string sendmessage, bool isporxy) : base(host, sendmessage, isporxy)
        {
            symbollist= CommandEnum.OkexMessage.GetSwapContract_size_V5("SWAP");
            this.OnResponseReceived += MessageOperation;
            this.OnGetmessage += SendMessages;
        }



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
                                model = USDTMapping(item, pair, coin);
                            }
                            else
                            {
                                model = Mapping(item, pair, coin);
                            }
                            model.kind = CommandEnum.RedisKey.PERP;
                            list.Add(model);                            
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

       
            o.side = model.side;
            o.times = model.actcualtime;
            o.timestamp = model.timestamp;
            o.utctime = model.actcualtime;
    

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
 
            o.side = model.side;
            o.times = model.actcualtime;
            o.timestamp = model.timestamp;
            o.utctime = model.actcualtime;
 

            return o;
        }



        public decimal swich(string coin)
        {
            decimal salary = 0;

            var result = this.symbollist.Where(p => p.uly == coin).FirstOrDefault();
            if (result != null)
            {
                salary = Convert.ToDecimal(result.ctValCcy);
            }
            else
            {
                salary = 0.00001m;
                LogHelpers.Info("Okex 交割合约价值信息为空，价值计算错误！");
                Console.WriteLine("Okex 交割合约价值信息为空，价值计算错误！");
            }


            return salary;

        }






        public void MessageOperation(string ts)
        {
            var results = JsonConvert.DeserializeObject<dynamic>(ts);
            Savedata(results);
            Console.WriteLine(results.tick);


        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Message"></param>
        public void SendMessagecopy()
        {

            string list = string.Empty;
            foreach (var item in this.symbollist)
            {
                string topic = item.uly;
                list += "\"swap/trade:" + topic + "\",";
            }
            list = list.Remove(list.Length - 1);
            var opss = "{ \"op\": \"subscribe\", \"args\":  [\"channel:books\",\"instId\":" + list + "]}";




            var ops = "{ \"op\": \"subscribe\", \"args\": [\"channel:books\",\"instId\":" + "BTC-USD-210625" + "]}";


            if (this._WebSocket != null && _WebSocket.ReadyState == WebSocketState.Open)
            {
                this._WebSocket.Send(ops);
            }
 

        }

        public void SendMessages()
        {
          
            string list = string.Empty;
            foreach (var item in this.symbollist)
            {
                string topic = item.uly;
                list += "{\"channel\": \"trades\",\"instId\": \""+item.uly + "\"},";
            }
            list = list.Remove(list.Length - 1);
            var ops = "{ \"op\": \"subscribe\", \"args\":  [ "+ list +" ]}";


            if (this._WebSocket != null && _WebSocket.ReadyState == WebSocketState.Open)
            {
                this._WebSocket.Send(ops);
            }


        }

    }




}
