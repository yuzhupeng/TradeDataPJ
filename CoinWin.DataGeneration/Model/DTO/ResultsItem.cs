﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    /// <summary>
    /// api获取数据DTO
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class ResultsItemArry
    {
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(0)]
        public string time { get; set; }
        /// <summary>
        /// 
        [ArrayProperty(2)]
        public string close { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(1)]
        public int count_buy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(3)]
        public int count_sell { get; set; }
        /// <summary>
        /// 
        /// </summ
        [ArrayProperty(8)]
        public string exchange { get; set; }
        /// <summary>
        /// 
        [ArrayProperty(4)]
        public double high { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(5)]
        public decimal  liquidation_buy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(7)]
        public decimal liquidation_sell { get; set; }
        /// <summary>
        /// 
        [ArrayProperty(6)]
        public decimal low { get; set; }
        /// <summary>
        /// 
        [ArrayProperty(9)]
        public decimal open { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string pair { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? vol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(10)]
        public decimal vol_buy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(11)]
        public decimal vol_sell { get; set; }
    }

    public class ExchageDataDTOArry
    {
        /// <summary>
        /// 
        /// </summary>
        public string format { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ResultsItemArry> results { get; set; }
    }



    //
    // 摘要:
    //     Converter for arrays to properties
    public class ArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(JToken))
            {
                return JToken.Load(reader);
            }

            object result = Activator.CreateInstance(objectType);
            JArray arr = JArray.Load(reader);
            return ParseObject(arr, result, objectType);
        }

        private static object ParseObject(JArray arr, object result, Type objectType)
        {
            PropertyInfo[] properties = objectType.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                ArrayPropertyAttribute arrayPropertyAttribute = (ArrayPropertyAttribute)propertyInfo.GetCustomAttribute(typeof(ArrayPropertyAttribute));
                if (arrayPropertyAttribute == null || arrayPropertyAttribute.Index >= arr.Count)
                {
                    continue;
                }

                if (propertyInfo.PropertyType.BaseType == typeof(Array))
                {
                    Type elementType = propertyInfo.PropertyType.GetElementType();
                    JArray jArray = (JArray)arr[arrayPropertyAttribute.Index];
                    int num = 0;
                    if (jArray.Count == 0)
                    {
                        IList value = (IList)Activator.CreateInstance(propertyInfo.PropertyType, new int[1]);
                        propertyInfo.SetValue(result, value);
                    }
                    else if (jArray[0].Type == JTokenType.Array)
                    {
                        IList list = (IList)Activator.CreateInstance(propertyInfo.PropertyType, new int[1]
                        {
                            jArray.Count
                        });
                        foreach (JToken item in jArray)
                        {
                            object result2 = Activator.CreateInstance(elementType);
                            list[num] = ParseObject((JArray)item, result2, elementType);
                            num++;
                        }

                        propertyInfo.SetValue(result, list);
                    }
                    else
                    {
                        IList list2 = (IList)Activator.CreateInstance(propertyInfo.PropertyType, new int[1]
                        {
                            1
                        });
                        object result3 = Activator.CreateInstance(elementType);
                        list2[0] = ParseObject(jArray, result3, elementType);
                        propertyInfo.SetValue(result, list2);
                    }

                    continue;
                }

                JsonConverterAttribute jsonConverterAttribute = ((JsonConverterAttribute)propertyInfo.GetCustomAttribute(typeof(JsonConverterAttribute))) ?? ((JsonConverterAttribute)propertyInfo.PropertyType.GetCustomAttribute(typeof(JsonConverterAttribute)));
                JsonConversionAttribute jsonConversionAttribute = ((JsonConversionAttribute)propertyInfo.GetCustomAttribute(typeof(JsonConversionAttribute))) ?? ((JsonConversionAttribute)propertyInfo.PropertyType.GetCustomAttribute(typeof(JsonConversionAttribute)));
                object obj = (jsonConverterAttribute != null) ? arr[arrayPropertyAttribute.Index].ToObject(propertyInfo.PropertyType, new JsonSerializer
                {
                    Converters =
                    {
                        (JsonConverter)Activator.CreateInstance(jsonConverterAttribute.ConverterType)
                    }
                }) : ((jsonConversionAttribute == null) ? arr[arrayPropertyAttribute.Index] : arr[arrayPropertyAttribute.Index].ToObject(propertyInfo.PropertyType));
                if (obj != null && propertyInfo.PropertyType.IsInstanceOfType(obj))
                {
                    propertyInfo.SetValue(result, obj);
                    continue;
                }

                JToken jToken = obj as JToken;
                if (jToken != null && jToken.Type == JTokenType.Null)
                {
                    obj = null;
                }

                if ((propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?)) && (obj?.ToString().Contains("e") ?? false))
                {
                    if (decimal.TryParse(obj.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result4))
                    {
                        propertyInfo.SetValue(result, result4);
                    }
                }
                else
                {
                    propertyInfo.SetValue(result, (obj == null) ? null : Convert.ChangeType(obj, propertyInfo.PropertyType));
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            writer.WriteStartArray();
            PropertyInfo[] properties = value.GetType().GetProperties();
            IOrderedEnumerable<PropertyInfo> orderedEnumerable = properties.OrderBy((PropertyInfo p) => p.GetCustomAttribute<ArrayPropertyAttribute>()?.Index);
            int i = -1;
            foreach (PropertyInfo item in orderedEnumerable)
            {
                ArrayPropertyAttribute customAttribute = item.GetCustomAttribute<ArrayPropertyAttribute>();
                if (customAttribute != null && customAttribute.Index != i)
                {
                    for (; customAttribute.Index != i + 1; i++)
                    {
                        writer.WriteValue((string)null);
                    }

                    i = customAttribute.Index;
                    JsonConverterAttribute jsonConverterAttribute = (JsonConverterAttribute)item.GetCustomAttribute(typeof(JsonConverterAttribute));
                    if (jsonConverterAttribute != null)
                    {
                        writer.WriteRawValue(JsonConvert.SerializeObject(item.GetValue(value), (JsonConverter)Activator.CreateInstance(jsonConverterAttribute.ConverterType)));
                    }
                    else if (!IsSimple(item.PropertyType))
                    {
                        serializer.Serialize(writer, item.GetValue(value));
                    }
                    else
                    {
                        writer.WriteValue(item.GetValue(value));
                    }
                }
            }

            writer.WriteEndArray();
        }

        private static bool IsSimple(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsSimple(type.GetGenericArguments()[0]);
            }

            return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal);
        }
    }
    //
    // 摘要:
    //     Mark property as an index in the array
    public class ArrayPropertyAttribute : Attribute
    {
        //
        // 摘要:
        //     The index in the array
        public int Index
        {
            get;
        }

        //
        // 摘要:
        //     ctor
        //
        // 参数:
        //   index:
        public ArrayPropertyAttribute(int index)
        {
            Index = index;
        }
    }
    //
    // 摘要:
    //     Used for conversion in ArrayConverter
    public class JsonConversionAttribute : Attribute
    {
    }





}
