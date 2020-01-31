using System;

namespace IFramework.Moudles.Message
{
    public interface IMessageMoudle
    {
        bool Subscribe(Type type, IObserver observer);
        bool Subscribe<T>(IObserver observer) where T : IPublisher;
        bool Unsubscribe(Type type, IObserver observer);
        bool Unsubscribe<T>(IObserver observer) where T : IPublisher;
        bool Publish<T>(int code, IEventArgs args, params object[] param) where T : IPublisher;
        bool Publish<T>(T t, int code, IEventArgs args, params object[] param) where T : IPublisher;
        bool Publish(Type type, int code, IEventArgs args, params object[] param);
        bool Publish(IPublisher publisher, Type type, int code, IEventArgs args, params object[] param);

       
    }

}
