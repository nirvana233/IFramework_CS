using System;
using System.Reflection;

namespace IFramework.Modules
{
    /// <summary>
    /// 模块
    /// </summary>
    public abstract class FrameworkModule : FrameworkObject
    {
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <param name="chunck">代码块</param>
        /// <param name="name">模块名称</param>
        /// <returns></returns>
        public static FrameworkModule CreatInstance(Type type, string chunck,string name="")
        {
            FrameworkModule moudle = Activator.CreateInstance(type,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, null, null) as FrameworkModule;

            if (moudle != null)
            {
                moudle._binded = false;
                //moudle._disposed = false;
                moudle._chunck = chunck;
                moudle._moudleType = moudle.GetType().Name;
                if (string.IsNullOrEmpty(name))
                    moudle.name = string.Format("{0}.{1}", moudle._chunck, moudle._moudleType);
                else
                    moudle.name = name;

                moudle.Awake();
                moudle.enable = true;
            }
            else
                Log.E(string.Format("Type: {0} Non Public Ctor With 0 para Not Find", type));

            return moudle;
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="chunck">代码块</param>
        /// <param name="name">模块名称</param>
        /// <returns></returns>
        public static T CreatInstance<T>(string chunck ,string name="") where T : FrameworkModule
        {
            return CreatInstance(typeof(T), chunck,name) as T;
        }

        /// <summary>
        /// 绑定模块容器
        /// </summary>
        /// <param name="container"></param>
        public void Bind(FrameworkModuleContainer container)
        {
            if (this._container!=null)
            {
                Log.E(string.Format("Have Bind One Container chunck: {0},You Can UnBind First", this._container.chunck));
                return;
            }

            if (container.SubscribeModule(this))
            {
                this._binded = true;
                this._chunck = container.chunck;
                //this._name = string.Format("{0}.{1}", this._chunck, this._moudleType);
                this._container = container;
            }
            
        }
        /// <summary>
        /// 解除绑定模块容器
        /// </summary>
        /// <param name="dispose"></param>
        public void UnBind(bool dispose=true)
        {
            if (!binded) return;
            if (binded && this._container != null)
            {
                this._container.UnSubscribeBindModule(this);
                this._binded = false;
                this._container = null;
            }
            if (dispose)
                Dispose();
        }

        private FrameworkModuleContainer _container;

       
        private bool _enable;
        private string _chunck;
        private string _moudleType;
        private bool _binded;
        /// <summary>
        /// 是否需要不断刷新
        /// </summary>
        protected virtual bool needUpdate { get { return true; } }
        /// <summary>
        /// 模块类型
        /// </summary>
        public string moudeType { get { return _moudleType; } }
        /// <summary>
        /// 代码块
        /// </summary>
        public string chunck { get { return _chunck; } }
        /// <summary>
        /// 是否绑定了
        /// </summary>
        public bool binded { get { return _binded; } }
        /// <summary>
        /// 模块所处的容器
        /// </summary>
        public FrameworkModuleContainer container { get { return _container; } }
        /// <summary>
        /// 开启关闭 Update
        /// </summary>
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
        /// <summary>
        /// 改变 enable
        /// </summary>
        /// <param name="enable"></param>
        public void SetActive(bool enable) { this.enable = enable; }

        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            Dispose(() => {
                enable = false;
                OnDispose();
            }, ()=> {
                UnBind(false);
            });
        }


        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            if (!needUpdate || !enable || disposed) return;
            OnUpdate();
        }



#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected abstract void Awake();
        protected abstract new void OnDispose();
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected abstract void OnUpdate();
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

    }

}
