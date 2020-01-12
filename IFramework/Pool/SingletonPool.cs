namespace IFramework
{
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
}
