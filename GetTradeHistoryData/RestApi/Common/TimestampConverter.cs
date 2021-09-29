using Newtonsoft.Json;
using System;

namespace GetTradeHistoryData
{
    //
    // 摘要:
    //     converter for milliseconds to datetime
    public class TimestampConverter : JsonConverter
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

            long num = long.Parse(reader.Value!.ToString());
            //  return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(num);
            var times = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddMilliseconds(num*1000).AddHours(8);
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