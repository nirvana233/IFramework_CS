using IFramework.Modules.Message;
namespace IFramework.MVVM
{
    internal interface IViewModel
    {
        void Initialize();
        void SyncModelValue();
        void Listen(IEventArgs message);
    }
    /// <summary>
    /// VM
    /// </summary>
    public abstract class ViewModel : ObservableObject, IViewModel
    {
        internal MVVMGroup group { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        protected IModel model { get { return group.model; } }

        void IViewModel.SyncModelValue()
        {
            SyncModelValue();
        }
        void IViewModel.Initialize()
        {
            Initialize();
        }
        void IViewModel.Listen(IEventArgs message)
        {
            Listen(message);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Initialize();
        /// <summary>
        /// 同步model数据
        /// </summary>
        protected abstract void SyncModelValue();
        /// <summary>
        /// 来自于view的消息
        /// </summary>
        /// <param name="message"></param>
        protected abstract void Listen(IEventArgs message);
        /// <summary>
        /// 释放时
        /// </summary>
        protected override void OnDispose() { }

       
    }
    /// <summary>
    /// 方便书写
    /// </summary>
    public abstract class ViewModel<T> : ViewModel where T : IModel
    {
        /// <summary>
        /// 方便书写
        /// </summary>
        protected T Tmodel { get { return (T)group.model; } }

    }

}
