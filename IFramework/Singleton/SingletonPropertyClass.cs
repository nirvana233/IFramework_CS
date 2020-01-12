namespace IFramework
{
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

}
