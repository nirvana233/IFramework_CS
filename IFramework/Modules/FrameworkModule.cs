using System;
using System.Reflection;

namespace IFramework.Modules
{
    public abstract class FrameworkModule : IFrameworkModule
    {
        public static FrameworkModule CreatInstance(Type type, string chunck,string name="")
        {
            FrameworkModule moudle = Activator.CreateInstance(type,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, null, null) as FrameworkModule;

            if (moudle != null)
            {
                moudle._binded = false;
                moudle._disposed = false;
                moudle._chunck = chunck;
                moudle._moudleType = moudle.GetType().Name;
                if (string.IsNullOrEmpty(name))
                    moudle._name = string.Format("{0}.{1}", moudle._chunck, moudle._moudleType);
                else
                    moudle._name = name;

                moudle.Awake();
                moudle.enable = true;
            }
            else
                Log.E(string.Format("Type: {0} Non Public Ctor With 0 para Not Find", type));

            return moudle;
        }
        public static T CreatInstance<T>(string chunck ,string name="") where T : FrameworkModule
        {
            return CreatInstance(typeof(T), chunck,name) as T;
        }


        public void Bind(FrameworkModuleContainer container)
        {
            if (this._container!=null)
            {
                Log.E(string.Format("Have Bind One Container chunck: {0},You Can UnBind First", this._container.chunck));
                return;
            }

            if (container.AddModule(this))
            {
                this._binded = true;
                this._chunck = container.chunck;
                //this._name = string.Format("{0}.{1}", this._chunck, this._moudleType);
                this._container = container;
            }
            
        }
        public void UnBind(bool dispose=true)
        {
            if (!binded) return;
            if (binded && this._container != null)
            {
                this._container.RemoveBindModule(this);
                this._binded = false;
                this._container = null;
            }
            if (dispose)
                Dispose();
        }

        private FrameworkModuleContainer _container;
        private string _name;
        private bool _disposed;
       
        private bool _enable;
        private string _chunck;
        private string _moudleType;
        private bool _binded;
        protected virtual bool needUpdate { get { return true; } }

        public string moudeType { get { return _moudleType; } }
        public string chunck { get { return _chunck; } }
        public string name { get { return _name; } set { _name = value; } }
        public bool binded { get { return _binded; } }
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
        public FrameworkModuleContainer container { get { return _container; } }

        public void SetActive(bool enable) { this.enable = enable; }

        protected FrameworkModule() { }

        public void Dispose()
        {
            enable = false;
            OnDispose();
            UnBind(false);
            _disposed = true;
            _name = string.Empty;
        }
        public void Update()
        {
            if (!needUpdate || !enable || disposed) return;
            OnUpdate();
        }



        protected abstract void Awake();
        protected abstract void OnDispose();
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected abstract void OnUpdate();

    }

}
