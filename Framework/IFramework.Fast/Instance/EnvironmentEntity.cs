using IFramework.Modules.Message;
using IFramework.Singleton;
using System;

namespace IFramework.Fast
{
    /// <summary>
    /// 实体容器
    /// </summary>
    /// <typeparam name="TEnvironmentEntity"></typeparam>
    public abstract class EnvironmentEntity<TEnvironmentEntity> :FastEntity<TEnvironmentEntity> ,IEvvironmentEntity, ISingleton where TEnvironmentEntity : EnvironmentEntity<TEnvironmentEntity>
    {
        private class RootSystemEntity : SystemEntity<TEnvironmentEntity>
        {
            public override string name => "";
            protected override void Awake()
            {

            }
        }
        private RootSystemEntity __sys;
        private RootSystemEntity _sys
        {
            get
            {
                if (__sys == null)
                {
                    __sys = new RootSystemEntity();
                }
                return __sys;
            }
        }
        /// <summary>
        /// 跟实例
        /// </summary>
        protected static IEvvironmentEntity self { get { return EnvironmentEntity; } }
        /// <summary>
        /// 环境类型
        /// </summary>
        protected abstract EnvironmentType envType { get; }
        /// <summary>
        /// 环境
        /// </summary>
        public IEnvironment env { get; private set; }

         string  ISystemEntity.name { get { return _sys.name; } }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static TEnvironmentEntity Initialize()
        {
            return EnvironmentEntity;
        }
        /// <summary>
        /// 刷新
        /// </summary>
        public static void Update()
        {
            EnvironmentEntity.env.Update();
        }
        /// <summary>
        /// 消灭
        /// </summary>
        public static void Destory()
        {
            EnvironmentEntity.env.Dispose();
            SingletonCollection.Dispose<TEnvironmentEntity>();
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
            env.container.SubscribeInstance<TEnvironmentEntity>(this as TEnvironmentEntity, "", false);
        }
        /// <summary>
        /// 获取根model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public static TModel GetModel<TModel>() where TModel : class, IModel
        {
            return self.GetModel<TModel>();
        }
        /// <summary>
        /// 获取根工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        public static TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return self.GetUtility<TUtility>();
        }

        /// <summary>
        /// 注册根消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static void SubscribeMessage<T1>(MessageListener listener)
        {
            self.SubscribeMessage<T1>(listener);
        }
        /// <summary>
        /// 取消监听根消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static void UnSubscribeMessage<T1>(MessageListener listener)
        {
            self.UnSubscribeMessage<T1>(listener);
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
            return self.PublishMessage<T1>(args, priority);
        }
        /// <summary>
        /// 发布根命令
        /// </summary>
        /// <param name="command"></param>
        public static void SendCommand(ICommand command)
        {
            self.SendCommand(command);
        }
        /// <summary>
        /// 设置根数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        public static void SetModel<TModel>(TModel model) where TModel : class, IModel
        {
            self.SetModel<TModel>(model);
        }
        /// <summary>
        /// 设置根工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <param name="utility"></param>
        public static void SetUtility<TUtility>(TUtility utility) where TUtility : class, IUtility
        {
            self.SetUtility<TUtility>(utility);
        }

        /// <summary>
        /// 获取根数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetValue<T>() where T : class
        {
            return self.GetValue<T>();
        }

        /// <summary>
        /// 设置根数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void SetValue<T>(T t) where T : class
        {
            self.SetValue<T>(t);
        }















        TModel ISystemEntity.GetModel<TModel>()
        {
            return _sys.GetModel<TModel>();
        }

        TUtility ISystemEntity.GetUtility<TUtility>()
        {
            return _sys.GetUtility<TUtility>();
        }



        void ISystemEntity.SubscribeMessage<T1>(MessageListener listener)
        {
            _sys.SubscribeMessage<T1>(listener);
        }

        void ISystemEntity.UnSubscribeMessage<T1>(MessageListener listener)
        {
            _sys.UnSubscribeMessage<T1>(listener);
        }

        IMessage ISystemEntity.PublishMessage<T1>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return _sys.PublishMessage<T1>(args, priority);
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



        void IDisposable.Dispose()
        {
            OnDispose();
        }

        T ISystemEntity.GetValue<T>()
        {
            return _sys.GetValue<T>();
        }

        void ISystemEntity.SetValue<T>(T t)
        {
            _sys.SetValue<T>(t);
        }
    }
}
