using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace IFramework
{
    /// <summary>
    /// 可观测 Object
    /// </summary>
    [FrameworkVersion(20)]
    public class ObservableObject : IDisposable
    {
        private Dictionary<string, Action> _callmap;
        /// <summary>
        /// Ctor
        /// </summary>
        protected ObservableObject()
        {
            _callmap = new Dictionary<string, Action>();
        }
        /// <summary>
        /// 注册数值变化监听
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="listener"></param>
        public void Subscribe(string propertyName, Action listener)
        {
            if (!_callmap.ContainsKey(propertyName))
                _callmap.Add(propertyName, null);
            _callmap[propertyName] += listener;
        }
        /// <summary>
        /// 取消注册数值变化监听
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="listener"></param>
        public void UnSubscribe(string propertyName, Action listener)
        {
            if (!_callmap.ContainsKey(propertyName))
                throw new Exception("Have not Subscribe " + propertyName);
            _callmap[propertyName] -= listener;
            if (_callmap[propertyName] == null)
                _callmap.Remove(propertyName);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">获取的属性</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        protected T GetProperty<T>(ref T property, string propertyName = "")
        {
            if (ObservableObjectHandler.recordingHandler != null)
            {
                if (string.IsNullOrEmpty(propertyName))
                    propertyName = GetProperyName(new StackTrace(true).GetFrame(1).GetMethod().Name);
                ObservableObjectHandler.recordingHandler.Subscribe(this, propertyName);
            }
            return property;
        }
        private string GetProperyName(string methodName)
        {
            if (methodName.StartsWith("get_") || methodName.StartsWith("set_") ||
                methodName.StartsWith("put_"))
            {
                return methodName.Substring("get_".Length);
            }
            throw new Exception(methodName + " not a method of Property");
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">赋值的变量</param>
        /// <param name="value">变化的值</param>
        /// <param name="propertyName">属性名称</param>
        protected void SetProperty<T>(ref T property, T value, string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            if (string.IsNullOrEmpty(propertyName))
                propertyName = GetProperyName(new StackTrace(true).GetFrame(1).GetMethod().Name);
            property = value;
            PublishPropertyChange(propertyName);
        }
        /// <summary>
        /// 发布属性发生变化
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void PublishPropertyChange(string propertyName)
        {
            if (!_callmap.ContainsKey(propertyName)) return;
            if (_callmap[propertyName] == null) return;

            _callmap[propertyName].Invoke();
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            OnDispose();

            _callmap.Clear();
            _callmap = null;
        }
        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() { }
    }
    /// <summary>
    /// ObservableObject 注册监听Helper
    /// </summary>
    [FrameworkVersion(10)]
    public class ObservableObjectHandler
    {
        struct ObserveEntity
        {
            private string _propertyName;
            private ObservableObject _observableObject;
            private Action _listenner;

            public string propertyName { get { return _propertyName; } }
            public ObservableObject observableObject { get { return _observableObject; } }
            public ObserveEntity(ObservableObject obj, string propertyName, Action listenner)
            {
                this._propertyName = propertyName;
                this._observableObject = obj;
                this._listenner = listenner;
            }

            public void Bind()
            {
                observableObject.Subscribe(propertyName, _listenner);
            }
            public void UnBind()
            {
                observableObject.UnSubscribe(propertyName, _listenner);
            }
        }

        internal static ObservableObjectHandler recordingHandler { get; private set; }

        private List<ObserveEntity> _entitys = new List<ObserveEntity>();
        internal Action listenner;
        internal ObservableObjectHandler Subscribe(ObservableObject _object, string propertyName)
        {
            Subscribe(_object, propertyName, listenner);
            return this;
        }
        /// <summary>
        /// 对一个 ObservableObject 注册一个监听
        /// </summary>
        /// <param name="_object"> ObservableObject </param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="listenner">回调</param>
        /// <returns></returns>
        public ObservableObjectHandler Subscribe(ObservableObject _object, string propertyName, Action listenner)
        {
            var bindTarget = new ObserveEntity(_object, propertyName, listenner);
            bindTarget.Bind();
            _entitys.Add(bindTarget);
            return this;
        }
        /// <summary>
        /// 绑定一个监听
        /// </summary>
        /// <param name="setter"> 回调</param>
        /// <returns></returns>
        public ObservableObjectHandler BindProperty(Action setter)
        {
            this.listenner = setter;
            recordingHandler = this;
            setter.Invoke();
            listenner = null;
            recordingHandler = null;
            return this;
        }
        /// <summary>
        /// 绑定一个监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setter"> 设置值 </param>
        /// <param name="getter"> 获取值 </param>
        /// <returns></returns>
        public ObservableObjectHandler BindProperty<T>(Action<T> setter, Func<T> getter)
        {
            this.listenner = () => { setter(getter()); };
            setter(AddExpressionListener(getter));
            return this;
        }
        private T AddExpressionListener<T>(Func<T> expression)
        {
            recordingHandler = this;
            var result = expression.Invoke();
            recordingHandler = null;
            listenner = null;
            return result;
        }
        /// <summary>
        /// 取消所有监听
        /// </summary>
        public void UnSubscribe()
        {
            _entitys.ForEach((entity) =>
            {
                entity.UnBind();
            });
            _entitys.Clear();
        }
        /// <summary>
        /// 取消符合条件的监听
        /// </summary>
        /// <param name="_object"> ObservableObject </param>
        /// <param name="propertyName"> 属性名称 </param>
        public void UnSubscribe(ObservableObject _object, string propertyName)
        {
            var result = _entitys.RemoveAll((entity) =>
            {
                if (entity.observableObject != _object || entity.propertyName != propertyName) return false;
                entity.UnBind();
                return true;
            });
        }
    }
    /// <summary>
    /// 可观测树值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [FrameworkVersion(10)]
    public class ObservableValue<T> : ObservableObject
    {
        /// <summary>
        /// 默认的名字
        /// </summary>
        public const string ValuePropertyName = "value";
        private T _value;
        /// <summary>
        /// 具体的数值
        /// </summary>
        public T value
        {
            get { return GetProperty(ref _value, ValuePropertyName); }
            set
            {
                SetProperty(ref _value, value, ValuePropertyName);
            }
        }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value"></param>
        public ObservableValue(T value) : base()
        {
            _value = value;
        }
        /// <summary>
        /// 注册 value 变化监听
        /// </summary>
        /// <param name="listener"></param>
        public void Subscribe(Action listener)
        {
            base.Subscribe(ValuePropertyName, listener);
        }
        /// <summary>
        /// 取消注册 value 变化监听
        /// </summary>
        /// <param name="listener"></param>
        public void UnSubscribe(Action listener)
        {
            base.UnSubscribe(ValuePropertyName, listener);
        }
        /// <summary>
        /// 方便书写，缩减代码
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator T(ObservableValue<T> value)
        {
            return value.value;
        }
    }





}
