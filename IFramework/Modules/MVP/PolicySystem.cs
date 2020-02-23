using IFramework.Modules.Message;
using System;

namespace IFramework.Modules.MVP
{
    /// <summary>
    /// 消息决断
    /// </summary>
    public abstract class PolicySystem : MVPSystem
    {
        /// <summary>
        /// ctor
        /// </summary>
        protected PolicySystem() : base() { }
        /// <summary>
        /// 设置好message模块时
        /// </summary>
        /// <param name="message"></param>
        protected override void OnSetMessage(MessageModule message)
        {
            message.Subscribe<SensorSystem>(OnSensor);
        }

        private void OnSensor(Type type, int code, IEventArgs args, object[] param)
        {
            OnSensor(code, args, param);
        }
        /// <summary>
        /// 发送处理命令
        /// </summary>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="param"></param>
        protected void SendPolicy(int code, IEventArgs args, params object[] param)
        {
            SendMessage(typeof(PolicySystem), code, args, param);
        }
        /// <summary>
        /// 收到探测器消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="param"></param>
        protected abstract void OnSensor(int code, IEventArgs args, object[] param);
        /// <summary>
        /// 相当于Update
        /// </summary>
        protected override void Excute() { }

    }

}
