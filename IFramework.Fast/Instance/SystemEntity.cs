using IFramework.Injection;
using IFramework.Modules.Message;

namespace IFramework.Fast
{
    /// <summary>
    /// 子实体
    /// </summary>
    /// <typeparam name="TEnvironmentEnity"></typeparam>
    public abstract class SystemEntity<TEnvironmentEnity> : FastEntity<TEnvironmentEnity>, ISystemEntity where TEnvironmentEnity : EnvironmentEntity<TEnvironmentEnity>
    {
        /// <summary>
        /// 标记
        /// </summary>
        protected virtual string flag { get { return GetType().Name; } }
        /// <summary>
        /// 数据容器
        /// </summary>
        private IValuesContainer container { get { return EnvironmentEntity.env.container; } }
        /// <summary>
        /// 消息
        /// </summary>
        private IMessageModule message { get { return EnvironmentEntity.env.modules.GetModule<MessageModule>(flag); } }
        /// <summary>
        /// ctor
        /// </summary>
        protected SystemEntity()
        {
            container.SubscribeInstance(this.GetType(),this, "", false);
            message.fitSubType = true;
            message.processesPerFrame = 20;
            //message.Subscribe<ICommand>(Listen);
            Awake();
            container.InjectInstances();
        }

        //private void Listen(IMessage message)
        //{
        //    if (message.args.Is<ICommand>())
        //    {
        //        message.args.As<ICommand>().Excute();
        //    }
        //}
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
            command.Excute();
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return  GetValue<TModel>();
        }
        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return GetValue<TUtility>();
        }
        /// <summary>
        /// 获取数据处理
        /// </summary>
        /// <typeparam name="TProcessor"></typeparam>
        public TProcessor GetModelProcessor<TProcessor>() where TProcessor : class, IProcessor
        {
            return GetValue<TProcessor>();
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        public void SetModel<TModel>(TModel model) where TModel : class, IModel
        {
            SetValue(model);
        }
        /// <summary>
        /// 设置工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <param name="utility"></param>
        public void SetUtility<TUtility>(TUtility utility) where TUtility : class, IUtility
        {
            SetValue(utility);
        }
        /// <summary>
        /// 设置数据处理
        /// </summary>
        /// <typeparam name="TProcessor"></typeparam>
        /// <param name="processor"></param>
        public void SetModelProcessor<TProcessor>(TProcessor processor) where TProcessor : class, IProcessor
        {
            SetValue(processor);
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>() where T : class
        {
            return container.GetValue<T>(flag);
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void SetValue<T>(T t) where T : class
        {
            container.SubscribeInstance(t, flag);
        }
    }
}
