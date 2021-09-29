using Newtonsoft.Json;
using System;

namespace GetTradeHistoryData
{
    //
    // 摘要:
    //     converter for milliseconds to datetime
    public class BJTimestampConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            
            return objectType == typeof(DateTime);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }
            //long num = long.Parse(reader.Value!.ToString());
            //var times =    new DateTime((num * 10000) + 621355968000000000);
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(reader.Value!.ToString() + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            var times =dtStart.Add(toNow);
            return times;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue((DateTime?)null);
            }
            else
            {
                writer.WriteValue((long)Math.Round(((DateTime)value - new DateTime(1970, 1, 1)).TotalMilliseconds));
            }
        }
    }
}