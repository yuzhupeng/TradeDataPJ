using System;
using System.Collections.Generic;
using Newtonsoft.Json;
 

namespace GetTradeHistoryData 
{
    public class GetOpenInterest
    {
        public string status { get; set; }

        [JsonProperty("err_code", NullValueHandling = NullValueHandling.Ignore)]
        public string errorCode { get; set; }

        [JsonProperty("err_msg", NullValueHandling = NullValueHandling.Ignore)]
        public string errorMessage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Data> data { get; set; }

        public long ts { get; set; }

        public class Data
        {
            public string symbol { get; set; }

            [JsonProperty("contract_code")]
            public string contractCode { get; set; }

            public decimal amount { get; set; }

            public decimal volume { get; set; }

            public decimal value { get; set; }

            public decimal trade_amount { get; set; }
            public decimal trade_volume { get; set; }
            public decimal trade_turnover { get; set; }

            public decimal avgvalue 
            { get
                {
                    if (trade_volume == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return trade_turnover / trade_volume;
                    }
                } 
            }

        }

    }
}
//返回参数
//参数名称	是否必须	类型	描述	取值范围
//status	true	string	请求处理结果	"ok" , "error"
//<data>	true	object array		
//symbol	true	string	品种代码	"BTC", "ETH" ...
//contract_code	true	string	合约代码	"BTC-USDT" ...
//amount	true	decimal	持仓量(币)，单边数量
//volume	true	decimal	持仓量(张)，单边数量
//value	true	decimal	总持仓额（单位为合约的计价币种，如USDT）	
//trade_amount	true	decimal	最近24小时成交量（币）（当前时间-24小时）, 值是买卖双边之和	
//trade_volume	true	decimal	最近24小时成交量（张）（当前时间-24小时）, 值是买卖双边之和	
//trade_turnover	true	decimal	最近24小时成交额 （当前时间-24小时）,值是买卖双边之和	
//</data>				
//ts	true	long	响应生成时间点，单位：毫秒	
//备注
//持仓量（币）= 持仓量（张）*合约面值
//总持仓额 = 持仓量（张）* 合约面值 * 最新价