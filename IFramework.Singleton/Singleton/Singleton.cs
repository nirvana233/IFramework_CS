namespace IFramework.Singleton
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        protected static T instance;
        static object lockObj = new object();
        public static T Instance
        {
            get
            {
                lock (lockObj)
                    if (instance == null)
                    {
                        instance = SingletonCreator.CreateSingleton<T>();
                        SingletonCollection.Set(instance);
                    }
                return instance;
            }
        }
        protected Singleton() { }

        protected virtual void OnSingletonInit() { }
        public virtual void Dispose()
        {
            instance = null;
        }

        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
