using IFramework.Modules.Message;
using IFramework.Singleton;
using System;

namespace IFramework.Fast
{
    /// <summary>
    /// 实体容器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EnvironmentEntity<T>: Entity<T>, IEvvironmentEntity, ISingleton where T: EnvironmentEntity<T>
    {
        private RootSystemEntity __sys;
        private RootSystemEntity _sys
        {
            get
            {
                if (__sys==null)
                {
                    __sys = new RootSystemEntity();
                }
                return __sys;
            } }
        private static ISystemEntity _self { get { return EnvEntity as ISystemEntity; } }
        private class RootSystemEntity : SystemEntity<T>
        {
            protected override string flag => "";
            protected override void Awake()
            {
               
            }
        }
        /// <summary>
        /// 环境类型
        /// </summary>
        protected abstract EnvironmentType envType { get; }
        /// <summary>
        /// 环境
        /// </summary>
        public IEnvironment env { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static T Initialize()
        {
            return EnvEntity;
        }
        /// <summary>
        /// 刷新
        /// </summary>
        public static void Update()
        {
            EnvEntity.env.Update();
        }
        /// <summary>
        /// 消灭
        /// </summary>
        public static void Destory()
        {
            EnvEntity.env.Dispose();
            SingletonCollection.Dispose<T>();
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void OnDispose() { }

        void ISingleton.OnSingletonInit()
        {
            env = Framework.CreateEnv(envType);
            env.InitWithAttribute();
            env.container.SubscribeInstance(env, "", false);
            env.container.SubscribeInstance<T>(this as T, "", false);
        }
        /// <summary>
        /// 获取根model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public static TModel GetModel<TModel>() where TModel : class, IModel
        {
            return _self.GetModel<TModel>();
        }
        /// <summary>
        /// 获取根工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        public static TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return _self.GetUtility<TUtility>();
        }
        /// <summary>
        /// 获取根数据处理
        /// </summary>
        /// <typeparam name="TModelProcessor"></typeparam>
        /// <returns></returns>
        public static TModelProcessor GetModelProcessor<TModelProcessor>() where TModelProcessor : class, IModelProcessor
        {
            return _self.GetModelProcessor<TModelProcessor>();
        }
        /// <summary>
        /// 注册根消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static  bool SubscribeMessage<T1>(IMessageListener listener)
        {
            return _self.SubscribeMessage<T1>(listener);
        }
        /// <summary>
        /// 取消监听根消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static bool UnSubscribeMessage<T1>(IMessageListener listener)
        {
            return _self.UnSubscribeMessage<T1>(listener);
        }
        /// <summary>
        /// 发布根消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static IMessage PublishMessage<T1>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return _self.PublishMessage<T1>(args, priority);
        }
        /// <summary>
        /// 发布根命令
        /// </summary>
        /// <param name="command"></param>
        public static void SendCommand(ICommand command)
        {
            _self.SendCommand(command);
        }
        /// <summary>
        /// 设置根数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        public static void SetModel<TModel>(TModel model)where TModel:class,IModel
        {
            _self.SetModel<TModel>(model);
        }
        /// <summary>
        /// 设置根工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <param name="utility"></param>
        public static void SetUtility<TUtility>(TUtility utility) where TUtility : class, IUtility
        {
            _self.SetUtility<TUtility>(utility);
        }
        /// <summary>
        /// 设置根数据处理
        /// </summary>
        /// <typeparam name="TModelProcessor"></typeparam>
        /// <param name="processor"></param>
        public static void SetModelProcessor<TModelProcessor>(TModelProcessor processor) where TModelProcessor : class, IModelProcessor
        {
            _self.SetModelProcessor<TModelProcessor>(processor);
        }

















        TModel ISystemEntity.GetModel<TModel>()
        {
           return _sys.GetModel<TModel>();
        }

        TUtility ISystemEntity.GetUtility<TUtility>()
        {
            return _sys.GetUtility<TUtility>();
        }

        TModelProcessor ISystemEntity.GetModelProcessor<TModelProcessor>()
        {
            return _sys.GetModelProcessor<TModelProcessor>();
        }

        bool ISystemEntity.SubscribeMessage<T1>(IMessageListener listener)
        {
            return _sys.SubscribeMessage<T1>(listener);
        }

        bool ISystemEntity.UnSubscribeMessage<T1>(IMessageListener listener)
        {
            return _sys.UnSubscribeMessage<T1>(listener);
        }

        IMessage ISystemEntity.PublishMessage<T1>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return _sys.PublishMessage<T1>(args,priority);
        }

        void ISystemEntity.SendCommand(ICommand command)
        {
            _sys.SendCommand(command);
        }

        void ISystemEntity.SetModel<TModel>(TModel model)
        {
            _sys.SetModel<TModel>(model);
        }

        void ISystemEntity.SetUtility<TUtility>(TUtility utility)
        {
            _sys.SetUtility<TUtility>(utility);
        }

        void ISystemEntity.SetModelProcessor<TModelProcessor>(TModelProcessor processor)
        {
            _sys.SetModelProcessor<TModelProcessor>(processor);
        }

        void IDisposable.Dispose()
        {
            OnDispose();
        }
    }
}
