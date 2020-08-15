namespace IFramework.Resources
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public abstract class ResourceLoader 
    {
        private Resource _res;
        internal string error;

        internal int refCount { get; private set; }
        internal ResourceGroup group { get; set; }

        private  bool _isdone { get; set; }
        private float _progress { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string path { get; private set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool isdone { get { return _isdone; } protected set { _isdone = value; } }
        /// <summary>
        /// 进度
        /// </summary>
        public virtual float progress { get { return _progress; } protected set { _progress = value; } }
        /// <summary>
        /// 资源
        /// </summary>
        public Resource resource { get { return _res; } }

        /// <summary>
        /// ctor
        /// </summary>
        public ResourceLoader()
        {
            refCount = 0;
            isdone = false;
            progress = 0;
            _res = CreateResource();
            _res.loader = this;
        }


        internal void Config(string path)
        {
            this.path = path;
        }

        internal void Retain() { ++refCount; }
        internal void Release() { --refCount; }
        internal void Load() { OnLoad(); }
        internal void UnLoad()
        {
            OnUnLoad();
            isdone = false;
            progress = 0;
        }
        internal void Reset()
        {
            progress = 0;
            isdone = false;
            error = path = string.Empty;
            group = null;
        }

        /// <summary>
        /// 创建泛型资源实例
        /// </summary>
        /// <returns></returns>
        protected abstract Resource CreateResource();
        /// <summary>
        /// 加载资源
        /// </summary>
        protected abstract void OnLoad();
        /// <summary>
        /// 卸载资源
        /// </summary>
        protected abstract void OnUnLoad();

        /// <summary>
        /// 抛出错误
        /// </summary>
        /// <param name="err"></param>
        protected void ThrowErr(string err)
        {
            this.error = err;
            _isdone = true;
            Log.E(err);
        }

    }
    /// <summary>
    /// 泛型资源加载器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ResourceLoader<T> : ResourceLoader
    {
        /// <summary>
        /// 泛型资源
        /// </summary>
        public Resource<T> Tresource { get { return resource as Resource<T>; } }
        /// <summary>
        /// 创建泛型资源实例
        /// </summary>
        /// <returns></returns>
        protected override Resource CreateResource()
        {
            return new Resource<T>();
        }
        /// <summary>
        /// 卸载时
        /// </summary>
        protected override void OnUnLoad()
        {
            if (Tresource.Tvalue != null)
            {
                Tresource.Tvalue = default(T);
            }
        }
    }


}
