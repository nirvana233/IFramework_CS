using IFramework.Modules.Message;

namespace IFramework.Fast
{
    /// <summary>
    /// 子实例
    /// </summary>
    public interface ISubEntity : IEntity {
        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool SubscribeMessage<T>(IMessageListener listener);
        /// <summary>
        /// 取消监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        bool UnSubscribeMessage<T>(IMessageListener listener);
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        IMessage PublishMessage<T>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common);
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="command"></param>
        void SendCommand(ICommand command);
    }
}
