/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

    public static partial class StringConvert
    {
        public static string ConvertToString<T>(this T self)
        {
           return  StringConverter.Get<T>().ConvertToString(self);
        }
        public static string ConvertToString(this object self, Type type)
        {
            if (self == null) return string.Empty;
            var s = StringConverter.Get(type);
            return s.ConvertToString(self);
        }

        public static bool TryConvert<T>(this string self, out T t)
        {
            return StringConverter.Get<T>().TryConvert(self,out t);
        }
        public static bool TryConvert(this string self, Type type, ref object obj)
        {
            if (string.IsNullOrEmpty(self)) return false;
            return StringConverter.Get(type).TryConvertObject(self, out obj);
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
