using System;

namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息模块
    /// </summary>
    public interface IMessageModule
    {
        /// <summary>
        /// 剩余消息数目
        /// </summary>
        int count { get; }
        /// <summary>
        /// 适配子类型
        /// </summary>
        bool fitSubType { get; set; }
        /// <summary>
        /// 每一帧处理消息上限
        /// </summary>
        int processesPerFrame { get; set; }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="tyoe"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        IMessage Publish(Type tyoe, IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common);
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        IMessage Publish<T>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common) where T : IMessagePublisher;
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        IMessage Publish<T>(T t, IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common) where T : IMessagePublisher;
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        IMessage PublishByNumber<T>(T t, IEventArgs args, int priority = 512) where T : IMessagePublisher;
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        IMessage PublishByNumber<T>(IEventArgs args, int priority = 512) where T : IMessagePublisher;
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        IMessage PublishByNumber(Type type, IEventArgs args, int priority = 512);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool Subscribe(Type type, IMessageListener listener);
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="tyoe"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool Subscribe(Type tyoe, MessageListener listener);
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
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool UnSubscribe(Type type, IMessageListener listener);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool UnSubscribe(Type type, MessageListener listener);
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