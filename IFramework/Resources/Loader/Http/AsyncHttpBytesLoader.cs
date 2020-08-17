using System;
using System.IO;
using System.Net;

namespace IFramework.Resource
{
    /// <summary>
    /// 异步Http加载器
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class AsyncHttpBytesLoader : ResourceLoader<byte[]>
    {
        private int _blockSize = 2048;
        private byte[] _buffer;
        private Stream _stream;
        /// <summary>
        /// 进度
        /// </summary>
        public override float progress
        {
            get
            {
                if (_stream != null)
                    return _stream.Position / _stream.Length;
                if (isdone)
                    return 1;
                return 0;
            }
            protected set
            {

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
                progress = 1;
                isdone = true;
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
                Tresource.Tvalue = new byte[_stream.Length];

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
                    Array.Copy(_buffer, 0, Tresource.Tvalue, _stream.Position, _blockSize);
                    _stream.BeginRead(_buffer, 0, _buffer.Length, EndRead, null);
                }
                else
                {
                    _stream.Dispose();
                    isdone = true;
                }
            }
            catch (Exception e)
            {
                ThrowErr(e.Message);
            }
        }
    }
}
