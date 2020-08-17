using System;
using System.IO;

namespace IFramework.Resource
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
                Tresource.Tvalue = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                ThrowErr(e.Message);
            }
            finally
            {
                isdone = true;
                progress = 1;
            }
        }
      
    }
}
