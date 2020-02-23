using IFramework.Modules.Message;

namespace IFramework.Modules.MVP
{
    /// <summary>
    /// 消息探头
    /// </summary>
    public abstract class SensorSystem : MVPSystem
    {
        /// <summary>
        /// Ctor
        /// </summary>
        protected SensorSystem() : base() { }
        /// <summary>
        /// 设置好message模块时 
        /// </summary>
        /// <param name="message"></param>
        protected override void OnSetMessage(MessageModule message) { }
        /// <summary>
        /// 发送探测到消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="param"></param>
        protected void SendSensor(int code, IEventArgs args, params object[] param)
        {
            SendMessage(typeof(SensorSystem), code, args, param);
        }
        /// <summary>
        /// Update
        /// </summary>
        protected override void Excute() { }
    }

}
