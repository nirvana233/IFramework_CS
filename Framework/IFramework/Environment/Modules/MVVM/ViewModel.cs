﻿using IFramework.Modules.Message;
namespace IFramework.Modules.MVVM
{
    internal interface IViewModel
    {
        void SubscribeMessage();
        void UnSubscribeMessage();
        void SyncModelValue();
    }
    /// <summary>
    /// VM
    /// </summary>
    [ScriptVersion(12)]
    public abstract class ViewModel : ObservableObject, IViewModel
    {
        internal MVVMGroup group { get; set; }
        /// <summary>
        /// 消息转发
        /// </summary>
        protected IMessageModule message { get { return group.message; } }
        /// <summary>
        /// 数据
        /// </summary>
        protected IModel model { get { return group.model; } }

        void IViewModel.SubscribeMessage()
        {
            SyncModelValue();
            Initialize();
            SubscribeMessage();
        }
        void IViewModel.UnSubscribeMessage()
        {
            UnSubscribeMessage();
        }
        void IViewModel.SyncModelValue()
        {
            SyncModelValue();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Initialize() { }
        /// <summary>
        /// 同步model数据
        /// </summary>
        protected abstract void SyncModelValue();
        /// <summary>
        /// 注册消息监听
        /// </summary>
        protected virtual void SubscribeMessage() { }
        /// <summary>
        /// 取消消息监听
        /// </summary>
        protected virtual void UnSubscribeMessage() { }

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
