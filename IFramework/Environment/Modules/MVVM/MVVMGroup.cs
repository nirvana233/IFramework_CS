using IFramework.Modules.Message;
using System;

namespace IFramework.Modules.MVVM
{
    /// <summary>
    /// MVVM 组结构
    /// </summary>
    public class MVVMGroup : IDisposable
    {
        private MVVMModule _module;
        internal MVVMModule module
        {
            get { return _module; }
            set
            {
                if (value != null)
                {
                    message = value._message;
                    OnSetMessage(value._message);
                }
                else
                {
                    OnRemoveMessage(message);
                    message = null;
                }
                _module = value;
            }
        }

        private ViewModel _viewModel;
        private View _view;
        private IModel _model;
        private string _name;
        /// <summary>
        /// 组名
        /// </summary>
        public string name { get { return _name; } }
        /// <summary>
        /// 界面
        /// </summary>
        public View view { get { return _view; } }
        /// <summary>
        /// 数据
        /// </summary>
        public IModel model
        {
            get { return _model; }
            set { _model = value;
                if (viewModel!=null)
                {
                    (_viewModel as IViewModel).SyncModelValue();
                }
            }
        }
        /// <summary>
        /// 消息转发
        /// </summary>
        public IMessageModule message { get; private set; }
        /// <summary>
        /// VM
        /// </summary>
        public ViewModel viewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != null)
                {
                    (_viewModel as IViewModel).UnSubscribeMessage();
                    _viewModel.group = null;

                }
                _viewModel = value;
                _viewModel.group = this;
                (_viewModel as IViewModel).SubscribeMessage();
                if (_view != null)
                    _view.context = _viewModel;
            }
        }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="model"></param>
        public MVVMGroup(string name, View view, ViewModel viewModel, IModel model)
        {
            this._name = name;
            this._view = view;
            this._model = model;
            this._viewModel = viewModel;

            this._viewModel.group = this;
            this._view.group = this;
            this._view.context = _viewModel;
        }
        /// <summary>
        /// 移除消息转发时
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnRemoveMessage(IMessageModule message)
        {
            (_viewModel as IViewModel).UnSubscribeMessage();
        }
        /// <summary>
        /// 设置消息转发时
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnSetMessage(IMessageModule message)
        {
            (_viewModel as IViewModel).SubscribeMessage();
        }
        /// <summary>
        /// 发布model数据发生变化
        /// </summary>
        public void PublishModelDirty()
        {
            if (viewModel != null)
            {
                (_viewModel as IViewModel).SyncModelValue();
            }
        }
        /// <summary>
        /// 释放时
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            if (_view != null)
            {
                _view.Dispose();
            }
            if (_viewModel != null)
            {
                (_viewModel as IViewModel).UnSubscribeMessage();
                _viewModel.Dispose();
            }
            if (module != null)
                module.RemoveGroup(name);

        }
        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() { }
    }
}
