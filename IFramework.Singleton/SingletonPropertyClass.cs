namespace IFramework.Singleton
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public abstract class SingletonPropertyClass<T> : ISingleton where T : SingletonPropertyClass<T>
    {
        protected static T Instance { get { return SingletonProperty<T>.Instance; } }
        protected SingletonPropertyClass() { }
        protected virtual void OnSingletonInit()
        {

        }
        public virtual void Dispose()
        {
            SingletonProperty<T>.Dispose();
        }


        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
