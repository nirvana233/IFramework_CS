using IFramework.Modules.Message;
using System;

namespace IFramework.Modules.MVP
{
    /// <summary>
    /// 消息决断
    /// </summary>
    public abstract class PolicySystem : MVPSystem
    {
        protected PolicySystem() : base() { }

        protected override void OnSetMessage(MessageModule message)
        {
            message.Subscribe<SensorSystem>(OnSensor);
        }

        private void OnSensor(Type type, int code, IEventArgs args, object[] param)
        {
            OnSensor(code, args, param);
        }

        protected void SendPolicy(int code, IEventArgs args, params object[] param)
        {
            SendMessage(typeof(PolicySystem), code, args, param);
        }
        protected abstract void OnSensor(int code, IEventArgs args, object[] param);
        protected override void Excute() { }

    }

}
