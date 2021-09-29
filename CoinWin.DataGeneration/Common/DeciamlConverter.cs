using Newtonsoft.Json;
using System;
using System.Globalization;

namespace CoinWin.DataGeneration
{
    //
    // 摘要:
    //     converter for milliseconds to datetime
    public class DeciamlConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return 0;
            }

            decimal.TryParse(reader.Value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result4);
                
            return result4;
              
            

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue((decimal?)null);
            }
            else
            {
                writer.WriteValue((decimal?)null);
            }
        }
    }
}