using System;
using System.Collections.Generic;

namespace IFramework.FieldExtend
{
    /// <summary>
    /// 可以扩展字段的对象
    /// </summary>
    public class ExtensibleObject : FrameworkObject
    {
        private ValueMap _map = new ValueMap();
        private Dictionary<string, Type> _pairs = new Dictionary<string, Type>();
        /// <summary>
        /// 是否扩展字段名字唯一
        /// </summary>
        public bool uniqueFieldName = true;



        private bool CheckType(Type type, string name, bool createPair = true)
        {
            if (!uniqueFieldName)
                return true;
            if (_pairs.ContainsKey(name))
            {
                return _pairs[name] == type;
            }
            else
            {
                if (createPair)
                {
                    _pairs.Add(name, type);
                }
                return true;
            }
        }
        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[Type type, string name]
        {
            get
            {
                return GetField(type, name);
            }
            set
            {
                SetField(type, name, value);
            }
        }
        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get
            {
                return _map.GetFirst(name);
            }
            set
            {
                SetField(value.GetType(), name, value);
            }
        }


        /// <summary>
        /// 是否存在字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ExistField<T>(string name)
        {
            return ExistField(typeof(T), name);
        }
        /// <summary>
        /// 是否存在字段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ExistField(Type type, string name)
        {
            return _map.Exist(type, name);
        }
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool SetField<T>(string name, T t)
        {
            if (CheckType(typeof(T), name))
            {
                _map.Set<T>(name, t);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool SetField(Type type, string name, object t)
        {
            if (CheckType(type, name))
            {
                _map.Set(type, name, t);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public T GetField<T>(string name, bool autoCreate = true)
        {
            if (CheckType(typeof(T), name))
                return _map.Get<T>(name, autoCreate);
            return default(T);
        }
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public object GetField(Type type, string name, bool autoCreate = true)
        {
            if (CheckType(type, name, autoCreate))
                return _map.Get(type, name, autoCreate);
            return null;
        }
        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            _map.Clear();
            _pairs.Clear();
            _map = null;
        }
    }
}
