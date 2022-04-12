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
            { typeof(TimeSpan),typeof(TimeSpanStringConverter)},
            { typeof(sbyte),typeof(SByteStringConverter)},
            { typeof(Point2),typeof(Point2StringConverter)},
            { typeof(Point3),typeof(Point3StringConverter)},
        };
        private static Dictionary<Type, Type> _genMap = new Dictionary<Type, Type>()
        {
            {(typeof(List<>)),typeof(ListStringConverter<>) } ,
            {(typeof(Dictionary<,>)),typeof(DictionaryStringConverter<,>) } ,
            {(typeof(Stack<>)),typeof(StackStringConverter<>) } ,
            {(typeof(Queue<>)),typeof(QueueStringConverter<>) } ,
            {(typeof(LinkedList<>)),typeof(LinkedListStringConverter<>) } ,

        };
        private static Dictionary<Type, StringConverter> _ins = new Dictionary<Type, StringConverter>();

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

        public static void SubscribeGenericConverter(Type key,Type converter)
        {
            if (!_genMap.ContainsKey(key))
            {
                _genMap.Add(key, converter);
            }
            else
            {
                _genMap[key] = converter;
            }
        }

        public abstract string ConvertToString( object t);
        public abstract bool TryConvertObject( string str, out object result);

        public static StringConverter Get(Type type)
        {
            StringConverter c;
            if (!_ins.TryGetValue(type,out c))
            {
                Type t;
                if (!_map.TryGetValue(type, out t))
                {
                    c = CreateExtra(type);
                }
                else
                {
                    c = Create(t);
                }
                if (c!=null)
                {
                    _ins.Add(type, c);
                }
                else
                {
                    throw new Exception($"None StringConverter with Type {type}");
                }
            }
            return c;
        }
        public static StringConverter Create(Type type)
        {
            return Activator.CreateInstance(type) as StringConverter;
        }
        public static StringConverter CreateExtra(Type type)
        {
            if (type.IsSubclassOf(typeof(Delegate)))
                return null;
            if (type.IsEnum)
                return Activator.CreateInstance(typeof(EnumStringConverter<>).MakeGenericType(type)) as StringConverter;
            if (type.IsArray)
                return Activator.CreateInstance(typeof(ArrayStringConverter<>).MakeGenericType(type.GetElementType())) as StringConverter;
            if (type.IsGenericType)
            {
                foreach (var item in _genMap.Keys)
                {
                    if (type.IsSubclassOfGeneric(item))
                    {
                        return Activator.CreateInstance(_genMap[item].MakeGenericType(type.GetGenericArguments())) as StringConverter;
                    }
                }
                return null;
            }
            else
            {
                return Activator.CreateInstance(typeof(ObjectStringConverter<>).MakeGenericType(type)) as StringConverter;
            }
        }

        public const char dot = ',';
        public const char leftBound = '{';
        public const char rightBound = '}';
        public const char midLeftBound = '[';
        public const char midRightBound = ']';
        public const char colon = ':';
    }
    public abstract class StringConverter<T> : StringConverter
    {
        public virtual string ConvertToString(T t)
        {
            return t.ToString();
        }
        public abstract bool TryConvert(string self, out T result);

        public override bool TryConvertObject(string str, out object result)
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
        protected T MakeDefault()
        {
            return default(T);
        }
    }
}
