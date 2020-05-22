using System;
using System.IO;
using System.Net;

namespace IFramework.Modules.Resources
{
    /// <summary>
    /// http流加载器
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class HttpBytesLoader : ResourceLoader<byte[]>
    {
        private int _blockSize=2048;
        /// <summary>
        /// 加载
        /// </summary>
        protected override void OnLoad()
        {
            try
            {
                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(path);
                HttpWebResponse _response = (HttpWebResponse)_request.GetResponse();
                Stream _stream = _response.GetResponseStream();
                Tresource.value = new byte[_stream.Length];
                while (_stream.Read(Tresource.value, (int)_stream.Position, _blockSize) > 0)
                {
                    if (!_stream.CanRead) break;
                    _progress = _stream.Position / _stream.Length;
                }
                //_stream.Close();
                _response.Close();
                _stream.Dispose();
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
