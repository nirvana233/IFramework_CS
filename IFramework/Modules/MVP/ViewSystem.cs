using IFramework.Modules.Message;
using System;

namespace IFramework.Modules.MVP
{
    /// <summary>
    /// 刷新视野
    /// </summary>
    public abstract class ViewSystem : MVPSystem
    {
        /// <summary>
        /// Ctor
        /// </summary>
        protected ViewSystem() : base() { }
        /// <summary>
        /// 接收到处理消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="param"></param>
        protected abstract void OnPolicyPolicyExecutor(int code, IEventArgs args, object[] param);
        /// <summary>
        /// 设置好message模块时
        /// </summary>
        /// <param name="message"></param>
        protected override void OnSetMessage(MessageModule message)
        {
            message.Subscribe<PolicyExecutorSystem>(OnPolicyPolicyExecutor);
        }

        private void OnPolicyPolicyExecutor(Type type, int code, IEventArgs args, object[] param)
        {
            OnPolicyPolicyExecutor(code, args, param);
        }
        /// <summary>
        /// Update
        /// </summary>
        protected override void Excute() { }

    }

}
