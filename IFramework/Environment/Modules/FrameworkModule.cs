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
                if (moudle is UpdateFrameworkModule)
                {
                    (moudle as UpdateFrameworkModule).enable = true;
                }
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
        public void Bind(IFrameworkModuleContainer container)
        {
            if (this._container!=null)
            {
                Log.E(string.Format("Have Bind One Container chunck: {0},You Can UnBind First", this._container.chunck));
                return;
            }

            if ((container as FrameworkModuleContainer).SubscribeModule(this))
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
                (this._container as FrameworkModuleContainer).UnSubscribeBindModule(this);
                this._binded = false;
                this._container = null;
            }
            if (dispose)
                Dispose();
        }

        private IFrameworkModuleContainer _container;
        private string _chunck;
        private string _moudleType;
        private bool _binded;

        /// <summary>
        /// 优先级（越大释放越早释放）
        /// </summary>
        public abstract int priority { get; }
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
        public IFrameworkModuleContainer container { get { return _container; } }
       
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            UnBind(false);
            base.Dispose();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Awake();

    }
}
