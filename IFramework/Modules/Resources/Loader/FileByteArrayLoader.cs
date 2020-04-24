using System;
using System.IO;

namespace IFramework.Modules.Resources
{
    /// <summary>
    /// 文件流加载器
    /// </summary>
    public class FileByteArrayLoader : ResourceLoader<byte[], ByteArryResource>
    {
        /// <summary>
        /// 加载
        /// </summary>
        protected override void OnLoad()
        {
            try
            {
                Tresource.value = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                ThrowErr(e.Message);
            }
            finally
            {
                _isdone = true;
                _progress = 1;
            }
        }
    }
}
