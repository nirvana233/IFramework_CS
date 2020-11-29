using System;

namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息模块
    /// </summary>
    public interface IMessageModule
    {
        /// <summary>
        /// 每一帧处理消息上限
        /// </summary>
        int processesPerFrame { get; set; }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        void Publish(Type publishType, int code, IEventArgs args, MessageUrgencyType type = MessageUrgencyType.Common, params object[] param);
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        void Publish<T>(int code, IEventArgs args, MessageUrgencyType type = MessageUrgencyType.Common, params object[] param) where T : IMessagePublisher;
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        void Publish<T>(T t, int code, IEventArgs args, MessageUrgencyType type = MessageUrgencyType.Common, params object[] param) where T : IMessagePublisher;
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <param name="param"></param>
        void PublishByNumber<T>(T t, int code, IEventArgs args, int priority = 512, params object[] param) where T : IMessagePublisher;
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <param name="param"></param>
        void PublishByNumber<T>(int code, IEventArgs args, int priority = 512, params object[] param) where T : IMessagePublisher;
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <param name="param"></param>
        void PublishByNumber(Type publishType, int code, IEventArgs args, int priority = 512, params object[] param);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool Subscribe(Type publishType, IMessageListener listener);
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool Subscribe(Type publishType, MessageListener listener);
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool Subscribe<T>(IMessageListener listener) where T : IMessagePublisher;
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool Subscribe<T>(MessageListener listener) where T : IMessagePublisher;
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool UnSubscribe(Type publishType, IMessageListener listener);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool UnSubscribe(Type publishType, MessageListener listener);
        /// <summary>
        /// 移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool UnSubscribe<T>(IMessageListener listener) where T : IMessagePublisher;
        /// <summary>
        /// 移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool UnSubscribe<T>(MessageListener listener) where T : IMessagePublisher;
    }
}