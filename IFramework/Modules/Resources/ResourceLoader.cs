using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework.Modules.Resources
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public abstract class ResourceLoader : IDisposable
    {
        private Resource _res;
        private List<ResourceLoader> _dependences;
        internal string error;

        internal int RefCount { get; private set; }
        internal ResourceGroup group { get; set; }

        /// <summary>
        /// 自生是否完成
        /// </summary>
        protected virtual bool _isdone { get; set; }
        /// <summary>
        /// 自身进度
        /// </summary>
        protected virtual float _progress { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string name { get; private set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string path { get; private set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool isdone
        {
            get
            {
                for (int i = 0; i < _dependences.Count; i++)
                {
                    if (!_dependences[i].isdone)
                        return false;
                }
                return _isdone;
            }
        }
        /// <summary>
        /// 进度
        /// </summary>
        public float progress
        {
            get
            {
                float _pro = 0;
                for (int i = 0; i < _dependences.Count; i++)
                {
                    _pro += _dependences[i].progress;
                }
                return (_progress + _pro) / _dependences.Count + 1;
            }
        }
        /// <summary>
        /// 资源
        /// </summary>
        public Resource resource { get { return _res; } protected set { _res = value; } }

        /// <summary>
        /// ctor
        /// </summary>
        public ResourceLoader()
        {
            RefCount = 0;
            _dependences = new List<ResourceLoader>();
            _isdone = false;
            _progress = 0;
            _res = CreateResource();
            _res.loader = this;
        }
        /// <summary>
        /// 最终释放
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            _res = null;
            _dependences = null;
            _isdone = false;
            _progress = 0;
        }


        internal void Config(string name, string path, List<ResourceLoader> loaders)
        {
            this.name = name;
            this.path = path;
            Config(loaders);
        }
        internal void Config(List<ResourceLoader> loaders)
        {
            if (loaders != null && loaders.Count > 0)
            {
                for (int i = 0; i < loaders.Count; i++)
                {
                    _dependences.Add(loaders[i]);
                }
            }
        }

        internal Resource[] GetDependences()
        {
            return _dependences.Select((dp) => { return dp.resource; }).ToArray();
        }
        internal void Retain() { ++RefCount; }
        internal void Release() { --RefCount; }
        internal void Load() { OnLoad(); }
        internal void UnLoad()
        {
            OnUnLoad();
            for (int i = 0; i < _dependences.Count; i++)
                _dependences[i].Release();
            _dependences.Clear();
            _isdone = false;
            _progress = 0;
        }
        internal void Reset()
        {
            OnReset();
            _progress = 0;
            _isdone = false;
            error = name = path = string.Empty;
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
        /// 重置数据
        /// </summary>
        protected virtual void OnReset() { }
        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() { }
        /// <summary>
        /// 抛出错误
        /// </summary>
        /// <param name="err"></param>
        protected void ThrowErr(string err)
        {
            this.error = err;
            Log.E(err);
        }

    }
    /// <summary>
    ///  泛型资源加载器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public abstract class ResourceLoader<T, V> : ResourceLoader where V : Resource<T>
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
            return Activator.CreateInstance<V>();
        }
        /// <summary>
        /// 卸载时
        /// </summary>
        protected override void OnUnLoad()
        {
            if (Tresource.value != null)
            {
                Tresource.value = default(T);
            }
        }
    }
}
