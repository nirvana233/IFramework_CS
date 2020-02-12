using System;

namespace IFramework.Modules.Message
{
    public interface IMessageListener
    {
        void Listen(IMessagePublisher publisher, Type type, int code, IEventArgs args, object[] param);
    }
}
