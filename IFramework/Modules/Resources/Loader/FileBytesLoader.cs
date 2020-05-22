using System;
using System.IO;

namespace IFramework.Modules.Resources
{
    /// <summary>
    /// 文件流加载器
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class FileBytesLoader : ResourceLoader<byte[]>
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
