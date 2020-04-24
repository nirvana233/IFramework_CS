using System;
using System.IO;
using System.Text;

namespace IFramework.Modules.Resources
{
    /// <summary>
    /// 文本加载器
    /// </summary>
    /// <typeparam name="Encod"></typeparam>
    public class FileTextLoader<Encod> : ResourceLoader<string, TextResource> where Encod: Encoding,new()
    {
        private Encod _en { get { return new Encod(); } }
        /// <summary>
        /// 加载
        /// </summary>
        protected override void OnLoad()
        {
            try
            {
                Tresource.value = File.ReadAllText(path,_en);
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
