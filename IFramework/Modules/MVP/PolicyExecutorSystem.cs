using IFramework.Modules.Message;
using System;

namespace IFramework.Modules.MVP
{
    /// <summary>
    /// 做事情的
    /// </summary>
    public abstract class PolicyExecutorSystem : MVPSystem
    {
        /// <summary>
        /// Ctor
        /// </summary>
        protected PolicyExecutorSystem() : base() { }
        /// <summary>
        /// 设置好message模块时
        /// </summary>
        /// <param name="message"></param>
        protected override void OnSetMessage(MessageModule message)
        {
            message.Subscribe<PolicySystem>(OnPolicy);
        }

        private void OnPolicy(Type type, int code, IEventArgs args, object[] param)
        {
            OnPolicy(code, args, param);
        }
        /// <summary>
        /// 发送处理信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="param"></param>
        protected void SendPolicyExecutor(int code, IEventArgs args, params object[] param)
        {
            SendMessage(typeof(PolicyExecutorSystem), code, args, param);
        }
        /// <summary>
        /// 收到需要处理命令
        /// </summary>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="param"></param>
        protected abstract void OnPolicy(int code, IEventArgs args, object[] param);
        /// <summary>
        /// 相当于Update
        /// </summary>
        protected override void Excute() { }


    }

}
