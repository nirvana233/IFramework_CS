using IFramework.Injection;
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
            container.SubscribeInstance(env, "",false);
            container.SubscribeInstance<T>(this as T, "", false);
            Awake();
            AfterAwake();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static RootEntity<T> Initialize()
        {
            return root;
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
    }
}
