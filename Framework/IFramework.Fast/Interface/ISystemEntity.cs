using IFramework.Modules.Message;

namespace IFramework.Fast
{
    /// <summary>
    /// 子实例
    /// </summary>
    public interface ISystemEntity
    {
        /// <summary>
        /// 名字
        /// </summary>
        string name { get; }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetValue<T>() where T : class;
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        void  SetValue<T>(T t) where T : class;
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        TModel GetModel<TModel>() where TModel : class, IModel;
        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        TUtility GetUtility<TUtility>() where TUtility : class, IUtility;

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        void SubscribeMessage<T>(MessageListener listener);
        /// <summary>
        /// 取消监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        void UnSubscribeMessage<T>(MessageListener listener);
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
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        void SetModel<TModel>(TModel model) where TModel : class, IModel;
        /// <summary>
        /// 设置工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <param name="utility"></param>
        void SetUtility<TUtility>(TUtility utility) where TUtility : class, IUtility;

    }
}
