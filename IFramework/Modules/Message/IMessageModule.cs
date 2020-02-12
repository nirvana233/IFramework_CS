using System;

namespace IFramework.Modules.Message
{
    internal interface IMessageModule
    {
        bool Subscribe(Type type, IMessageListener listener);
        bool Subscribe<T>(IMessageListener listener) where T : IMessagePublisher;
        bool Unsubscribe(Type type, IMessageListener listener);
        bool Unsubscribe<T>(IMessageListener listener) where T : IMessagePublisher;
        bool Publish<T>(int code, IEventArgs args, params object[] param) where T : IMessagePublisher;
        bool Publish<T>(T t, int code, IEventArgs args, params object[] param) where T : IMessagePublisher;
        bool Publish(Type type, int code, IEventArgs args, params object[] param);
        bool Publish(IMessagePublisher publisher, Type type, int code, IEventArgs args, params object[] param);
        bool Subscribe(Type type, MessageLostener listener);
        bool Subscribe<T>(MessageLostener listener) where T : IMessagePublisher;
        bool Unsubscribe(Type type, MessageLostener listener);
        bool Unsubscribe<T>(MessageLostener listener) where T : IMessagePublisher;

    }

}
