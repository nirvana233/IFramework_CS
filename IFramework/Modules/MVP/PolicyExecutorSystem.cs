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

        protected override void OnSetMessage(MessageModule message)
        {
            message.Subscribe<PolicySystem>(OnPolicy);
        }

        private void OnPolicy(Type type, int code, IEventArgs args, object[] param)
        {
            OnPolicy(code, args, param);
        }

        protected void SendPolicyExecutor(int code, IEventArgs args, params object[] param)
        {
            SendMessage(typeof(PolicyExecutorSystem), code, args, param);
        }
        protected abstract void OnPolicy(int code, IEventArgs args, object[] param);
        protected override void Excute() { }


    }

}
