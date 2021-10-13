using IFramework.Modules.Message;

namespace IFramework.Fast
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class FastEx
    {
        /// <summary>
        /// 获取其他系统
        /// </summary>
        /// <typeparam name="TSystemEntity"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TSystemEntity GetOtherSystemEntity<TSystemEntity>(this ICanGetOtherSystemEntity obj) where TSystemEntity : class,ISystemEntity
        {
            return obj.GetEnvironmentEnitity().env.container.GetValue<TSystemEntity>();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TModel GetModel<TModel>(this ICanGetModel obj)where TModel:class,IModel
        {
            return obj.GetSystemEntity().GetModel<TModel>();
        }
        /// <summary>
        /// 获取其他系统数据
        /// </summary>
        /// <typeparam name="TSystemEntity"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TModel GetModel<TSystemEntity,TModel>(this ICanGetOtherModel obj) where TModel : class, IModel where TSystemEntity:class ,ISystemEntity
        {
            return obj.GetOtherSystemEntity<TSystemEntity>().GetModel<TModel>();
        }
        /// <summary>
        /// 获取根数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TModel GetEnvironmentModel<TModel>(this ICanGetOtherModel obj) where TModel : class, IModel
        { 
            return obj.GetEnvironmentEnitity().GetModel<TModel>();
        }

        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TUtility GetUtility<TUtility>(this ICanGetOtherUtility obj) where TUtility : class, IUtility
        {
            return obj.GetSystemEntity().GetUtility<TUtility>();
        }
        /// <summary>
        /// 获取其它系统工具
        /// </summary>
        /// <typeparam name="TSystemEntity"></typeparam>
        /// <typeparam name="TUtility"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TUtility GetUtilty<TSystemEntity, TUtility>(this ICanGetOtherUtility obj) where TUtility : class, IUtility where TSystemEntity :class, ISystemEntity
        {
            return obj.GetOtherSystemEntity<TSystemEntity>().GetValue<TUtility>();
        }
        /// <summary>
        /// 获取根工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TUtility GetEnvironmentUtility<TUtility>(this ICanGetOtherUtility obj) where TUtility : class, IUtility
        {
            return obj.GetEnvironmentEnitity().GetUtility<TUtility>();
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static IMessage PublishMessage<T>(this ICanPublishMessage obj, IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common) 
        {
            return obj.GetSystemEntity().PublishMessage<T>(args,priority);
        }

        /// <summary>
        /// 发布其他系统消息
        /// </summary>
        /// <typeparam name="TSystemEntity"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static IMessage PublishMessage<TSystemEntity, T>(this ICanPublishOtherMessage obj, IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)  where TSystemEntity :class, ISystemEntity
        {
            return obj.GetOtherSystemEntity<TSystemEntity>().PublishMessage<T>(args,priority);
        }
        /// <summary>
        /// 发布根消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static IMessage PublishEnvironmentMessage<T>(this ICanPublishOtherMessage obj, IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return obj.GetEnvironmentEnitity().PublishMessage<T>(args,priority);
        }



        /// <summary>
        /// 注册监听消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static void SubscribeMessage<T>(this ICanListenMessage obj,MessageListener listener)
        {
            obj.GetSystemEntity().SubscribeMessage<T>(listener);
        }
        /// <summary>
        /// 注册监听其他系统消息
        /// </summary>
        /// <typeparam name="TSystemEntity"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static void SubscribeMessage<TSystemEntity, T>(this ICanListenOtherMessage obj , MessageListener listener) where TSystemEntity : class, ISystemEntity
        {
             obj.GetOtherSystemEntity<TSystemEntity>().SubscribeMessage<T>(listener);
        }
        /// <summary>
        /// 注册监听根消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static void SubscribeEnvironmentMessage<T>(this ICanListenOtherMessage obj, MessageListener listener)
        {
             obj.GetEnvironmentEnitity().SubscribeMessage<T>(listener);
        }
        /// <summary>
        /// 取消监听消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static void UnSubscribeMessage<T>(this ICanListenMessage obj, MessageListener listener)
        {
             obj.GetSystemEntity().UnSubscribeMessage<T>(listener);
        }
        /// <summary>
        /// 取消监听其它系统消息
        /// </summary>
        /// <typeparam name="TSystemEntity"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static void UnSubscribeMessage<TSystemEntity, T>(this ICanListenOtherMessage obj, MessageListener listener) where TSystemEntity : class, ISystemEntity
        {
             obj.GetOtherSystemEntity<TSystemEntity>().UnSubscribeMessage<T>(listener);
        }
        /// <summary>
        /// 取消监听根消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static void UnSubscribeEnvironmentMessage<T>(this ICanListenOtherMessage obj, MessageListener listener)
        {
             obj.GetEnvironmentEnitity().UnSubscribeMessage<T>(listener);
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="obj"></param>
        /// <param name="command"></param>
        public static void SendCommand<TCommand>(this ICanSendCommand obj,TCommand command) where TCommand : class, ICommand
        {
            obj.GetSystemEntity().SendCommand(command);
        }

    }
}
