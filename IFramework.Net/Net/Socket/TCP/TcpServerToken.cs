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
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class TcpServerToken : TcpSocket, IDisposable
    {
        private bool _isRunning;
        private int _curConCount;
        private int _maxConCount;
        private bool _isDisposed = false;
        private int _offsetNumber = 2;

        private LockParam _lockParam;
        private Semaphore _countSema;
        private SocketEventArgPool _sendArgs;
        private SocketEventArgPool _receiveArgs;
        private SockArgBuffers _recBuffMgr;
        private SockArgBuffers _sendBuffMgr;
        private Encoding _encoding = Encoding.UTF8;

        public int curConCount { get { return _curConCount; } }
        public int maxConCount { get { return _maxConCount; } }
        public bool isRunning { get { return _isRunning; } }

        public OnReceivedString onRecieveString { get; set; }
        public OnAccept onAcccept { get; set; }
        public OnReceieve onReceive { get; set; }
        public OnDisConnect onDisConnect { get; set; }
        public OnSendCallBack onSendCallBack { get; set; }

        public TcpServerToken(int maxConCount, int bufferSize) : base(bufferSize)
        {
            this._maxConCount = maxConCount;
            this.bufferSize = bufferSize;
            _recBuffMgr = new SockArgBuffers(maxConCount + _offsetNumber, bufferSize);
            _sendBuffMgr = new SockArgBuffers(maxConCount + _offsetNumber, bufferSize);
            _sendArgs = new SocketEventArgPool(maxConCount + _offsetNumber);
            _receiveArgs = new SocketEventArgPool(maxConCount + _offsetNumber);
            _lockParam = new LockParam();
            _countSema = new Semaphore(maxConCount + _offsetNumber, maxConCount + _offsetNumber);
        }


        public bool Start(int port, string ip = "0.0.0.0")
        {
            Stop();
            InitArgs();
            int errCount = 0;
            reStart:
            try
            {
                using (LockWait wait = new LockWait(ref _lockParam))
                {
                    CreateTcpSocket(port, ip);
                    sock.Bind(endPoint);
                    sock.Listen(_maxConCount);
                    _isRunning = true;
                }
                StartAcceptAsync(null);
                return true;
            }
            catch (Exception)
            {
                SafeClose();
                errCount++;
                if (errCount >= 3)
                {
                    throw;
                }
                else
                {
                    Thread.Sleep(1000);
                    goto reStart;
                }
            }
        }
        public void Stop()
        {
            try
            {
                using (LockWait wait = new LockWait(ref _lockParam))
                {
                    if (_curConCount > 0)
                    {
                        if (_countSema != null)
                            _countSema.Release(_curConCount);
                        _curConCount = 0;
                    }
                    SafeClose();
                    _isRunning = false;
                }
            }
            catch (Exception) { }
        }
        public int SendSync(SocketToken token, BufferSegment seg)
        {
            return token.sock.Send(seg.buffer, seg.offset, seg.count, SocketFlags.None);
        }
        public bool SendAsync(SocketToken token, BufferSegment seg, bool wait = false)
        {
            try
            {
                if (!token.sock.Connected) return false;
                bool isWillEvent = true;
                ArraySegment<byte>[] segs = _sendBuffMgr.BuffToSegs(seg.buffer, seg.offset, seg.count);
                for (int i = 0; i < segs.Length; i++)
                {
                    SocketAsyncEventArgs senArg = GetFreeSendArg(wait, token.sock);
                    if (senArg == null) return false;
                    senArg.UserToken = token;
                    if (!_sendBuffMgr.WriteBuffer(senArg, segs[i].Array, segs[i].Offset, segs[i].Count))
                    {
                        _sendArgs.Set(senArg);
                        throw new Exception(string.Format("发送缓冲区溢出...buffer block max size:{0}", _sendBuffMgr.bufferSize));
                    }
                    if (!token.sock.Connected) return false;
                    isWillEvent &= token.sock.SendAsync(senArg);
                    if (!isWillEvent)
                    {
                        SendCallBack(senArg);
                    }
                    if (_sendArgs.count < (_sendArgs.capcity >> 2))
                        Thread.Sleep(5);
                }
                return isWillEvent;
            }
            catch (Exception )
            {
                Close(token);
                throw ;
            }
        }
        public void Close(SocketToken token)
        {
            DisConnectAsync(token);
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
                SafeClose();
                _sendBuffMgr.Clear();
                _recBuffMgr.Clear();
                _isDisposed = true;
                _countSema.Close();
                //countSema.Dispose();                       //.net4.X,ok
                _sendArgs.Dispose();
                _receiveArgs.Dispose();
            }
        }
        private void InitArgs()
        {
            _receiveArgs.Clear();
            _sendArgs.Clear();
            _sendBuffMgr.FreeBuffer();
            _recBuffMgr.FreeBuffer();
            for (int i = 0; i < _maxConCount + _offsetNumber; i++)
            {
                SocketAsyncEventArgs senArg = new SocketAsyncEventArgs()
                {
                    DisconnectReuseSocket = true,
                    SocketError = SocketError.NotInitialized
                };
                senArg.Completed += IOCompleted;
                senArg.UserToken = new SocketToken(i);
                _sendArgs.Set(senArg);
                _sendBuffMgr.SetBuffer(senArg);

                SocketAsyncEventArgs recArg = new SocketAsyncEventArgs()
                {
                    DisconnectReuseSocket = true,
                    SocketError = SocketError.SocketError
                };
                recArg.Completed += IOCompleted;
                SocketToken token = new SocketToken(i)
                {
                    arg = recArg
                };
                recArg.UserToken = token;
                _recBuffMgr.SetBuffer(recArg);
                _receiveArgs.Set(recArg);
            }
        }
        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    AcceptCallBack(e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    DisconnectCallBack(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ReceiveCallBack(e);
                    break;
                case SocketAsyncOperation.Send:
                    SendCallBack(e);
                    break;
            }
        }
        private void StartAcceptAsync(SocketAsyncEventArgs e)
        {
            if (!_isRunning || sock == null)
            {
                _isRunning = false;
                return;
            }
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.DisconnectReuseSocket = true;
                e.UserToken = new SocketToken(-255);
                e.Completed += IOCompleted;
            }
            else
            {
                e.AcceptSocket = null;
            }
            _countSema.WaitOne();
            if (!sock.AcceptAsync(e))
            {
                AcceptCallBack(e);
            }
        }
        private void AcceptCallBack(SocketAsyncEventArgs e)
        {
            try
            {
                if (!_isRunning /*|| curConCount >= maxConCount*/ || e.SocketError != SocketError.Success)
                {
                    //DisconnectCallBack(e);
                    //if (e.SocketError != SocketError.ConnectionReset)
                    //{
                    //    return;
                    //}
                    DisposeSocketArgs(e);
                    return;
                }
                SocketAsyncEventArgs recArg = _receiveArgs.GetFreeArg((retry) => { return true; }, false);
                if (recArg == null)
                {
                    DisposeSocketArgs(e);
                    throw new Exception(string.Format("已经达到最大连接数max:{0};used:{1}", _maxConCount, _curConCount));
                }
                Interlocked.Increment(ref _curConCount);
                SocketToken token = ((SocketToken)recArg.UserToken);
                token.sock = e.AcceptSocket;
                token.endPoint = (IPEndPoint)e.AcceptSocket.RemoteEndPoint;
                token.arg = recArg;
                token.connTime = DateTime.Now;
                recArg.UserToken = token;
                if (e.AcceptSocket.Connected)
                {
                    if (_curConCount > _maxConCount)
                    {
                        token.sock.Shutdown(SocketShutdown.Send);
                        token.sock.Close();
                        _receiveArgs.Set(recArg);
                        Interlocked.Decrement(ref _curConCount);
                        _countSema.Release();
                    }
                    else
                    {
                        if (onAcccept != null) onAcccept(token);
                        if (!token.sock.ReceiveAsync(recArg))
                        {
                            ReceiveCallBack(recArg);
                        }
                    }
                }
                else
                {
                    DisconnectCallBack(recArg);
                }
                if (!_isRunning) return;
            }
            catch (Exception exc)
            {
                Log.L(exc);
                throw;
            }
            finally
            {
                //if (e.SocketError != SocketError.ConnectionReset)
                //{
                StartAcceptAsync(e);
                //}
            }
        }
        private void DisposeSocketArgs(SocketAsyncEventArgs e)
        {
            SocketToken token = e.UserToken as SocketToken;
            if (token != null) token.Close();
            e.Dispose();
        }
        private void ReceiveCallBack(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success)
                {
                    DisconnectCallBack(e);
                    return;
                }
                SocketToken token = e.UserToken as SocketToken;
                if (onReceive != null) onReceive(token,new BufferSegment( e.Buffer, e.Offset, e.BytesTransferred));
                if (onRecieveString != null) onRecieveString(token, _encoding.GetString(e.Buffer, e.Offset, e.BytesTransferred));
                if (token.sock.Connected)
                {
                    if (!token.sock.ReceiveAsync(e))
                    {
                        ReceiveCallBack(e);
                    }
                }
                else
                {
                    DisconnectCallBack(e);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void DisConnectAsync(SocketToken token)
        {
            try
            {
                if (token == null || token.sock == null || token.arg == null) return;
                if (token.sock.Connected)
                    token.sock.Shutdown(SocketShutdown.Send);
                SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
                arg.DisconnectReuseSocket = true;
                arg.SocketError = SocketError.SocketError;
                arg.UserToken = null;
                arg.Completed += IOCompleted;
                if (token.sock.DisconnectAsync(arg) == false)
                {
                    DisconnectCallBack(token.arg);
                }
            }
            catch (ObjectDisposedException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void DisconnectCallBack(SocketAsyncEventArgs e)
        {
            try
            {
                SocketToken token = e.UserToken as SocketToken;
                if (token != null)
                {
                    if (token.tokenId == -255) return;
                    _countSema.Release();
                    Interlocked.Decrement(ref _curConCount);
                    if (onDisConnect != null)
                    {
                        onDisConnect(token);
                    }
                    token.Close();
                    _receiveArgs.Set(e);
                }
                else
                {
                    throw new Exception("UserToken is Null,May From Accept");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private SocketAsyncEventArgs GetFreeSendArg(bool wait, Socket sock)
        {
            SocketAsyncEventArgs tArgs = _sendArgs.GetFreeArg((retry) =>
            {
                if (sock.Connected == false) return false;
                return true;
            }, wait);
            if (sock.Connected == false)
                return null;
            if (tArgs == null)
                throw new Exception("发送缓冲池已用完,等待回收超时...");
            return tArgs;
        }
        private void SendCallBack(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    SocketToken token = e.UserToken as SocketToken;
                    if (onSendCallBack != null)
                    {
                        onSendCallBack(token,new BufferSegment( e.Buffer, e.Offset, e.BytesTransferred));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _sendArgs.Set(e);
            }
        }
    }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
