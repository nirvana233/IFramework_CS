using IFramework.Modules.Message;
using System;

namespace IFramework.Modules.MVP
{
    /// <summary>
    /// 刷新视野
    /// </summary>
    public abstract class ViewSystem : MVPSystem
    {
        protected ViewSystem() : base() { }

        protected abstract void OnPolicyPolicyExecutor(int code, IEventArgs args, object[] param);

        protected override void OnSetMessage(MessageModule message)
        {
            message.Subscribe<PolicyExecutorSystem>(OnPolicyPolicyExecutor);
        }

        protected void OnPolicyPolicyExecutor(Type type, int code, IEventArgs args, object[] param)
        {
            OnPolicyPolicyExecutor(code, args, param);
        }

        protected override void Excute() { }

    }

}
