 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Reflection;
using EmitMapper.Mappers;
using EmitMapper;
using EmitMapper.MappingConfiguration;

namespace CoinWin.DataGeneration
{
    /// <summary>
    /// 对象工具库
    /// </summary>
    public class ObjectUtil
    {
        /// <summary>
        /// 屬性映射
        /// </summary>
        /// <param name="src">原對象</param>
        /// <param name="target">目標對象</param>
        public static void MapTo(object src, object target)
        {
            try
            {
                if (src == null || target == null) return;
                ObjectsMapperBaseImpl mapperImpl = ObjectMapperManager.DefaultInstance.GetMapperImpl(src.GetType(), target.GetType(), new DefaultMapConfig());
                mapperImpl.Map(src, target, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("转化类出现异常，异常信息：" + e.Message.ToString());
                LogHelper.WriteLog(typeof(DownExchangeData), "转化类出现异常，异常信息：" + e.Message.ToString());
            }
        

        }
        /// <summary>
        /// 忽略某些屬性然後屬性映射
        /// </summary>
        /// <param name="src">原對象</param>
        /// <param name="target">目標對象</param>
        /// <param name="ignoreProperties">忽略的屬性</param>
        public static void MapToIgnore(object src, object target, params string[] ignoreProperties)
        {
            if (src == null || target == null) return;
            if (ignoreProperties != null && ignoreProperties.Length > 0)
            {
                ObjectsMapperBaseImpl mapperImpl = ObjectMapperManager.DefaultInstance.GetMapperImpl(src.GetType(), target.GetType(),
                                                   new DefaultMapConfig().IgnoreMembers(src.GetType(), target.GetType(), ignoreProperties));
                mapperImpl.Map(src, target, null);
            }
            else
            {
                MapTo(src, target);
            }
        }
        /// <summary>
        /// 忽略系统自动处理的属性，将原对象的属性映射到目标属性上，主要用于将dto对象转换成domain对象时调用
        /// </summary>
        /// <param name="src">原对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="callback">提供回调函数，用于检测被忽略的属性</param>
        public static void MapToIgnoreAuto(object src, object target, Action<ReadOnlyCollection<string>> callback = null)
        {
            if (src == null || target == null) return;
            var ignoreProperties = GetAutoProperties(target);
            if (callback != null)
            {
                callback(ignoreProperties.ToList().AsReadOnly());
            }
            MapToIgnore(src, target, ignoreProperties.ToArray());
        }

        /// <summary>
        /// 內存鏡像
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Dump(object obj, int length = 15)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                stringBuilder.AppendFormat("{0}\t={1}\r\n", FixLength(properties[i].Name, length), properties[i].GetValue(obj, null));
            }
            return stringBuilder.ToString();
        }

        #region internal
        /// <summary>
        /// 填充字符串長度
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="length">長度</param>
        /// <returns></returns>
        private static string FixLength(string str, int length)
        {
            for (int i = str.Length; i < length; i++)
            {
                str += " ";
            }
            return str;
        }

        /// <summary>
        /// 獲取model拷貝時可以忽略的屬性，通常為領域對象關聯關係的屬性
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetAutoProperties(object model)
        {
            return model.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttributes(typeof(SystemAutoAttribute), true).Any()).Select(m => m.Name);
        }
        #endregion
    }

    /// <summary>
    /// 由系统自动处理的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SystemAutoAttribute : Attribute
    {
    }
}
