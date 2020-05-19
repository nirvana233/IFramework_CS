using System;
using System.IO;
using System.Net;

namespace IFramework.Modules.Resources
{
    /// <summary>
    /// http流加载器
    /// </summary>
    public class HttpTextLoader : ResourceLoader<string>
    {
        /// <summary>
        /// 加载
        /// </summary>
        protected override void OnLoad()
        {
            try
            {
                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(path);
                HttpWebResponse _response = (HttpWebResponse)_request.GetResponse();
                StreamReader _reader = new StreamReader(_response.GetResponseStream(), System.Text.Encoding.GetEncoding(_response.ContentEncoding));
                Tresource.value = _reader.ReadToEnd();
                //_reader.Close();
                _response.Close();
                _reader.Dispose();
            }
            catch (Exception e)
            {
                ThrowErr(e.Message);
            }
            finally
            {
                _progress = 1;
                _isdone = true;
            }
        }
    }
}
