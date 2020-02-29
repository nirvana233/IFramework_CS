/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

    public abstract class StringConverter
    {
        private static Dictionary<Type, Type> _map = new Dictionary<Type, Type>()
        {
            { typeof(int),typeof(IntStringConverter)},
            { typeof(short),typeof(ShortStringConverter)},
            { typeof(long),typeof(LongStringConverter)},
            { typeof(UInt16),typeof(UInt16StringConverter)},
            { typeof(UInt32),typeof(UInt32StringConverter)},
            { typeof(UInt64),typeof(UInt64StringConverter)},
            { typeof(float),typeof(FloatStringConverter)},
            { typeof(double),typeof(DoubleStringConverter)},
            { typeof(decimal),typeof(DecimalStringConverter)},
            { typeof(string),typeof(StringStringConverter)},
            { typeof(bool),typeof(BoolStringConverter)},
            { typeof(char),typeof(CharStringConverter)},
            { typeof(byte),typeof(ByteStringConverter)},
            { typeof(DateTime),typeof(DateTimeStringConverter)},
            { typeof(byte[]),typeof(ByteArrayStringConverter)},
            { typeof(object),typeof(ByteArrayStringConverter)},

        };
        private static Dictionary<Type, StringConverter> _ins = new Dictionary<Type, StringConverter>();
        public static void SubscribeConverter<T>(StringConverter<T> converter)
        {
            if (!_ins.ContainsKey(typeof(T)))
            {
                _ins.Add(typeof(T), converter);
            }
            else
            {
                _ins[typeof(T)] = converter;
            }
        }
        public static void SubscribeConverter<T>(Type converter)
        {
            if (!_map.ContainsKey(typeof(T)))
            {
                _map.Add(typeof(T), converter);
            }
            else
            {
                _map[typeof(T)] = converter;
            }
        }

        public abstract string ConvertToString( object t);
        public abstract bool TryConvert( string str, out object result);

        public static StringConverter Get(Type type)
        {
            if (type.IsEnum)
            {
                return  Activator.CreateInstance(typeof(EnumStringConverter<>).MakeGenericType(type)) as StringConverter;
            }
            StringConverter c;
            if (!_ins.TryGetValue(type,out c ))
            {
                Type t;
                if (!_map.TryGetValue(type, out t))
                {
                    throw new Exception("Could not Convert Type " + type);
                }
                else
                {
                    StringConverter sc = Activator.CreateInstance(t) as StringConverter;
                    _ins.Add(type, sc);
                    return sc;
                }
            }
            return c;
        }
        public static StringConverter<T> Get<T>()
        {
            return Get(typeof(T)) as StringConverter<T>;
        }

        public const string dot = ",";
        public const string leftBound = "[ ";
        public const string rightBound = " ]";
        public const char colon = ':';
    }
    public abstract class StringConverter<T>: StringConverter
    {
        public virtual string ConvertToString(T t)
        {
            return t.ToString();
        }
        public abstract bool TryConvert(string self, out T result);

        public override bool TryConvert(string str, out object result)
        {
      
            T t;
            if (TryConvert(str, out t))
            {
                result = t;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }
        public override string ConvertToString(object t)
        {
            return ConvertToString((T)t);
        }
    }

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
            return StringConverter.Get(type).TryConvert(self, out obj);
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
