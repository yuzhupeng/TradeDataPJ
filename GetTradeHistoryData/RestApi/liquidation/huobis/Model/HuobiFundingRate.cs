using System;
using System.Collections.Generic;
using System.Text;

namespace GetTradeHistoryData
{
    /// <summary>
    /// 费率
    /// </summary>
   public class HuobiFundingRate
    {
    public string estimated_rate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string funding_rate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string contract_code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string symbol { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string fee_asset { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string funding_time { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string next_funding_time { get; set; }
}

}
//返回参数
//参数名称	是否必须	类型	描述	取值范围
//status	true	string	请求处理结果	"ok" , "error"
//ts	true	long	响应生成时间点，单位：毫秒	
//<data>	true	object array		
//symbol	true	string	品种代码	
//contract_code	true	string	合约代码	"BTC-USD" ...
//fee_asset	true	string	资金费币种	"BTC","ETH"...
//funding_time	true	string	当期资金费率时间（毫秒）	
//funding_rate	true	string	当期资金费率	
//estimated_rate	true	string	下一期预测资金费率	
//next_funding_time	true	string	下一期资金费率时间（毫秒）	
//</data>	