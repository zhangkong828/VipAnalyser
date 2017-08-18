using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace VipAnalyser.Common
{
    public class CommonCla
    {
       
        /// <summary>
        /// 检测是否有中文字符
        /// </summary>
        public static bool IsHasCHZN(string inputData)
        {
            Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
            Match m = RegCHZN.Match(inputData);
            return m.Success;
        }

        

        /// <summary>
        /// 加入缓存,滑动过期,单位:秒
        /// </summary>
        public static bool AddCacheSliding(string key, object value, int timeout)
        {
            var item = new CacheItem(key, value);
            var policy = new CacheItemPolicy();
            policy.SlidingExpiration = new TimeSpan(1000 * timeout);
            return MemoryCache.Default.Add(item, policy);
        }

        /// <summary>
        /// 加入缓存,定时过期,单位:秒
        /// </summary>
        public static bool AddCacheAbsolute(string key, object value, int timeout)
        {
            var item = new CacheItem(key, value);
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now + TimeSpan.FromSeconds(timeout);
            return MemoryCache.Default.Add(item, policy);
        }

        /// <summary>
        /// 缓存中是否有
        /// </summary>
        public static bool CacheIsHave(string key)
        {
            return MemoryCache.Default.Contains(key);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        public static object RemoveCache(string key)
        {
            return MemoryCache.Default.Remove(key);
        }

       

        /// <summary>
        /// 用starttime得到到当前的时间
        /// </summary>
        /// <returns>单位:毫秒</returns>
        public static long GetMilliseconds(long startTime)
        {
            return ((DateTime.Now.Ticks - startTime) / 10000);
        }

        /// <summary>
        /// 用starttime得到到当前的时间
        /// </summary>
        /// <returns>单位:秒</returns>
        public static long GetSeconds(long startTime)
        {
            return ((DateTime.Now.Ticks - startTime) / 10000 / 1000);
        }

        /// <summary>
        /// 格式化JSon串
        /// </summary>
        public static string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 获取该接口的所有实现类
        /// </summary>
        public static List<Type> FindAllClassByInterface<T>()
        {
            var types = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var t in type.GetInterfaces())
                    {
                        if (t == typeof(T))
                        {
                            types.Add(type);
                            break;
                        }
                    }
                }
            }

            return types;
        }

        /// <summary>
        /// 获取类上的Description注解
        /// </summary>
        public static string GetDescription(Type type)
        {
            try
            {
                object[] attribArray = type.GetCustomAttributes(false);
                DescriptionAttribute attrib = (DescriptionAttribute)attribArray[0];
                return attrib.Description;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>  
        /// 获取枚举值的描述文本  
        /// </summary>  
        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                  (DescriptionAttribute[])fi.GetCustomAttributes(
                  typeof(DescriptionAttribute), false);

            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// 对象的属性是否都不为null,仅限可以为null的类型
        /// </summary>
        public static bool ObjectIsNotNull(Type t, object o)
        {
            foreach (var pi in t.GetProperties())
            {
                var v = pi.GetValue(o);
                if (v == null)
                    return false;
            }
            return true;
        }
    }
}
