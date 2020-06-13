/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IFramework.Net
{
     class SocketSend : UdpSocket, IDisposable
    {
        private SocketEventArgPool _sendArgs;
        private SockArgBuffers _sendBuff;
        private bool _isDisposed;
        public event EventHandler<SocketAsyncEventArgs> sendEventHandler;

        public SocketSend(int maxCount, int bufferSize = 4096) : base(bufferSize)
        {
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _sock.ReceiveTimeout = _receiveTimeout;
            _sock.SendTimeout = _sendTimeout;
            _sendArgs = new SocketEventArgPool(maxCount);
            _sendBuff = new SockArgBuffers(maxCount, bufferSize);
            for (int i = 0; i < maxCount; ++i)
            {
                SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
                socketArgs.UserToken = _sock;
                socketArgs.Completed += SendCompleted;
                _sendBuff.SetBuffer(socketArgs);
                _sendArgs.Set(socketArgs);
            }
        }
        public SocketSend(Socket sock, int maxCount, int bufferSize = 4096) : base(bufferSize)
        {
            base._sock = sock;
            _sendArgs = new SocketEventArgPool(maxCount);
            _sendBuff = new SockArgBuffers(maxCount, bufferSize);
            for (int i = 0; i < maxCount; ++i)
            {
                SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
                socketArgs.UserToken = sock;
                socketArgs.Completed += SendCompleted;
                _sendBuff.SetBuffer(socketArgs);
                _sendArgs.Set(socketArgs);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool dispose)
        {
            if (_isDisposed) return;
            if (dispose)
            {
                _sendArgs.Clear();
                _sock.Dispose();
                _sendBuff.Clear();
                _isDisposed = true;
            }
        }

        public bool Send(BufferSegment segBuff, IPEndPoint remoteEP, bool waiting)
        {
            try
            {
                bool isWillEvent = true;
                ArraySegment<byte>[] segItems = _sendBuff.BuffToSegs(segBuff.buffer, segBuff.offset, segBuff.count);
                foreach (var seg in segItems)
                {
                    var SendArg = _sendArgs.GetFreeArg((retry) => { return true; }, waiting);
                    if (SendArg == null)
                        throw new Exception("发送缓冲池已用完,等待回收超时...");
                    SendArg.RemoteEndPoint = remoteEP;
                    //Socket s = SocketVersion(remoteEP);
                    //SendArg.UserToken = s;
                    if (!_sendBuff.WriteBuffer(SendArg, seg.Array, seg.Offset, seg.Count))
                    {
                        _sendArgs.Set(SendArg);
                        throw new Exception(string.Format("发送缓冲区溢出...buffer block max size:{0}", _sendBuff.bufferSize));
                    }
                    if (SendArg.RemoteEndPoint != null)
                    {
                        isWillEvent &= _sock.SendToAsync(SendArg);
                        if (!isWillEvent)
                        {
                            SendCallBack(SendArg);
                        }
                    }
                    Thread.Sleep(5);
                }
                return isWillEvent;
            }
            catch (Exception )
            {
                throw ;
            }
        }
        private void SendCallBack(SocketAsyncEventArgs e)
        {
            _sendArgs.Set(e);
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if (sendEventHandler != null)
                {
                    sendEventHandler(e.UserToken as Socket, e);
                }
            }
        }
        void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.SendTo:
                case SocketAsyncOperation.SendPackets:
                case SocketAsyncOperation.Send:
                    SendCallBack(e);
                    break;
            }
        }

        public int SendSync(BufferSegment segBuff, IPEndPoint remoteEP)
        {
            return _sock.SendTo(segBuff.buffer, segBuff.offset, segBuff.count, SocketFlags.None, remoteEP);
        }

        //private Socket SocketVersion(IPEndPoint ips)
        //{
        //    if (ips.AddressFamily == sock.AddressFamily)
        //    {
        //        return sock;
        //    }
        //    else
        //    {
        //        sock = new Socket(ips.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        //    }
        //    return sock;
        //}
    }

}
