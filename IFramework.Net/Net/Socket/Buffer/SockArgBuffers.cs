/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace IFramework.Net
{
     class SockArgBuffers
    {
        private int _totalSize = 0;
        private int _curOffset = 0;
        private int _bufferSize = 2048;
        private LockParam _lockParam = new LockParam();
        private byte[] _buff = null;
        private Queue<int> _freeOffsets = null;

        public int bufferSize { get { return _bufferSize; } }

        public SockArgBuffers(int maxCounts, int bufferSize)
        {
            if (bufferSize < 4) bufferSize = 4;
            this._bufferSize = bufferSize;
            this._curOffset = 0;
            _totalSize = maxCounts * bufferSize;
            _buff = new byte[_totalSize];
            _freeOffsets = new Queue<int>(maxCounts);
        }
        public void FreeBuffer()
        {
            using (LockWait wait = new LockWait(ref _lockParam))
            {
                _curOffset = 0;
                _freeOffsets.Clear();
            }
        }
        public void Clear()
        {
            using (LockWait wait = new LockWait(ref _lockParam))
            {
                _freeOffsets.Clear();
            }
        }

        public bool SetBuffer(SocketAsyncEventArgs agrs)
        {
            using (LockWait wait = new LockWait(ref _lockParam))
            {
                if (_freeOffsets.Count > 0)
                {
                    agrs.SetBuffer(this._buff, this._freeOffsets.Dequeue(), _bufferSize);
                }
                else
                {
                    if ((_totalSize - _bufferSize) < _curOffset) return false;
                    agrs.SetBuffer(this._buff, this._curOffset, this._bufferSize);
                    this._curOffset += this._bufferSize;
                }
                return true;
            }
        }
        public bool WriteBuffer(SocketAsyncEventArgs agrs, byte[] buffer, int offset, int len)
        {
            using (LockWait wait = new LockWait(ref _lockParam))
            {
                if (agrs.Offset + len > this._buff.Length) return false;
                if (len > _bufferSize) return false;
                Buffer.BlockCopy(buffer, offset, this._buff, agrs.Offset, len);
                agrs.SetBuffer(this._buff, agrs.Offset, len);
                return true;
            }
        }
        public void FreeBuffer(SocketAsyncEventArgs arg)
        {
            using (LockWait wait = new LockWait(ref _lockParam))
            {
                this._freeOffsets.Enqueue(arg.Offset);
                arg.SetBuffer(null, 0, 0);
            }
        }
        public ArraySegment<byte>[] BuffToSegs(byte[] buffer, int offset, int len)
        {
            if (len <= _bufferSize)
                return new ArraySegment<byte>[] { new ArraySegment<byte>(buffer, offset, len) };
            int bSize = _bufferSize;
            int bCnt = len / _bufferSize;
            int bOffset = 0;
            bool isRem = false;
            if (len % _bufferSize != 0)
            {
                isRem = true;
                bCnt += 1;
            }
            ArraySegment<byte>[] segItems = new ArraySegment<byte>[bCnt];
            for (int i = 0; i < bCnt; ++i)
            {
                bOffset = i * _bufferSize;
                if (i == (bCnt - 1) && isRem)
                {
                    bSize = len - bOffset;
                }
                segItems[i] = new ArraySegment<byte>(buffer, offset + bOffset, bSize);
            }
            return segItems;
        }
    }

}
