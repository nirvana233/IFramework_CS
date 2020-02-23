/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace IFramework.Net
{
    enum ClientChannelType
    {
        Async = 0,
        AsyncWait = 1,
        Sync = 2
    }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class TcpClientToken : TcpSocket, IDisposable
    {
        private ClientChannelType channelType;
        private bool _isDisposed;
        private int _bufferNumber = 8;
        private LockParam _lockParam = new LockParam();
        private AutoResetEvent _autoReset = new AutoResetEvent(false);
        private SocketEventArgPool _sendArgs;
        private SockArgBuffers _sendBuffMgr;
        private Encoding _encoding = Encoding.UTF8;



        public int bufferNumber { get { return _bufferNumber; } }
        public OnReceivedString onRecieveString { get; set; }
        public OnConnect onConnect { get; set; }
        public OnDisConnect onDisConnect { get; set; }
        public OnSendCallBack onSendCallBack { get; set; }
        public OnReceieve onReceive { get; set; }

        public TcpClientToken(int bufferSize, int bufferNumber) : base(bufferSize)
        {
            this._recBuffer = new byte[bufferSize];
            this._bufferNumber = bufferNumber;
            _sendArgs = new SocketEventArgPool(bufferNumber);
            _sendBuffMgr = new SockArgBuffers(bufferNumber, bufferSize);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;
            if (isDisposing)
            {
                _sendArgs.Clear();
                if (_sendBuffMgr != null)
                {
                    _sendBuffMgr.Clear();
                    _sendBuffMgr.FreeBuffer();
                }

                _isDisposed = true;
                SafeClose();
            }
        }


        public bool ConnectTo(int port, string ip)
        {
            try
            {
                if (connected == false || sock == null || sock.Connected == false)
                {
                    Close();
                }
                connected = false;
                channelType = ClientChannelType.AsyncWait;
                using (LockWait lwait = new LockWait(ref _lockParam))
                {
                    CreateTcpSocket(port, ip);
                    //连接事件绑定
                    var sArgs = new SocketAsyncEventArgs
                    {
                        RemoteEndPoint = endPoint,
                        UserToken = new SocketToken() { sock = sock }
                    };
                    sArgs.AcceptSocket = sock;
                    sArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IOCompleted);
                    if (!sock.ConnectAsync(sArgs))
                    {
                        ConnectCallback(sArgs);
                    }
                }
                _autoReset.WaitOne(connTimeout);
                connected = sock.Connected;
                return connected;
            }
            catch (Exception ex)
            {
                Close();
                throw ex;
            }
        }
        public bool ConnectSync(int port, string ip)
        {
            if (connected == false || sock == null || sock.Connected == false)
            {
                Close();
            }
            connected = false;
            channelType = ClientChannelType.Sync;
            int retry = 3;
            using (LockWait wait = new LockWait(ref _lockParam))
            {
                CreateTcpSocket(port, ip);
            }
            while (retry > 0)
            {
                try
                {
                    --retry;
                    sock.Connect(endPoint);
                    connected = true;
                    return true;
                }
                catch (Exception ex)
                {
                    Close();
                    if (retry <= 0) throw ex;
                    Thread.Sleep(1000);
                }
            }
            return false;
        }
        public void ConnectAsync(int port, string ip)
        {
            try
            {
                if (connected == false || sock == null || sock.Connected == false)
                {
                    Close();
                }
                connected = false;
                channelType = ClientChannelType.Async;
                using (LockWait wait = new LockWait(ref _lockParam))
                {
                    CreateTcpSocket(port, ip);
                    SocketAsyncEventArgs conArg = new SocketAsyncEventArgs
                    {
                        RemoteEndPoint = endPoint,
                        UserToken = new SocketToken(-1) { sock = sock }
                    };
                    conArg.AcceptSocket = sock;
                    conArg.Completed += new EventHandler<SocketAsyncEventArgs>(IOCompleted);
                    if (!sock.ConnectAsync(conArg))
                    {
                        ConnectCallback(conArg);
                    }
                }
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }

        public int SendSync(BufferSegment sendbuff, BufferSegment recBuff)
        {
            if (channelType != ClientChannelType.Sync)
            {
                throw new Exception("需要使用同步连接...ConnectSync");
            }
            int sent = sock.Send(sendbuff.buffer, sendbuff.offset, sendbuff.count, SocketFlags.None);
            if (recBuff.buffer == null || recBuff.count == 0) return sent;
            /*int cnt =*/ sock.Receive(recBuff.buffer, recBuff.offset, recBuff.count, 0);
            return sent;
        }
        public void SendFile(string filename)
        {
            sock.SendFile(filename);
        }
        public bool SendAsync(BufferSegment segBuff, bool wait = true)
        {
            try
            {
                if (connected == false || sock == null || sock.Connected == false)
                {
                    Close();
                    return false;
                }
                ArraySegment<byte>[] segs = _sendBuffMgr.BuffToSegs(segBuff.buffer, segBuff.offset, segBuff.count);
                bool isWillEvent = true;
                foreach (var seg in segs)
                {
                    SocketAsyncEventArgs senArg = GetFreeSendArg(wait);
                    if (senArg == null) return false;
                    if (!_sendBuffMgr.WriteBuffer(senArg, seg.Array, seg.Offset, seg.Count))
                    {
                        _sendArgs.Set(senArg);
                        throw new Exception(string.Format("发送缓冲区溢出...buffer block max size:{0}", _sendBuffMgr.bufferSize));
                    }
                    if (senArg.UserToken == null)
                        ((SocketToken)senArg.UserToken).sock = sock;
                    if (connected == false || sock == null || sock.Connected == false)
                    {
                        Close();
                        return false;
                    }
                    isWillEvent &= sock.SendAsync(senArg);
                    if (!isWillEvent)//can't trigger the io complated event to do
                    {
                        SendCallback(senArg);
                    }
                    if (_sendArgs.count < (_sendArgs.capcity >> 2))
                        Thread.Sleep(2);
                }
                return isWillEvent;
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }
        public int SendSync(BufferSegment segBuff)
        {
            return sock.Send(segBuff.buffer, segBuff.offset, segBuff.count, SocketFlags.None);
        }
        public void ReceiveSync(byte[] recbuff, int reclen, Action<byte[],int> receivedAction)
        {
            if (channelType != ClientChannelType.Sync)
            {
                throw new Exception("需要使用同步连接...ConnectSync");
            }
            int cnt = 0;
            do
            {
                if (sock.Connected == false) break;
                cnt = sock.Receive(recbuff, reclen, 0);
                if (cnt <= 0) break;
                receivedAction(recbuff,reclen);

            } while (true);
        }
        public void DisConnect()
        {
            connected = false;
            Close();
        }




        private void Close()
        {
            using (LockWait wait = new LockWait(ref _lockParam))
            {
                _sendArgs.Clear();
                if (_sendBuffMgr != null)
                {
                    _sendBuffMgr.Clear();
                    _sendBuffMgr.FreeBuffer();
                }
                SafeClose();
                connected = false;
            }
        }
        private void ConnectCallback(SocketAsyncEventArgs e)
        {
            try
            {
                connected = (e.SocketError == SocketError.Success);
                if (connected)
                {
                    using (LockWait wait = new LockWait(ref _lockParam))
                    {
                        InitializePool(_bufferNumber);
                    }
                    e.SetBuffer(_recBuffer, 0, bufferSize);
                    if (onConnect != null)
                    {
                        SocketToken token = e.UserToken as SocketToken;
                        token.endPoint = (IPEndPoint)e.RemoteEndPoint;
                        onConnect(token, connected);
                    }
                    if (!e.AcceptSocket.ReceiveAsync(e))
                    {
                        ReceiveCallback(e);
                    }
                }
                else
                {
                    DisconnectAsync(e);
                }
                if (channelType == ClientChannelType.AsyncWait)
                    _autoReset.Set();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private SocketAsyncEventArgs GetFreeSendArg(bool wait)
        {
            SocketAsyncEventArgs senArg = _sendArgs.GetFreeArg((retry) =>
            {
                return !(connected == false || sock == null || sock.Connected == false);
            }, wait);
            if (connected == false) return null;
            if (senArg == null)
                throw new Exception("发送缓冲池已用完,等待回收超时...");
            return senArg;
        }
        private void SendCallback(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    if (onSendCallBack != null)
                    {
                        SocketToken token = e.UserToken as SocketToken;
                        token.endPoint = (IPEndPoint)e.RemoteEndPoint;
                        onSendCallBack( token,new BufferSegment( e.Buffer, e.Offset, e.BytesTransferred));
                    }
                }
                else
                {
                    DisconnectAsync(e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _sendArgs.Set(e);
            }
        }
        private void DisconnectAsync(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.AcceptSocket == null) return;

                bool willRaiseEvent = false;
                if (e.AcceptSocket != null && e.AcceptSocket.Connected)
                    willRaiseEvent = e.AcceptSocket.DisconnectAsync(e);

                if (!willRaiseEvent)
                {
                    DisconnectCallback(e);
                }
                else
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void DisconnectCallback(SocketAsyncEventArgs e)
        {
            try
            {
                connected = (e.SocketError == SocketError.Success);
                if (connected)
                {
                    Close();
                }
                if (onDisConnect != null)
                {
                    SocketToken token = e.UserToken as SocketToken;
                    token.endPoint = (IPEndPoint)e.RemoteEndPoint;
                    onDisConnect(token);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void ReceiveCallback(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success || e.AcceptSocket.Connected == false)
            {
                DisconnectAsync(e);
                return;
            }
            SocketToken token = e.UserToken as SocketToken;
            token.endPoint = (IPEndPoint)e.RemoteEndPoint;
            if (onReceive != null) onReceive(token, new BufferSegment(e.Buffer, e.Offset, e.BytesTransferred));
            if (onRecieveString != null) onRecieveString(token, _encoding.GetString(e.Buffer, e.Offset, e.BytesTransferred));

            if (!sock.Connected) return;
            if (!e.AcceptSocket.ReceiveAsync(e))
            {
                ReceiveCallback(e);
            }
        }
        private void InitializePool(int bufferNumber)
        {
            _sendArgs.Clear();
            _sendBuffMgr.Clear();
            _sendBuffMgr.FreeBuffer();
            _sendBuffMgr = new SockArgBuffers(bufferNumber, bufferSize);
            _sendArgs = new SocketEventArgPool(bufferNumber);
            for (int i = 0; i < bufferNumber; ++i)
            {
                SocketAsyncEventArgs tArgs = new SocketAsyncEventArgs()
                {
                    DisconnectReuseSocket = true
                };
                tArgs.Completed += IOCompleted;
                tArgs.UserToken = new SocketToken(i)
                {
                    sock = sock,
                    tokenId = i
                };
                _sendBuffMgr.SetBuffer(tArgs);
                _sendArgs.Set(tArgs);
            }
        }
        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ConnectCallback(e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    DisconnectCallback(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ReceiveCallback(e);
                    break;
                case SocketAsyncOperation.Send:
                    SendCallback(e);
                    break;
            }
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
