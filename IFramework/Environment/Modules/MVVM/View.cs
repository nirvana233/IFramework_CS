using IFramework.Modules.Message;
using System;

namespace IFramework.Modules.MVVM
{
    /// <summary>
    /// 界面
    /// </summary>
    [ScriptVersion(12)]
    public abstract class View : IDisposable
    {
        internal MVVMGroup group { get; set; }
        private bool _inited;
        /// <summary>
        /// 消息转发
        /// </summary>
        protected IMessageModule message { get { return group.message; } }


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
                if (!_inited)
                {
                    handler = new ObservableObjectHandler();
                    _context.Subscribe(BindProperty);
                    _inited = true;
                }
                _context.value = value;
            }
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
            _context.Dispose();
        }
        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() { }
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
