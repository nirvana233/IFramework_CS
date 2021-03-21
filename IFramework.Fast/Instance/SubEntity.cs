using IFramework.Modules.Message;

namespace IFramework.Fast
{
    /// <summary>
    /// 子实体
    /// </summary>
    /// <typeparam name="TRootEntity"></typeparam>
    public abstract class SubEntity<TRootEntity> : Entity<TRootEntity>, ISubEntity where TRootEntity: class,IRootEntity
    {
        private IMessageModule message { get { return root.env.modules.GetModule<MessageModule>(flag); } }
        /// <summary>
        /// ctor
        /// </summary>
        protected SubEntity()
        {
            if (this is IRootEntity) return;
            container.SubscribeInstance(this.GetType(),this, "", false);
            Awake();

            AfterAwake();
        }
        /// <summary>
        /// 初始化之后
        /// </summary>
        protected virtual void AfterAwake()
        {
            container.InjectInstances();
            message.Subscribe<ICommand>(Listen);
        }
        private void Listen(IMessage message)
        {
            if (message.args.Is<ICommand>())
            {
                message.args.As<ICommand>().Excute();
            }
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
            return message.Subscribe<T>(listener);
        }
        /// <summary>
        /// 取消监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool UnSubscribeMessage<T>(IMessageListener listener)
        {
            return message.UnSubscribe<T>(listener);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage PublishMessage<T>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return message.Publish<T>(args, priority);
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(ICommand command)
        {
            message.Publish<ICommand>(command);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        protected void SetModel<TModel>(TModel model) where TModel : class, IModel
        {
            container.SubscribeInstance(model, flag);
        }
        /// <summary>
        /// 设置工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <param name="utility"></param>
        protected void SetUtility<TUtility>(TUtility utility) where TUtility : class, IUtility
        {
            container.SubscribeInstance(utility, flag);
        }
        /// <summary>
        /// 设置数据处理
        /// </summary>
        /// <typeparam name="TModelProcessor"></typeparam>
        /// <param name="processor"></param>
        protected void SetModelProcessor<TModelProcessor>(TModelProcessor processor) where TModelProcessor : class, IModelProcessor
        {
            container.SubscribeInstance(processor, flag);
            processor.Awake();
        }
        /// <summary>
        /// 设置界面处理
        /// </summary>
        /// <typeparam name="TViewProcessor"></typeparam>
        /// <param name="processor"></param>
        protected void SetViewProcessor<TViewProcessor>(TViewProcessor processor) where TViewProcessor : class, IViewProcessor
        {
            container.SubscribeInstance(processor, flag);
            processor.Awake();
        }
    }
}
