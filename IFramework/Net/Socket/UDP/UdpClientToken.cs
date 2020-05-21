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
using System.Text;

namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class UdpClientToken : UdpSocket, IDisposable
    {
        private bool _isDisposed = false;
        private int _recBufferSize = 4096;
        private int _maxConn = 8;
        private Encoding _encoding = Encoding.UTF8;
        private LockParam _lockParam = new LockParam();
        private ManualResetEvent _mReset = new ManualResetEvent(false);
        private SocketEventArgPool _sendArgs = null;
        private SockArgBuffers _sendBuff = null;

        public int sendBufferPoolNumber { get { return _sendArgs.count; } }
        public OnReceieve onReceive { get; set; }
        public OnSendCallBack onSendCallback { get; set; }
        public OnReceivedString onRecieveString { get; set; }

        public UdpClientToken(int bufferSize, int maxConn=64) : base(bufferSize)
        {
            this._recBufferSize = bufferSize;
            this._maxConn = maxConn;

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
                if (_sendBuff != null)
                {
                    _sendBuff.Clear();
                    _sendBuff.FreeBuffer();
                }
                SafeClose();
                _isDisposed = true;
            }
        }

        public void Disconnect()
        {
            Close();
            _isConnected = false;
        }
        public bool Connect(int port, string ip )
        {
            Close();
            CreateUdpSocket(port, IPAddress.Parse(ip));
            _sendArgs = new SocketEventArgPool(_maxConn);
            _sendBuff = new SockArgBuffers(_maxConn, _recBufferSize);
            for (int i = 0; i < _maxConn; ++i)
            {
                SocketAsyncEventArgs sendArg = new SocketAsyncEventArgs();
                sendArg.Completed += IOCompleted;

                sendArg.UserToken = _sock;
                _sendBuff.SetBuffer(sendArg);
                _sendArgs.Set(sendArg);
            }
            return true;
            
        }
       
        public void StartReceive()
        {
            using (LockWait wait = new LockWait(ref _lockParam))
            {
                SocketAsyncEventArgs recArg = new SocketAsyncEventArgs();
                recArg.Completed += IOCompleted;
                recArg.UserToken = _sock;
                recArg.RemoteEndPoint = _endPoint;
                recArg.SetBuffer(_recBuffer, 0, _recBufferSize);
                if (!_sock.ReceiveFromAsync(recArg))
                {
                    ReceiveCallBack(recArg);
                }
            }
        }
        public void ReceiveSync(BufferSegment segRecieve, Action<BufferSegment> receiveAction)
        {
            int cnt = 0;
            do
            {
                cnt = _sock.ReceiveFrom(segRecieve.buffer,
                    segRecieve.count,
                    SocketFlags.None,
                    ref _endPoint);

                if (cnt <= 0) break;

                receiveAction(segRecieve);
            } while (true);
        }
        public bool Send(BufferSegment segBuff, bool waiting = true)
        {
            try
            {
                bool isWillEvent = true;
                ArraySegment<byte>[] segItems = _sendBuff.BuffToSegs(segBuff.buffer, segBuff.offset, segBuff.count);
                foreach (var seg in segItems)
                {
                    SocketAsyncEventArgs sendArg = _sendArgs.GetFreeArg((retry) => { return true; }, waiting);
                    if (sendArg == null)
                        throw new Exception("发送缓冲池已用完,等待回收...");
                    sendArg.RemoteEndPoint = _endPoint;
                    if (!_sendBuff.WriteBuffer(sendArg, seg.Array, seg.Offset, seg.Count))
                    {
                        _sendArgs.Set(sendArg);
                        throw new Exception(string.Format("发送缓冲区溢出...buffer block max size:{0}", _sendBuff.bufferSize));
                    }
                    isWillEvent &= _sock.SendToAsync(sendArg);
                    if (!isWillEvent)
                    {
                        SendCallBack(sendArg);
                    }
                }
                return isWillEvent;
            }
            catch (Exception )
            {
                Close();
                throw ;
            }
        }
        public int SendSync(BufferSegment segSend, BufferSegment segRecieve)
        {
            int sent = _sock.SendTo(segSend.buffer, segSend.offset, segSend.count, 0, _endPoint);
            if (segSend == null|| segSend.buffer == null || segSend.count == 0) return sent;
            /*int cnt =*/ _sock.ReceiveFrom(segRecieve.buffer, segRecieve.offset, segRecieve.count, SocketFlags.None, ref _endPoint);
            return sent;
        }



        private void Close()
        {
            if (_sock!=null)
            {
                _sock.Shutdown(SocketShutdown.Both);
                _sock.Close();
                _sock.Dispose();
                _isConnected = false;
            }
        }
        private void ReceiveCallBack(SocketAsyncEventArgs e)
        {
            SocketToken token = new SocketToken();
            token.sock = e.UserToken as Socket;
            token.endPoint = (IPEndPoint)e.RemoteEndPoint;
            try
            {
                if (e.SocketError != SocketError.Success || e.BytesTransferred == 0) return;
                if (isServerResponse(e)) return;
                if (onReceive != null) onReceive(token, new BufferSegment(e.Buffer, e.Offset, e.BytesTransferred));
                if (onRecieveString != null) onRecieveString(token, _encoding.GetString(e.Buffer, e.Offset, e.BytesTransferred));
            }
            catch (Exception )
            {
                throw ;
            }
            finally
            {
                if (e.SocketError == SocketError.Success)
                {
                    if (!token.sock.ReceiveFromAsync(e))
                    {
                        ReceiveCallBack(e);
                    }
                }
            }
        }
        private void SendCallBack(SocketAsyncEventArgs e)
        {
            try
            {
              bool  sucess = e.SocketError == SocketError.Success;
                if (!_isConnected && sucess)
                {
                    StartReceive();
                    _isConnected = true;
                }
                if (onSendCallback != null && isClientRequest(e) == false)
                {
                    SocketToken token = new SocketToken();
                    token.sock = e.UserToken as Socket;
                    token.endPoint = (IPEndPoint)e.RemoteEndPoint;
                    onSendCallback(token,new BufferSegment( e.Buffer, e.Offset, e.BytesTransferred));
                }
            }
            catch (Exception)
            {
                throw ;
            }
            finally
            {
                _sendArgs.Set(e);
            }
        }
        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ReceiveCallBack(e);
                    break;
                case SocketAsyncOperation.SendTo:
                    SendCallBack(e);
                    break;
            }
        }

        private bool isServerResponse(SocketAsyncEventArgs e)
        {
            _isConnected = e.SocketError == SocketError.Success;
            if (e.BytesTransferred == 1 && e.Buffer[0] == 1)
            {
                _mReset.Set();
                return true;
            }
            else return false;
        }
        private bool isClientRequest(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 1 && e.Buffer[0] == 0)
            {
                return true;
            }
            else return false;
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
