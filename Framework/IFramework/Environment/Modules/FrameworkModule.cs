using System;
using System.Reflection;

namespace IFramework.Modules
{
    /// <summary>
    /// 模块
    /// </summary>
    public abstract class FrameworkModule : Unit
    {
        /// <summary>
        /// 默认名字
        /// </summary>
        public const string defaultName = "default";
        /// <summary>
        /// 阻止 New
        /// </summary>
        protected FrameworkModule() { }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <param name="name">模块名称</param>
        /// <returns></returns>
        public static FrameworkModule CreatInstance(Type type,string name= defaultName)
        {
            FrameworkModule moudle = Activator.CreateInstance(type) as FrameworkModule;
           
            if (moudle != null)
            {
                moudle._binded = false;
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
        /// <param name="name">模块名称</param>
        /// <returns></returns>
        public static T CreatInstance<T>(string name= defaultName) where T : FrameworkModule
        {
            return CreatInstance(typeof(T), name) as T;
        }

        /// <summary>
        /// 绑定模块容器
        /// </summary>
        /// <param name="container"></param>
        public void Bind(IFrameworkModuleContainer container)
        {
            if (this._container!=null)
            {
                Log.E(string.Format("Have Bind One Container chunck: You Can UnBind First"));
                return;
            }

            if ((container as FrameworkModuleContainer).SubscribeModule(this))
            {
                this._binded = true;
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
        private bool _binded;

        /// <summary>
        /// 优先级（越大释放越早释放）
        /// </summary>
        public abstract int priority { get; }

        /// <summary>
        /// 是否绑定了
        /// </summary>
        public bool binded { get { return _binded; } }
        /// <summary>
        /// 模块所处的容器
        /// </summary>
        public IFrameworkModuleContainer container { get { return _container; } }
        /// <summary>
        /// 名字
        /// </summary>
        public string name { get; set; }
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
