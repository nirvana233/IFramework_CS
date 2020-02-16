using System;

namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息监听者
    /// </summary>
    public interface IMessageListener
    {
        /// <summary>
        /// 收到消息回调
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="param"></param>
        void Listen(Type publishType, int code, IEventArgs args, object[] param);
    }
}
