namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public abstract class SingletonPool<Object, Singleton> : ObjectPool<Object>, ISingleton where Singleton : SingletonPool<Object, Singleton>
    {
        protected static Singleton Instance { get { return SingletonProperty<Singleton>.Instance; } }
        protected SingletonPool() { }
        protected virtual void OnSingletonInit()
        {

        }
        protected override void OnDispose()
        {
            base.OnDispose();
            SingletonProperty<Singleton>.Dispose();
        }
        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
