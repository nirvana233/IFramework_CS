using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace IFramework
{
    /// <summary>
    /// 可观测树值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [FrameworkVersion(10)]
    public class ObservableValue<T> : ObservableObject
    {
        private T _value;
        /// <summary>
        /// 具体的数值
        /// </summary>
        public T value
        {
            get { return GetProperty(ref _value, GetPropertyName(() => this.value)); }
            set
            {
                SetProperty(ref _value, value, GetPropertyName(() => this.value));
            }
        }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value"></param>
        public ObservableValue(T value):base()
        {
            _value = value;
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
    /// <summary>
    /// 可观测 Object
    /// </summary>
    [FrameworkVersion(10)]
    public class ObservableObject:IDisposable
    {
        /// <summary>
        /// 属性发生变化消息
        /// </summary>
        public struct PropertyChangedArg:IEventArgs
        {
            /// <summary>
            /// 对应的 Object
            /// </summary>
            public ObservableObject observableObject;
            /// <summary>
            /// 发生改变的 属性名称
            /// </summary>
            public string propertyName;
        }
        private PropertyChangedArg _propertyChangedArg;
        private event Action<PropertyChangedArg> onPropertyChanged;
        /// <summary>
        /// Ctor
        /// </summary>
        protected ObservableObject()
        {
            _propertyChangedArg = new PropertyChangedArg();
        }
        /// <summary>
        /// 注册数值变化监听
        /// </summary>
        /// <param name="listener"></param>
        public void Subscribe(Action<PropertyChangedArg> listener)
        {
            onPropertyChanged += listener;
        }
        /// <summary>
        /// 取消注册数值变化监听
        /// </summary>
        /// <param name="listener"></param>
        public void UnSubscribe(Action<PropertyChangedArg> listener)
        {
            onPropertyChanged -= listener;
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
        /// 获取属性名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">属性表达式</param>
        /// <returns></returns>
        public static  string GetPropertyName<T>(Expression<Func<T>> property)
        {
            MemberExpression memberExpression = property.Body as MemberExpression;
            if (memberExpression == null) return null;
            return memberExpression.Member.Name;
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
            if (onPropertyChanged != null)
            {
                _propertyChangedArg.observableObject = this;
                _propertyChangedArg.propertyName = propertyName;
            }
            onPropertyChanged(_propertyChangedArg);
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            onPropertyChanged = null;
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
        struct ObserveEnity
        {
            private string _propertyName;
            private ObservableObject _observableObject;
            private Action<ObservableObject.PropertyChangedArg> _callback;

            public string propertyName { get { return _propertyName; } }
            public ObservableObject observableObject { get { return _observableObject; } }
            public ObserveEnity(ObservableObject obj, string propertyName, Action<ObservableObject.PropertyChangedArg> callback)
            {
                this._propertyName = propertyName;
                this._observableObject = obj;
                this._callback = (eve) =>
                {
                    if (propertyName == null || propertyName == eve.propertyName)
                    {
                        callback.Invoke(eve);
                    }
                };
            }

            public void Bind()
            {
                observableObject.Subscribe(_callback);
            }
            public void UnBind()
            {
                observableObject.UnSubscribe(_callback);
            }
        }

        internal static ObservableObjectHandler recordingHandler { get; private set; }

        private List<ObserveEnity> _enitys = new List<ObserveEnity>();
        internal Action<ObservableObject.PropertyChangedArg> callback;
        internal ObservableObjectHandler Subscribe(ObservableObject _object, string propertyName)
        {
            Subscribe(_object, propertyName, callback);
            return this;
        }
        /// <summary>
        /// 对一个 ObservableObject 注册一个监听
        /// </summary>
        /// <param name="_object"> ObservableObject </param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        public ObservableObjectHandler Subscribe(ObservableObject _object, string propertyName, Action<ObservableObject.PropertyChangedArg> callback)
        {
            var bindTarget = new ObserveEnity(_object, propertyName, callback);
            bindTarget.Bind();
            _enitys.Add(bindTarget);
            return this;
        }
        /// <summary>
        /// 绑定一个监听
        /// </summary>
        /// <param name="setter"> 回调</param>
        /// <returns></returns>
        public ObservableObjectHandler BindProperty(Action setter)
        {
            this.callback = (e) => { setter.Invoke(); };
            recordingHandler = this;
            setter.Invoke();
            callback = null;
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
            this.callback=(e) => { setter(getter()); };
            setter(AddExpressionListener(getter));
            return this;
        }
        private T AddExpressionListener<T>(Func<T> expression)
        {
            recordingHandler = this;
            var result = expression.Invoke();
            recordingHandler = null;
            callback = null;
            return result;
        }
        /// <summary>
        /// 取消所有监听
        /// </summary>
        public void UnSubscribe()
        {
            _enitys.ForEach((enity) =>
            {
                enity.UnBind();
            });
            _enitys.Clear();
        }
        /// <summary>
        /// 取消符合条件的监听
        /// </summary>
        /// <param name="_object"> ObservableObject </param>
        /// <param name="propertyName"> 属性名称 </param>
        public void UnSubscribe(ObservableObject _object, string propertyName)
        {
            var result = _enitys.RemoveAll((enity) =>
            {
                if (enity.observableObject != _object || enity.propertyName != propertyName) return false;
                enity.UnBind();
                return true;
            });
        }
    }
}
