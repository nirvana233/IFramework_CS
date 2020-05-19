namespace IFramework.Modules.Resources
{
    /// <summary>
    /// 资源
    /// </summary>
    public class Resource
    {

        internal ResourceLoader loader;
        /// <summary>
        /// 进度
        /// </summary>
        public float progress { get { return loader.progress; } }
        /// <summary>
        /// 名字
        /// </summary>
        public string name { get { return loader.name; } }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool isdone { get { return loader.isdone; } }
        /// <summary>
        /// 路径
        /// </summary>
        public string path { get { return loader.path; } }
        /// <summary>
        /// 资源组
        /// </summary>
        public string groupName { get { return loader.group.name; } }
        /// <summary>
        /// 模块
        /// </summary>
        public ResourceModule moudule { get { return loader.group.moudule; } }
        /// <summary>
        /// 依赖资源
        /// </summary>
        public Resource[] dependences { get { return loader.GetDependences(); } }
        /// <summary>
        /// 错误
        /// </summary>
        public string error { get { return loader.error; } }
        /// <summary>
        /// 释放资源 
        /// </summary>
        public void Release()
        {
            this.loader.Release();
        }
        /// <summary>
        /// 资源
        /// </summary>
        public object o_value { get; set; }
    }
    /// <summary>
    /// 泛型资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Resource<T> : Resource
    {
        //private T _value;
        /// <summary>
        /// 资源值
        /// </summary>
        public T value { get { return (T)o_value; } set { o_value = value; } }
    }
}
