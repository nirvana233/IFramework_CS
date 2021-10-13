﻿using System;
using System.Collections.Generic;

namespace IFramework
{
    /// <summary>
    /// 绑定器
    /// </summary>
    [ScriptVersion(11)]
    public class BindableObjectHandler : Unit
    {
        struct BindEntity
        {
            private string _propertyName;
            private BindableObject _bind;
            private Type _type;

            private Action<string, object> _listenner { get { return _handler._callmap[_type][_propertyName]; } }
            private BindableObjectHandler _handler;
            public Type type { get { return _type; } }
            public string propertyName { get { return _propertyName; } }
            public BindableObject bindable { get { return _bind; } }
            public Action<string, object> setValueAction;
            public BindableObject.BindOperation operation;
            public BindEntity(BindableObject bind, string propertyName, BindableObjectHandler handler, Type type)
            {
                this._propertyName = propertyName;
                this._bind = bind;
                this._handler = handler;
                this._type = type;
                setValueAction = null;
                operation = BindableObject.BindOperation.Both;
            }

            public void Bind()
            {
                if (_bind.bindOperation == BindableObject.BindOperation.Both && operation == BindableObject.BindOperation.Both)
                    bindable.Subscribe(propertyName, _listenner);
            }
            public void UnBind(bool unbind=true)
            {
                if (unbind)
                    _handler._callmap[_type][_propertyName] -= setValueAction;
                if (_bind.bindOperation == BindableObject.BindOperation.Both && operation == BindableObject.BindOperation.Both)
                    bindable.UnSubscribe(propertyName, _listenner);
            }
        }

        internal static BindableObjectHandler handler;
        private List<BindEntity> _entitys = new List<BindEntity>();

        private ValueMap _valuemap = new ValueMap();
        private Dictionary<Type, Dictionary<string, Action<string, object>>> _callmap = new Dictionary<Type, Dictionary<string, Action<string, object>>>();

        /// <summary>
        /// 绑定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setter"></param>
        /// <param name="getter"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public BindableObjectHandler BindProperty<T>(Action<T> setter, Func<T> getter,BindableObject.BindOperation operation= BindableObject.BindOperation.Both)
        {
            Type type = typeof(T);
            if (!_callmap.ContainsKey(type))
                _callmap.Add(type, new Dictionary<string, Action<string, object>>());
            handler = this;
            T tvalue = getter.Invoke();
            handler = null;
            var lastEntity = _entitys[_entitys.Count - 1];
            string propertyName = lastEntity.propertyName;
            lastEntity.operation = operation;
            lastEntity.setValueAction = (name, obj) => {
                T t;
                if (obj == null)
                {
                    t = default(T);
                }
                else
                {
                    t = (T)obj;
                }
                _valuemap.Set<T>(name, t);

                if (setter!=null)
                {
                    setter(t);
                }
            };

            _entitys[_entitys.Count - 1] = lastEntity;
            T value = _valuemap.Get<T>(propertyName);

            if (value == null)
            {
                if (!EqualityComparer<T>.Default.Equals(tvalue, default(T)))
                {
                    _entitys[_entitys.Count - 1].setValueAction.Invoke(propertyName, value);
                }
            }
            else
            {
                if (!EqualityComparer<T>.Default.Equals(tvalue, value))
                {
                    _entitys[_entitys.Count - 1].setValueAction.Invoke(propertyName, value);
                }
            }

            _callmap[type][propertyName] += _entitys[_entitys.Count - 1].setValueAction;

            for (int i = 0; i < _entitys.Count; i++)
            {
                if (_entitys[i].type == type && _entitys[i].propertyName == propertyName)
                {
                    _entitys[i].UnBind(false);
                    _entitys[i].Bind();
                }
            }
            return this;
        }

        internal BindableObjectHandler Subscribe(BindableObject _object, Type propertyType, string propertyName)
        {
            if (!_callmap[propertyType].ContainsKey(propertyName))
                _callmap[propertyType].Add(propertyName, null);

            var listenner = _callmap[propertyType];
            var bindTarget = new BindEntity(_object, propertyName, this, propertyType);
            _entitys.Add(bindTarget);
            return this;
        }


        /// <summary>
        /// 获取数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetValue<T>(string name)
        {
            return _valuemap.Get<T>(name);
        }
        /// <summary>
        /// 获取数值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetValue(Type type,string name)
        {
            return _valuemap.Get(type,name);
        }


        /// <summary>
        /// 发布变化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public BindableObjectHandler PublishProperty(Type type,object value, string propertyName)
        {
            _valuemap.Set(type, propertyName, value);
            if (!_callmap.ContainsKey(type))
                _callmap.Add(type, new Dictionary<string, Action<string, object>>());
            if (!_callmap[type].ContainsKey(propertyName))
                _callmap[type].Add(propertyName, null);
            if (_callmap[type][propertyName] != null)
            {
                _callmap[type][propertyName].Invoke(propertyName, value);
            }
            return this;
        }

        /// <summary>
        /// 发布变化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public BindableObjectHandler PublishProperty<T>(T value, string propertyName)
        {
            Type type = typeof(T);
            return PublishProperty(type, value, propertyName);
            //_valuemap.Set(type, propertyName, value);

            //if (!_callmap.ContainsKey(type))
            //    _callmap.Add(type, new Dictionary<string, Action<string, object>>());
            //if (!_callmap[type].ContainsKey(propertyName))
            //    _callmap[type].Add(propertyName,new Action<string, object>((str,obj)=> { }));
            //if (_callmap[type][propertyName]!=null)
            //{
            //    _callmap[type][propertyName].Invoke(propertyName, value);
            //}
            //return this;
        }

        /// <summary>
        /// 解绑全部
        /// </summary>
        public void UnBind()
        {
            _entitys.ForEach((entity) =>
            {
                entity.UnBind(true);
            });
            _entitys.Clear();
        }
        /// <summary>
        /// 按照对象解绑
        /// </summary>
        /// <param name="_object"></param>
        public void UnBind(BindableObject _object)
        {
            var result = _entitys.RemoveAll((entity) =>
            {
                if (entity.bindable != _object)
                {
                    entity.UnBind();
                    return false;
                }
                entity.UnBind(true);
                return true;
            });
            _entitys.ForEach((entity) =>
            {
                entity.Bind();
            });
        }
        /// <summary>
        /// 按照名字解绑
        /// </summary>
        /// <param name="propertyName"></param>
        public void UnBind(string propertyName)
        {
            var result = _entitys.RemoveAll((entity) =>
            {
                if (entity.propertyName != propertyName)
                {
                    entity.UnBind();
                    return false;
                } 
                entity.UnBind(true);
                return true;
            });
            _entitys.ForEach((entity) =>
            {
                entity.Bind();
            });

        }
        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="propertyName"></param>
        public void UnBind(BindableObject _object, string propertyName)
        {
            var result = _entitys.RemoveAll((entity) =>
            {
                if (entity.bindable != _object || entity.propertyName != propertyName) {

                    entity.UnBind(false);
                    return false;
                }
                entity.UnBind();
                return true;
            });
            _entitys.ForEach((entity) =>
            {
                entity.Bind();
            });
        }
        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        public void UnBind(BindableObject _object, Type type, string propertyName)
        {
            var result = _entitys.RemoveAll((entity) =>
            {
                if (entity.bindable != _object || entity.type != type || entity.propertyName != propertyName) {
                    entity.UnBind(false);
                    return false;
                }
                entity.UnBind();
                return true;
            });
            _entitys.ForEach((entity) =>
            {
                entity.Bind();
            });
        }
        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            UnBind();
            _callmap.Clear();
            _valuemap.Clear();
        }
    }
}
