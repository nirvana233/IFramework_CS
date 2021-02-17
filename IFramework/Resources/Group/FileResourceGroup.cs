using System.Text;

namespace IFramework.Resource
{
    /// <summary>
    /// 文件资源加载组
    /// </summary>
    public class FileResourceGroup : ResourceGroup
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        public FileResourceGroup(string name) : base(name)
        {
        }

        /// <summary>
        /// 加载字节
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource<byte[]> LoadBytes(string path)
        {
            return Load(typeof(FileBytesLoader), path).resource as Resource<byte[]>;
        }
        /// <summary>
        /// 异步加载字节
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Resource<byte[]> LoadBytesAsync(string path)
        {
            return Load(typeof(AsyncFileBytesLoader), path).resource as Resource<byte[]>;
        }
        /// <summary>
        /// 加载文字
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public Resource<string> LoadText(string path, Encoding encoding)
        {
            return Load(typeof(FileTextLoader), path, (loader) => { (loader as FileTextLoader).encoding = encoding; }).resource as Resource<string>;
        }
        /// <summary>
        /// 异步加载文字
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public Resource<string> LoadTextAsync(string path, Encoding encoding)
        {
            return Load(typeof(AsyncFileTextLoader), path, (loader) => { (loader as AsyncFileTextLoader).encoding = encoding; }).resource as Resource<string>;
        }

    }
}
