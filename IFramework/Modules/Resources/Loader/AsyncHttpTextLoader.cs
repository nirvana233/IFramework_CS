using System;
using System.IO;
using System.Net;
using System.Text;

namespace IFramework.Modules.Resources
{
    /// <summary>
    /// 异步Http文本加载
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class AsyncHttpTextLoader : ResourceLoader<string>
    {
        private int _blockSize = 2048;
        private byte[] _buffer;
        private Stream _stream;
        private StringBuilder _sb;
        private Encoding _en;
        /// <summary>
        /// 进度
        /// </summary>
        protected override float _progress
        {
            get
            {
                if (_stream != null)
                    return _stream.Position / _stream.Length;
                if (_isdone)
                    return 1;
                return 0;
            }
        }
        /// <summary>
        /// 加载
        /// </summary>
        protected override void OnLoad()
        {
            try
            {
                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(path);
                HttpWebResponse _response = (HttpWebResponse)_request.BeginGetResponse(EndGetResponse, _request);
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

        private void EndGetResponse(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest _request = ar.AsyncState as HttpWebRequest;
                HttpWebResponse _response = _request.EndGetResponse(ar) as HttpWebResponse;
                _stream = _response.GetResponseStream();
                _buffer = new byte[_blockSize];
                _sb = new StringBuilder();
                _en = Encoding.GetEncoding(_response.ContentEncoding);
                _stream.BeginRead(_buffer, 0, _blockSize, EndRead, _stream);
            }
            catch (Exception e)
            {
                ThrowErr(e.Message);
            }

        }

        private void EndRead(IAsyncResult ar)
        {
            try
            {
                int bytesRead = _stream.EndRead(ar);
                if (bytesRead > 0)
                {
                    var datastr = _en.GetString(_buffer, 0, _buffer.Length);
                    _sb.Append(datastr);
                    _stream.BeginRead(_buffer, 0, _buffer.Length, EndRead, null);
                }
                else
                {
                    Tresource.value = _sb.ToString();
                    _stream.Dispose();
                    _isdone = true;
                }
            }
            catch (Exception e)
            {
                ThrowErr(e.Message);
            }
        }
    }
}
