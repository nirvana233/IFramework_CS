/*********************************************************************************
 *Author:       OnClick
 *Version:      1.0
 *UnityVersion: 2017.2.3p3
 *Date:         2019-01-26
 *Description:   
 *History:  
**********************************************************************************/

using Newtonsoft.Json;
using System;

namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public interface IJsonHelper
    {
        string ToJsonString(object obj);
        T ToObject<T>(string json);
        object ToObject(Type objectType, string json);
    }
    public class Json
    {
        public static IJsonHelper helper { get; set; }

        static Json() { helper = new DefaultJsonHelper(); }
       
        
        public static string ToJsonString(object obj)
        {
            return helper.ToJsonString(obj);
        }
        public static T ToObject<T>(string json)
        {
            return helper.ToObject<T>(json);
        }
        public static object ToObject(Type objectType, string json)
        {
            return helper.ToObject(objectType, json);
        }

    }
    public class DefaultJsonHelper : IJsonHelper
    {
        public string ToJsonString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T ToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public object ToObject(Type objectType, string json)
        {
            return JsonConvert.DeserializeObject(json, objectType);
        }
    }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
