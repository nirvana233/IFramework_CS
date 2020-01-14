using System;
using System.Reflection;

namespace IFramework.Moudles
{
    public abstract class FrameworkMoudle : IFrameworkMoudle
    {
        public static T CreatInstance<T>(string chunck = "Framework",bool bind=true) where T : FrameworkMoudle
        {
            return CreatInstance(typeof(T),chunck,bind) as T;
        }
        public static FrameworkMoudle CreatInstance(Type type,string chunck = "Framework", bool bind = true) 
        {
            FrameworkMoudle moudle = Activator.CreateInstance(type,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, null, null) as FrameworkMoudle;
            if (moudle != null)
            {
                moudle._disposed = false;
                moudle._moudleType = moudle.GetType().Name;
                moudle._chunck = chunck;

                moudle._name = string.Format("{0}.{1}", moudle._chunck, moudle._moudleType);
                moudle.Awake();
                moudle.enable = true;
                if (bind)
                    moudle.BindFramework();
            }
            return moudle;
        }
        private string _name;
        private bool _disposed;
        private bool _enable;
        private string _chunck;
        private string _moudleType;

        public string moudeType { get { return _moudleType; } }
        public string chunck { get { return _chunck; } }
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
