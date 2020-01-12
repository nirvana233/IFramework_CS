namespace IFramework
{
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

}
