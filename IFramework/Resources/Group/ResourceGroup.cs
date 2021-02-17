using System;
using System.Collections.Generic;

namespace IFramework.Resource
{
    /// <summary>
    /// 资源组
    /// </summary>
    public abstract class ResourceGroup : IResourceGroup
    {
        class ResourceLoaderPool : BaseTypePool<ResourceLoader> { }

        /// <summary>
        ///名字
        /// </summary>
        public string name { get; set; }
        private LockParam _lock;
        private ResourceLoaderPool _loaderPool;
        private Dictionary<string, ResourceLoader> _loaderMap;
        private Queue<ResourceLoader> _remove;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        protected ResourceGroup(string name)
        {
            this.name = name;
            _lock = new LockParam();
            _remove = new Queue<ResourceLoader>();
            _loaderMap = new Dictionary<string, ResourceLoader>();
            _loaderPool = new ResourceLoaderPool();
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            using (new LockWait(ref _lock))
            {
                OnDispose();
                _remove.Clear();
                foreach (var item in _loaderMap.Values)
                {
                    _remove.Enqueue(item);
                }
                while (_remove.Count > 0)
                {
                    ResourceLoader _loader = _remove.Dequeue();
                    _loader.UnLoad();
                    RecyleLoader(_loader);
                }
                _loaderMap.Clear();
            }
            _loaderPool.Dispose();
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="loaderType"></param>
        /// <param name="path"></param>
        /// <param name="beforeLoad"></param>
        /// <returns></returns>
        protected ResourceLoader Load(Type loaderType, string path,Action<ResourceLoader> beforeLoad=null)
        {
            using (new LockWait(ref _lock))
            {
                ResourceLoader _loader;
                if (!_loaderMap.TryGetValue(path, out _loader))
                {
                    _loader = AllocateLoader(loaderType);
                    _loader.Config(path);
                    if (beforeLoad!=null)
                    {
                        beforeLoad(_loader);
                    }
                    _loader.Load();
                    _loaderMap.Add(_loader.path, _loader);
                }
                _loader.Retain();
                return _loader;
            }
        }




        /// <summary>
        /// 删除无用资源
        /// </summary>
        public void ClearUnuseResources()
        {
            using (new LockWait(ref _lock))
            {
                _remove.Clear();
                foreach (var item in _loaderMap.Values)
                {
                    if ((item.isdone && item.refCount == 0) || !string.IsNullOrEmpty(item.error))
                    {
                        _remove.Enqueue(item);
                    }
                }
                while (_remove.Count > 0)
                {
                    ResourceLoader _loader = _remove.Dequeue();
                    _loader.UnLoad();
                    _loaderMap.Remove(_loader.path);
                    RecyleLoader(_loader);
                }
            }

        }
        /// <summary>
        /// 分配加载器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected ResourceLoader AllocateLoader(Type type)
        {
            return _loaderPool.Get(type) as ResourceLoader;
        }
        /// <summary>
        /// 回收加载器
        /// </summary>
        /// <param name="loader"></param>
        protected void RecyleLoader(ResourceLoader loader)
        {
            loader.Reset();
            _loaderPool.Set(loader);
        }

        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() { }

    }


}
