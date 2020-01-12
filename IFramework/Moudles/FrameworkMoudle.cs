using System;
using System.Reflection;

namespace IFramework.Moudles
{
    public abstract class FrameworkMoudle : IFrameworkMoudle
    {
        public static T CreatInstance<T>(string chunck = "Framework",bool autoBind=true) where T : FrameworkMoudle
        {
            T t = Activator.CreateInstance(typeof(T),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, null, null) as T;
            if (t != null)
            {
                t._disposed = false;
                t._name = string.Format("{0}.{1}", chunck, t.GetType().Name);
                t.Awake();
                t.enable = true;
                if (autoBind)
                    t.BindFramework();
            }
            return t;
        }
        private string _name;
        private bool _disposed;
        private bool _enable;

        public string name { get { return _name; } }
        public bool disposed { get { return _disposed; } }
        public bool enable
        {
            get { return _enable; }
            set
            {
                if (_enable != value)
                    _enable = value;
                if (_enable)
                    OnEnable();
                else
                    OnDisable();
            }
        }

        public void SetActive(bool enable) { this.enable = enable; }

        protected FrameworkMoudle() { }

        public void Dispose()
        {
            enable = false;
            _disposed = true;
            _name = string.Empty;
            OnDispose();
        }
        public void Update()
        {
            if (!enable || disposed) return;
            OnUpdate();
        }



        protected abstract void Awake();
        protected abstract void OnDispose();
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected abstract void OnUpdate();

    }

}
