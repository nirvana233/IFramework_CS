namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static class SingletonProperty<T> where T : class, ISingleton
    {
        private static T instance;
        private static readonly object lockObj = new object();

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

        public static void Dispose() { instance = null; }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
