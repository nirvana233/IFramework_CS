using System;

namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息监听
    /// </summary>
    /// <param name="publishType"></param>
    /// <param name="code"></param>
    /// <param name="args"></param>
    /// <param name="param"></param>
    public delegate void MessageListener(Type publishType, int code, IEventArgs args, object[] param);
}
