namespace IFramework.Resources
{
    /// <summary>
    /// 异步资源加载器
    /// </summary>
    public abstract class AsyncResourceLoader : ResourceLoader { }
    /// <summary>
    /// 泛型异步资源加载器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AsyncResourceLoader<T> : AsyncResourceLoader 
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
        /// 卸载
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
