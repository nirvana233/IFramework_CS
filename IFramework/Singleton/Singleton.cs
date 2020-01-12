namespace IFramework
{
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
                        SingletonPool.Set(instance);
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

}
