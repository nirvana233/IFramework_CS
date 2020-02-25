using IFramework.Modules.ECS;
using IFramework.Modules.Message;
using System;

namespace IFramework.Modules.MVP
{
    /// <summary>
    /// MVP系统
    /// </summary>
    public abstract class MVPSystem : IExcuteSystem, IMessagePublisher
    {
        private bool _moduleDisposed;
        private MessageModule _message;
        private MVPEnity _enity;
        /// <summary>
        /// 消息转发
        /// </summary>
        public MessageModule message
        {
            get { return _message; }
            internal set
            {
                _message = value;
                if (value!=null)
                    OnSetMessage(value);
            }
        }
        /// <summary>
        /// 实体
        /// </summary>
        public MVPEnity enity { get { return _enity; } internal set { _enity = value; OnSetEnity(value); } }

        internal MVPSystem() { _moduleDisposed = false; }
        void IExcuteSystem.Excute()
        {
            if (_moduleDisposed) return;
            Excute();
        }
        void IExcuteSystem.OnModuleDispose()
        {
            _moduleDisposed = true;
        }
        internal void GroupDispose()
        {
            OnGroupDispose();
            _moduleDisposed = true;
        }
        /// <summary>
        /// 组释放时
        /// </summary>
        protected virtual void OnGroupDispose() { }

        /// <summary>
        /// 被设置实体
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnSetEnity(MVPEnity value) { }
        /// <summary>
        /// 被设置消息
        /// </summary>
        /// <param name="message"></param>
        protected abstract void OnSetMessage(MessageModule message);
        /// <summary>
        /// 刷新
        /// </summary>
        protected abstract void Excute();
        /// <summary>
        /// 当模块释放
        /// </summary>

        internal void SendMessage(Type type, int code, IEventArgs args, params object[] param)
        {
            _message.Publish(type, code, args, param);
        }
    }

}
