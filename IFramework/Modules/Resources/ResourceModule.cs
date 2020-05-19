using System;
using System.Collections.Generic;
using IFramework.Pool;

namespace IFramework.Modules.Resources
{
    /// <summary>
    /// 资源模块
    /// </summary>
    [FrameworkVersion(54)]
    [ScriptVersionUpdate(54,"缩短泛型方法")]
    public class ResourceModule : UpdateFrameworkModule
    {
        class ResourceLoaderPool : BaseTypePool<ResourceLoader> { }
        class ResourceGroups : IDisposable
        {
            private List<ResourceGroup> _groups;
            private readonly ResourceModule moudule;
            public int trick;
            private int _curTrick = 0;
            private LockParam _lock = new LockParam();

            public ResourceGroups(ResourceModule moudule, int trick)
            {
                this.moudule = moudule;
                this.trick = trick;
                _groups = new List<ResourceGroup>();
            }

            private ResourceGroup FinGroup(string groupName)
            {
                ResourceGroup _group = _groups.Find((g) => { return g.name == groupName; });

                if (_group == null)
                {
                    _group = new ResourceGroup(groupName);
                    _group.moudule = moudule;
                    _groups.Add(_group);
                }
                return _group;
            }



            public Resource Load(Type loaderType,string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
            {
                using (new LockWait(ref _lock))
                {
                    ResourceGroup _group = FinGroup(groupName);
                    if (dependences == null || dependences.Length == 0 || dependenceLoader == null)
                        return _group.Load(loaderType,name, path, null, null).resource;
                    return _group.Load(loaderType,name, path, dependences,
                        (dp) => {
                            return dependenceLoader.Invoke(dp).loader;
                        }).resource;
                }
            }
            public Resource LoadAsync(Type loaderType, string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
            {
                using (new LockWait(ref _lock))
                {
                    ResourceGroup _group = FinGroup(groupName);

                    if (dependences == null || dependences.Length == 0 || dependenceLoader == null)
                        return _group.Load(loaderType,name, path, null, null).resource;
                    return _group.Load(loaderType,name, path, dependences, (dp) => {
                        return dependenceLoader.Invoke(dp).loader;
                    }).resource;

                }
            }
            public Resource Load<LoaderType>(string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
                where LoaderType : ResourceLoader
            {
                using (new LockWait(ref _lock))
                {
                    ResourceGroup _group = FinGroup(groupName);
                    if (dependences == null || dependences.Length == 0 || dependenceLoader == null)
                        return _group.Load<LoaderType>(name, path, null, null).resource;
                    return _group.Load<LoaderType>(name, path, dependences,
                        (dp) => {
                            return dependenceLoader.Invoke(dp).loader;
                        }).resource;
                }
            }
            public Resource LoadAsync<LoaderType>(string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
                where LoaderType : AsyncResourceLoader
            {
                using (new LockWait(ref _lock))
                {
                    ResourceGroup _group = FinGroup(groupName);

                    if (dependences == null || dependences.Length == 0 || dependenceLoader == null)
                        return _group.Load<LoaderType>(name, path, null, null).resource;
                    return _group.Load<LoaderType>(name, path, dependences, (dp) => {
                        return dependenceLoader.Invoke(dp).loader;
                    }).resource;

                }
            }
            public Resource LoadDependence(Resource resource, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
            {
                using (new LockWait(ref _lock))
                {
                    ResourceGroup _group = FinGroup(resource.groupName);

                    if (dependences == null || dependences.Length == 0 || dependenceLoader == null)
                        return _group.LoadDependence(resource.loader, null, null).resource;
                    return _group.LoadDependence(resource.loader, dependences, (dp) => {
                        return dependenceLoader.Invoke(dp).loader;
                    }).resource;
                }
            }

            public void Update()
            {
                using (new LockWait(ref _lock))
                {
                    _curTrick++;
                    if (_curTrick >= trick)
                    {
                        _curTrick = 0;
                        foreach (var item in _groups)
                        {
                            item.ClearUnuseRes();
                        }
                    }
                }
            }

            public void Dispose()
            {
                using (new LockWait(ref _lock))
                {
                    for (int i = 0; i < _groups.Count; i++)
                    {
                        ResourceGroup _group = _groups[i];
                        _group.Dispose();
                    }
                    _groups.Clear();
                }
            }
        }
        internal ResourceLoader AllocateLoader(Type type)
        {
            ResourceLoader loader = _loaderPool.Get(type);

            loader.Reset();
            return loader;
        }
        internal void RecyleLoader<T>(T loader) where T : ResourceLoader
        {
            loader.Reset();
            _loaderPool.Set(loader);
        }


        private ResourceGroups _groups;
        private ResourceLoaderPool _loaderPool;

        /// <summary>
        /// gc
        /// </summary>
        public int trick { get { return _groups.trick; } set { _groups.trick = value; } }



        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="LoaderType"></typeparam>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource Load<LoaderType>(string groupName, string name, string path) where LoaderType : ResourceLoader
        {
            return _groups.Load<LoaderType>(groupName, name, path, null, null);
        }
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="LoaderType"></typeparam>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource LoadAsync<LoaderType>(string groupName, string name, string path) where LoaderType : AsyncResourceLoader
        {
            return _groups.LoadAsync<LoaderType>(groupName, name, path, null, null);
        }
        /// <summary>
        /// 加载资源(泛型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="LoaderType"></typeparam>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource<T> Load<T, LoaderType>(string groupName, string name, string path)
            where LoaderType : ResourceLoader<T>
        {
            return _groups.Load<LoaderType>(groupName, name, path,null,null) as Resource<T>;
        }
        /// <summary>
        /// 异步加载资源(泛型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="LoaderType"></typeparam>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource<T> LoadAsync<T, LoaderType>(string groupName, string name, string path)
            where LoaderType : AsyncResourceLoader<T>
        {
            return _groups.LoadAsync<LoaderType>(groupName, name, path, null, null) as Resource<T>;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="LoaderType"></typeparam>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="dependences"></param>
        /// <param name="dependenceLoader"></param>
        /// <returns></returns>
        public Resource Load<LoaderType>(string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader) where LoaderType : ResourceLoader
        {
            return _groups.Load<LoaderType>(groupName, name, path, dependences, dependenceLoader);
        }
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="LoaderType"></typeparam>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="dependences"></param>
        /// <param name="dependenceLoader"></param>
        /// <returns></returns>
        public Resource LoadAsync<LoaderType>(string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader) where LoaderType : AsyncResourceLoader
        {
            return _groups.LoadAsync<LoaderType>(groupName, name, path, dependences, dependenceLoader);
        }
        /// <summary>
        /// 加载资源(泛型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="LoaderType"></typeparam>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="dependences"></param>
        /// <param name="dependenceLoader"></param>
        /// <returns></returns>
        public Resource<T> Load<T, LoaderType>(string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
            where LoaderType : ResourceLoader<T>
        {
            return _groups.Load<LoaderType>(groupName, name, path, dependences, dependenceLoader) as Resource<T>;
        }
        /// <summary>
        /// 异步加载资源(泛型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="LoaderType"></typeparam>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="dependences"></param>
        /// <param name="dependenceLoader"></param>
        /// <returns></returns>
        public Resource<T> LoadAsync<T, LoaderType>(string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
            where LoaderType : AsyncResourceLoader<T>
        {
            return _groups.LoadAsync<LoaderType>(groupName, name, path, dependences, dependenceLoader) as Resource<T>;
        }




        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="loaderType"></param>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource Load(Type loaderType, string groupName, string name, string path) 
        {
            return _groups.Load(loaderType, groupName, name, path, null, null);
        }
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="loaderType"></param>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource LoadAsync(Type loaderType, string groupName, string name, string path) 
        {
            return _groups.LoadAsync(loaderType,groupName, name, path, null, null);
        }
        /// <summary>
        /// 加载资源(泛型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loaderType"></param>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource<T> Load<T>(Type loaderType, string groupName, string name, string path)
        {
            return _groups.Load(loaderType,groupName, name, path, null, null) as Resource<T>;
        }
        /// <summary>
        /// 异步加载资源(泛型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loaderType"></param>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource<T> LoadAsync<T>(Type loaderType, string groupName, string name, string path)
        {
            return _groups.LoadAsync(loaderType,groupName, name, path, null, null) as Resource<T>;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="loaderType"></param>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="dependences"></param>
        /// <param name="dependenceLoader"></param>
        /// <returns></returns>
        public Resource Load(Type loaderType, string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
        {
            return _groups.Load(loaderType,groupName, name, path, dependences, dependenceLoader);
        }
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="loaderType"></param>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="dependences"></param>
        /// <param name="dependenceLoader"></param>
        /// <returns></returns>
        public Resource LoadAsync(Type loaderType, string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader) 
        {
            return _groups.LoadAsync(loaderType,groupName, name, path, dependences, dependenceLoader);
        }
        /// <summary>
        /// 加载资源(泛型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loaderType"></param>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="dependences"></param>
        /// <param name="dependenceLoader"></param>
        /// <returns></returns>
        public Resource<T> Load<T>(Type loaderType, string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
        {
            return _groups.Load(loaderType,groupName, name, path, dependences, dependenceLoader) as Resource<T>;
        }
        /// <summary>
        /// 异步加载资源(泛型)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loaderType"></param>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="dependences"></param>
        /// <param name="dependenceLoader"></param>
        /// <returns></returns>
        public Resource<T> LoadAsync<T>(Type loaderType, string groupName, string name, string path, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
        {
            return _groups.LoadAsync(loaderType,groupName, name, path, dependences, dependenceLoader) as Resource<T>;
        }





        /// <summary>
        /// 加载资源依赖（第一次没法加载依赖时候）
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="dependences"></param>
        /// <param name="dependenceLoader"></param>
        /// <returns></returns>
        public Resource LoadDependence(Resource resource, Dependence[] dependences, Func<Dependence, Resource> dependenceLoader)
        {
            return _groups.LoadDependence(resource, dependences, dependenceLoader);
        }
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="resource"></param>
        public void UnLoad(Resource resource)
        {
            resource.loader.Release();
        }



#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override void Awake()
        {
            _groups = new ResourceGroups(this, 1000);
            _loaderPool = new ResourceLoaderPool();
        }
        protected override void OnDispose()
        {
            _groups.Dispose();
            _loaderPool.Dispose();
        }
        protected override void OnUpdate()
        {
            _groups.Update();
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释


    }
}
