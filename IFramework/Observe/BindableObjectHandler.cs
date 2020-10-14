using System;
using System.Collections.Generic;

namespace IFramework
{
    /// <summary>
    /// 绑定器
    /// </summary>
    [FrameworkVersion(11)]
    public class BindableObjectHandler : FrameworkObject
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

            public BindEntity(BindableObject bind, string propertyName, BindableObjectHandler handler, Type type)
            {
                this._propertyName = propertyName;
                this._bind = bind;
                this._handler = handler;
                this._type = type;
            }

            public void Bind()
            {
                bindable.Subscribe(propertyName, _listenner);
            }
            public void UnBind()
            {
                bindable.UnSubscribe(propertyName, _listenner);
            }
        }

        internal static BindableObjectHandler handler;
        private List<BindEntity> _entitys = new List<BindEntity>();
        private Dictionary<Type, Dictionary<string, Action<string, object>>> _callmap = new Dictionary<Type, Dictionary<string, Action<string, object>>>();
        private Dictionary<Type, Dictionary<string, object>> _valuemap = new Dictionary<Type, Dictionary<string, object>>();


        /// <summary>
        /// 绑定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setter"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public BindableObjectHandler BindProperty<T>(Action<T> setter, Func<T> getter)
        {
            Type type = typeof(T);
            if (!_callmap.ContainsKey(type))
                _callmap.Add(type, new Dictionary<string, Action<string, object>>());
            if (!_valuemap.ContainsKey(type))
                _valuemap.Add(type, new Dictionary<string, object>());
            handler = this;
            T tvalue = getter.Invoke();
            handler = null;

            Action<string, object> action = (name, obj) => {
                _valuemap[type][name] = obj;
                T t = (T)obj;
                setter(t);
            };
            object value = _valuemap[type][this._propertyName];
            if (value == null)
            {
                if (!EqualityComparer<T>.Default.Equals(tvalue, default(T)))
                {
                    action.Invoke(this._propertyName, value);
                }
            }
            else
            {
                if (!EqualityComparer<T>.Default.Equals(tvalue, (T)value))
                {
                    action.Invoke(this._propertyName, value);
                }
            }

            _callmap[type][this._propertyName] += action;

            for (int i = 0; i < _entitys.Count; i++)
            {
                if (_entitys[i].type == type && _entitys[i].propertyName == this._propertyName)
                {
                    _entitys[i].UnBind();
                    _entitys[i].Bind();
                }
            }
            this._propertyName = string.Empty;
            return this;
        }
        private string _propertyName;
        internal BindableObjectHandler Subscribe(BindableObject _object, Type propertyType, string propertyName)
        {
            this._propertyName = propertyName;

            if (!_callmap[propertyType].ContainsKey(propertyName))
                _callmap[propertyType].Add(propertyName, null);
            if (!_valuemap[propertyType].ContainsKey(propertyName))
                _valuemap[propertyType].Add(propertyName, null);

            var listenner = _callmap[propertyType];
            var bindTarget = new BindEntity(_object, propertyName, this, propertyType);
            _entitys.Add(bindTarget);
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
            if (!_callmap.ContainsKey(type))
                throw new Exception(string.Format("Not Exist type {0},name {1}", type, propertyName));
            if (!_callmap[type].ContainsKey(propertyName))
                throw new Exception(string.Format("Not Exist type {0},name {1}", type, propertyName));
            _callmap[type][this._propertyName].Invoke(_propertyName, value);
            return this;
        }

        /// <summary>
        /// 解绑全部
        /// </summary>
        public void UnBind()
        {
            _entitys.ForEach((entity) =>
            {
                entity.UnBind();
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
                if (entity.bindable != _object) return false;
                entity.UnBind();
                return true;
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
                if (entity.propertyName != propertyName) return false;
                entity.UnBind();
                return true;
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
                if (entity.bindable != _object || entity.propertyName != propertyName) return false;
                entity.UnBind();
                return true;
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
                if (entity.bindable != _object || entity.type != type || entity.propertyName != propertyName) return false;
                entity.UnBind();
                return true;
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
