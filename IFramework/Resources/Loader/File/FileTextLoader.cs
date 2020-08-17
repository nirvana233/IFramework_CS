using System;
using System.IO;
using System.Text;

namespace IFramework.Resource
{
    /// <summary>
    /// 文本加载器
    /// </summary>
    /// <typeparam name="Encod"></typeparam>
    public class FileTextLoader<Encod> : ResourceLoader<string> where Encod: Encoding,new()
    {
        private Encod _en { get { return new Encod(); } }
        /// <summary>
        /// 加载
        /// </summary>
        protected override void OnLoad()
        {
            try
            {
                Tresource.Tvalue = File.ReadAllText(path,_en);
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
