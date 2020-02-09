using System;

namespace IFramework.Modules.Message
{
    public interface IObserver
    {
        void Listen(IPublisher publisher, Type type, int code, IEventArgs args, object[] param);
    }
}
