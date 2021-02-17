using System;
using System.IO;
using System.Text;

namespace IFramework.Resource
{
    /// <summary>
    /// 异步文本加载器
    /// </summary>
     class AsyncFileTextLoader : AsyncResourceLoader<string>
    {
        /// <summary>
        /// 进度
        /// </summary>
        public override float progress
        {
            get
            {
                if (_fs != null)
                    return _fs.Position / _fs.Length;
                if (isdone)
                    return 1;
                return 0;
            }
            protected set { }
        }

        /// <summary>
        /// 编码
        /// </summary>
        public Encoding encoding;

        private StringBuilder _sb;
        private byte[] _buffer;
        private FileStream _fs;
        private int _blockSize=2048;
        /// <summary>
        /// 加载
        /// </summary>
        protected override void OnLoad()
        {
            try
            {
                _sb = new StringBuilder();
                _buffer = new byte[_blockSize];
                _fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, _blockSize, true);
                _fs.BeginRead(_buffer, 0, _buffer.Length, EndRead, _fs);
            }
            catch (Exception e)
            {
                ThrowErr(e.Message);
            }

        }
        private void EndRead(IAsyncResult asyncResult)
        {
            try
            {
                int bytesRead = _fs.EndRead(asyncResult);
                if (bytesRead > 0)
                {
                    var datastr = encoding.GetString(_buffer, 0, _buffer.Length);
                    _sb.Append(datastr);
                    _fs.BeginRead(_buffer, 0, _buffer.Length, EndRead, null);
                }
                else
                {
                    Tresource.Tvalue = _sb.ToString();
                    _fs.Dispose();
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
