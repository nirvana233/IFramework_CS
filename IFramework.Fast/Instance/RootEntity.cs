using IFramework.Injection;
using IFramework.Modules.Message;
using IFramework.Singleton;

namespace IFramework.Fast
{
    /// <summary>
    /// 总实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RootEntity<T> : SubEntity<T>, IRootEntity where T:RootEntity<T>
    {
        /// <summary>
        /// 消息
        /// </summary>
        protected override IMessageModule message { get { return env.modules.Message; } }
        /// <summary>
        /// 环境
        /// </summary>
        public IEnvironment env { get; private set; }
        /// <summary>
        /// 标记
        /// </summary>
        public override string flag { get { return rootFlag; } }
        /// <summary>
        /// 数据容器
        /// </summary>
        protected override IValuesContainer container { get { return env.container; } }
        /// <summary>
        /// 环境类型
        /// </summary>
        protected abstract EnvironmentType envType { get; }
        /// <summary>
        /// ctor
        /// </summary>
        protected RootEntity() {
          
        }
        /// <summary>
        /// 初始化
        /// </summary>
        void ISingleton.OnSingletonInit()
        {
            env = Framework.CreateEnv(envType);
            env.InitWithAttribute();
            env.container.SubscribeInstance(env, "", false);
            env.container.SubscribeInstance<T>(this as T, "", false);
            BeforeAwake();
            Awake();

            AfterAwake();
        }
       
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static RootEntity<T> Initialize()
        {
            //var ins = SingletonProperty<T>.instance;
            //System.Console.WriteLine("         ");
            return root;
            //return SingletonProperty<T>.instance;
        }
        /// <summary>
        /// 刷新
        /// </summary>
        public static void Update()
        {
            root.env.Update();
        }
        /// <summary>
        /// 消灭
        /// </summary>
        public static void Destory()
        {
            root.env.Dispose();
            SingletonCollection.Dispose<T>();
        }

        /// <summary>
        /// 获取跟工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        public static TUtility GetRootUtility<TUtility>() where TUtility : class, IUtility
        {
            return root.container.GetValue<TUtility>(Entity.rootFlag);
          
        }
        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="type"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static bool SubscribeRootMessage<type>(IMessageListener listener)
        {
            return root.message.Subscribe<type>(listener);
        }
        /// <summary>
        /// 取消监听
        /// </summary>
        /// <typeparam name="type"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static bool UnSubscribeRootMessage<type>(IMessageListener listener)
        {
            return root.message.UnSubscribe<type>(listener);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static IMessage PublishRootMessage<Type>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return root.message.Publish<Type>(args, priority);
        }
    }
}
