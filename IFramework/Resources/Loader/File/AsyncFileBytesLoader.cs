using System;
using System.IO;

namespace IFramework.Resource
{
    /// <summary>
    /// 异步文件流加载器
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
     class AsyncFileBytesLoader : AsyncResourceLoader<byte[]>
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
            protected set
            {

            }
        }
        private byte[] _buffer;
        private FileStream _fs;
        private int _blockSize = 2048;
        /// <summary>
        /// 加载
        /// </summary>
        protected override void OnLoad()
        {
            try
            {
                _fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, _blockSize, true);
                Tresource.value = new byte[_fs.Length];
                _buffer = new byte[_blockSize];
                _fs.BeginRead(_buffer, 0, _buffer.Length, EndRead, _fs);
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
                int bytesRead = _fs.EndRead(ar);
                if (bytesRead > 0)
                {
                    Array.Copy(_buffer, 0, Tresource.value, _fs.Position, _blockSize);
                    _fs.BeginRead(_buffer, 0, _buffer.Length, EndRead, null);
                }
                else
                {
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
