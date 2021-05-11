using IFramework.Injection;
using IFramework.Modules.Message;

namespace IFramework.Fast
{
    /// <summary>
    /// 界面
    /// </summary>
    /// <typeparam name="TSystemEntity"></typeparam>
    /// <typeparam name="TEnvironmentEntity"></typeparam>
    public abstract class View<TSystemEntity,TEnvironmentEntity>:FastEntity<TEnvironmentEntity>
        where TEnvironmentEntity : EnvironmentEntity<TEnvironmentEntity>
        where TSystemEntity:ISystemEntity
    {
        /// <summary>
        /// 实体
        /// </summary>
        [Inject] public TSystemEntity systemEntity { get; set; }
        /// <summary>
        /// 环境
        /// </summary>
        [Inject] public IEnvironment env { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        protected View()
        {
            this.Inject();
            Awake();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Awake();

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool SubscribeMessage<T>(IMessageListener listener)
        {
            return systemEntity.SubscribeMessage<T>(listener);
        }
        /// <summary>
        /// 取消监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool UnSubscribeMessage<T>(IMessageListener listener)
        {
            return systemEntity.UnSubscribeMessage<T>(listener);

        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage PublishMessage<T>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return systemEntity.PublishMessage<T>(args, priority);
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(ICommand command)
        {
            systemEntity.SendCommand(command);
        }
        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return systemEntity.GetUtility<TUtility>();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return systemEntity.GetModel<TModel>();
        }
    }
}
