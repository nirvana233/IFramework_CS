using IFramework.Modules.Message;
using System;

namespace IFramework.MVVM
{
    /// <summary>
    /// 界面
    /// </summary>
    [ScriptVersion(12)]
    public abstract class View : IDisposable
    {
        private ObservableValue<ViewModel> _context = new ObservableValue<ViewModel>(null);
        /// <summary>
        /// 数据绑定
        /// </summary>
        protected ObservableObjectHandler handler;
        /// <summary>
        /// VM
        /// </summary>
        public ViewModel context
        {
            get { return _context; }
            set
            {
                _context.value = value;
            }
        }
        /// <summary>
        /// ctor
        /// </summary>
        public View()
        {
            handler = new ObservableObjectHandler();
            _context.Subscribe(BindProperty);
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        protected virtual void BindProperty()
        {
            handler.UnSubscribe();
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            handler.UnSubscribe();
        }
        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() { }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message"></param>
        protected void Publish(IEventArgs message)
        {
            (context as IViewModel).Listen(message);
        }

        
    }
    /// <summary>
    /// 方便书写
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class View<T> : View where T : ViewModel
    {
        /// <summary>
        /// 方便书写
        /// </summary>
        public T Tcontext { get { return context as T; } set { context = value; } }
    }
}
