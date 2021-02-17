using System;
using System.IO;
using System.Text;

namespace IFramework.Resource
{
    /// <summary>
    /// 文本加载器
    /// </summary>
     class FileTextLoader : ResourceLoader<string> 
    {
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding encoding;
        /// <summary>
        /// 加载
        /// </summary>
        protected override void OnLoad()
        {
            try
            {
                Tresource.Tvalue = File.ReadAllText(path,encoding);
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
