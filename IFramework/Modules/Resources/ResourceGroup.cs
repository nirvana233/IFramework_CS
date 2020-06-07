using System;
using System.Collections.Generic;

namespace IFramework.Modules.Resources
{
    class ResourceGroup :IDisposable
    {
        internal ResourceModule moudule { get; set; }
        private Dictionary<string, ResourceLoader> _loaderMap;
        public string name { get; set; }
        private LockParam _lock = new LockParam();

        internal ResourceGroup(string name)
        {
            this.name = name;
            _loaderMap = new Dictionary<string, ResourceLoader>();
        }

        private ResourceLoader AllocateLoader(Type type) 
        {
            var loader= moudule.AllocateLoader(type);
            loader.group = this;
            return loader;

        }
        private void RecyleLoader<T>(T loader) where T : ResourceLoader
        {
            moudule.RecyleLoader<T>(loader);
        }
        private List<ResourceLoader> LoadDependences(Dependence[] dependences, Func<Dependence, ResourceLoader> dependenceLoader)
        {
            List<ResourceLoader> _loaders = null;
            if (dependences != null && dependences.Length > 0 && dependenceLoader!=null)
            {
                _loaders = new List<ResourceLoader>();
                for (int i = 0; i < dependences.Length; i++)
                {
                    Dependence _dp = dependences[i];
                    if (dependenceLoader != null)
                    {
                        ResourceLoader _loader = dependenceLoader.Invoke(_dp);
                        _loaders.Add(_loader);
                    }
                }
            }
            return _loaders;
        }



        public ResourceLoader Load<T>(string name, string path, Dependence[] dependences, Func<Dependence, ResourceLoader> dependenceLoader) where T : ResourceLoader
        {
            return Load(typeof(T), name, path, dependences, dependenceLoader);
        }
        public ResourceLoader Load(Type loaderType,string name, string path, Dependence[] dependences, Func<Dependence, ResourceLoader> dependenceLoader) 
        {
            using (new LockWait(ref _lock))
            {
                ResourceLoader _loader;
                if (!_loaderMap.TryGetValue(name, out _loader))
                {
                    List<ResourceLoader> _loaders = LoadDependences(dependences, dependenceLoader);

                    _loader = AllocateLoader(loaderType);
                    _loader.Config(name, path, _loaders);
                    _loader.Load();
                    _loaderMap.Add(_loader.name, _loader);
                }
                _loader.Retain();
                return _loader;
            }
        }
        public ResourceLoader LoadDependence(ResourceLoader loader, Dependence[] dependences, Func<Dependence, ResourceLoader> dependenceLoader)
        {
            using (new LockWait(ref _lock))
            {
                if (!_loaderMap.ContainsKey(loader.name))
                    throw new Exception(string.Format("Not Exist Loader Type: {0} Name: {1} Path:{2} ", loader.GetType(), loader.name, loader.path));
                else
                {
                    List<ResourceLoader> _loaders = LoadDependences(dependences, dependenceLoader);
                    loader.Config(_loaders);
                }
                return loader;
            }
        }


        private Queue<ResourceLoader> _remove = new Queue<ResourceLoader>();
        internal void ClearUnuseRes()
        {
            using (new LockWait(ref _lock))
            {
                _remove.Clear();
                foreach (var item in _loaderMap.Values)
                {
                    if ((item.isdone && item.refCount == 0 ) || !string.IsNullOrEmpty(item.error))
                    {
                        _remove.Enqueue(item);
                    }
                }
                while (_remove.Count > 0)
                {
                    ResourceLoader _loader = _remove.Dequeue();
                    _loader.UnLoad();
                    _loaderMap.Remove(_loader.name);
                    RecyleLoader(_loader);
                }
            }

        }

        public void Dispose()
        {
            using (new LockWait(ref _lock))
            {
                foreach (var item in _loaderMap.Values)
                {
                    item.UnLoad();
                }
                _loaderMap.Clear();
            }
        }
    }
}
