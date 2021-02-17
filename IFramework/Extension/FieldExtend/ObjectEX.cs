using System;
using System.Collections.Generic;

namespace IFramework.FieldExtend
{
    /// <summary>
    /// 扩展字段扩展
    /// </summary>
    public static partial class ObjectEX
    {
        private interface IValue { }
        private struct Value<T> : IValue
        {
            public T value;
            public static implicit operator T(Value<T> value)
            {
                return value.value;
            }
            public static implicit operator Value<T>(T value)
            {
                return new Value<T>() { value = value };
            }
        }

        private static Dictionary<IValue, ExtensibleObject> _map = new Dictionary<IValue, ExtensibleObject>();
        /// <summary>
        /// 获取扩展字段容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ExtensibleObject GetExTable<T>(this T t)
        {
            Value<T> value = t;
            ExtensibleObject _out;
            if (!_map.TryGetValue(value, out _out))
            {
                _out = new ExtensibleObject();
                _map.Add(value, _out);
            }
            return _out;
        }
        /// <summary>
        /// 设置扩展字段名字唯一
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="unique"></param>
        public static void SetExFieldNameUnique<T>(this T t,bool unique=true)
        {
            GetExTable<T>(t).uniqueFieldName = unique;
        }

        /// <summary>
        /// 是否存在扩展字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="FieldType"></typeparam>
        /// <param name="t"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool ExistExField<T, FieldType>(this T t, string fieldName)
        {
            return GetExTable<T>(t).ExistField<FieldType>(fieldName);
        }
        /// <summary>
        /// 是否存在扩展字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool ExistExField<T>(this T t, Type type, string fieldName)
        {
            return GetExTable<T>(t).ExistField(type, fieldName);
        }
        /// <summary>
        /// 设置扩展字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="FieldType"></typeparam>
        /// <param name="t"></param>
        /// <param name="fieldName"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool SetExField<T, FieldType>(this T t, string fieldName, FieldType field)
        {
            return GetExTable<T>(t).SetField<FieldType>(fieldName, field);
        }
        /// <summary>
        /// 设置扩展字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool SetExField<T>(this T t, Type type, string fieldName, object field)
        {
            return GetExTable<T>(t).SetField(type, fieldName, field);
        }

        /// <summary>
        /// 获取扩展字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="FieldType"></typeparam>
        /// <param name="t"></param>
        /// <param name="fieldName"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public static FieldType GetExField<T, FieldType>(this T t, string fieldName, bool autoCreate = true)
        {
            return GetExTable<T>(t).GetField<FieldType>(fieldName, autoCreate);
        }
        /// <summary>
        /// 获取扩展字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public static object GetExField<T>(this T t, Type type, string fieldName, bool autoCreate = true)
        {
            return GetExTable<T>(t).GetField(type, fieldName, autoCreate);
        }


    }
}
