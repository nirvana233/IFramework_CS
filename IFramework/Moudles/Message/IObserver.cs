using System;

namespace IFramework.Moudles.Message
{
    public interface IObserver
    {
        void Listen(IPublisher publisher, Type type, int code, IEventArgs args, object[] param);
    }
}
