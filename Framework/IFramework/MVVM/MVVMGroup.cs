namespace IFramework.MVVM
{
    /// <summary>
    /// MVVM 组结构
    /// </summary>
    public class MVVMGroup : Unit
    {
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
        }
        /// <summary>
        /// VM
        /// </summary>
        public ViewModel viewModel
        {
            get { return _viewModel; }
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
            (_viewModel as IViewModel).Initialize();
            this._view.context = _viewModel;
            (_viewModel as IViewModel).SyncModelValue();
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
        public override void Dispose()
        {
            if (disposed) return;
            base.Dispose();
            if (_view != null)
            {
                _view.Dispose();
            }
            if (_viewModel != null)
            {
                _viewModel.Dispose();
            }
        }
        /// <summary>
        /// 释放时
        /// </summary>
        protected override void OnDispose() { }
    }
}
