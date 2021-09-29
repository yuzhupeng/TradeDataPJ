using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace GetTradeHistoryData
{
    /// <summary>
    /// Newtonsoft.Json 扩展库
    /// </summary>
    public static class JsonExt
    {
        public static object ToJson(this string Json)
        {
            return Json == null ? null : JsonConvert.DeserializeObject(Json);
        }
        public static T ToObject<T>(this string Json)
        {
            return Json == null ? default(T) : JsonConvert.DeserializeObject<T>(Json.Replace("&nbsp;", ""));
        }
        public static List<T> ToList<T>(this string Json)
        {

            var serializerSettings = new JsonSerializerSettings();

            // 设置为驼峰命名
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;              
            return Json == null ? null : JsonConvert.DeserializeObject<List<T>>(Json.Replace("&nbsp;", ""), serializerSettings);
        }
        public static string ToJson(this object obj)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                // 设置为驼峰命名
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            //支持DataTable轉換
            serializerSettings.Converters.Add(new DataTableConverter());
            return JsonConvert.SerializeObject(obj, serializerSettings);
        }
    }
}
