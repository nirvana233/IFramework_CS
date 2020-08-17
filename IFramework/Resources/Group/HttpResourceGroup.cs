namespace IFramework.Resource
{
    /// <summary>
    /// Http资源加载组
    /// </summary>
    public class HttpResourceGroup : ResourceGroup
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        public HttpResourceGroup(string name) : base(name)
        {
        }

        /// <summary>
        /// 加载字节
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource<byte[]> LoadBytes(string path)
        {
            return Load(typeof(HttpBytesLoader), path).resource as Resource<byte[]>;
        }
        /// <summary>
        /// 异步加载字节
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource<byte[]> LoadBytesAsync(string path)
        {
            return Load(typeof(AsyncHttpBytesLoader), path).resource as Resource<byte[]>;
        }
        /// <summary>
        /// 加载文字
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource<string> LoadText(string path)
        {
            return Load(typeof(HttpTextLoader), path).resource as Resource<string>;
        }
        /// <summary>
        /// 异步加载文字
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource<string> LoadTextAsync(string path)
        {
            return Load(typeof(AsyncHttpTextLoader), path).resource as Resource<string>;
        }

    }


}
