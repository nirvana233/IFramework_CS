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
using System.Text;

namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class UdpServerToken : UdpSocket, IDisposable
    {
        private SocketReceive _socketRecieve = null;
        private SocketSend _socketSend = null;
        private bool _isDisposed = false;
        private Encoding _encoding = Encoding.UTF8;
        private int _recBufferSize = 4096;
        private int _maxConn = 8;

        public OnReceieve onReceive { get; set; }
        public OnSendCallBack onSendCallback { get; set; }
        public OnReceivedString onRecieveString { get; set; }
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
                _socketRecieve.Dispose();
                _socketSend.Dispose();
                _isDisposed = true;
            }
        }
        public UdpServerToken(int maxConn, int recBufferSize, bool Broadcast = false) : base(recBufferSize, Broadcast)
        {
            this._recBufferSize = recBufferSize;
            this._maxConn = maxConn;
        }
        public void Start(int port)
        {
            _socketRecieve = new SocketReceive(port, _maxConn, _broadcast);
            _socketRecieve.OnReceived += ReceiveCallBack;
            _socketRecieve.StartReceive();
            _socketSend = new SocketSend(_socketRecieve._sock, _maxConn, _recBufferSize);
            _socketSend.SendEventHandler += SendCallBack;
        }
        public void Stop()
        {
            if (_socketSend != null)
            {
                _socketSend.Dispose();
            }
            if (_socketRecieve != null)
            {
                _socketRecieve.StopReceive();
            }
        }

        public bool SendAsync(BufferSegment seg, IPEndPoint remoteEP, bool waiting = true)
        {
            return _socketSend.Send(seg, remoteEP, waiting);
        }
        public int SendSync(IPEndPoint remoteEP, BufferSegment seg)
        {
            return _socketSend.SendSync(seg, remoteEP);
        }
        private void SendCallBack(object sender, SocketAsyncEventArgs e)
        {
            if (onSendCallback != null && isServerResponse(e) == false)
            {
                onSendCallback(new SocketToken()
                {
                    endPoint = (IPEndPoint)e.RemoteEndPoint
                }, new BufferSegment(e.Buffer, e.Offset, e.BytesTransferred));
            }
        }

        private void ReceiveCallBack(object sender, SocketAsyncEventArgs e)
        {
            //if (isClientRequest(e)) return;
            SocketToken token = new SocketToken()
            {
                endPoint = (IPEndPoint)e.RemoteEndPoint
            };
            if (onReceive != null)
            {
                //if (e.BytesTransferred > 0)
                {
                    onReceive(token, new BufferSegment(e.Buffer, e.Offset, e.BytesTransferred));
                }
            }
            if (onRecieveString != null && e.BytesTransferred > 0)
            {
                onRecieveString(token, _encoding.GetString(e.Buffer, e.Offset, e.BytesTransferred));
            }
        }

        private bool isClientRequest(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 1 && e.Buffer[0] == 0)
            {
                _socketSend.Send(new BufferSegment(new byte[] { 1 }, 0, 1), (IPEndPoint)e.RemoteEndPoint, true);
                return true;
            }
            else return false;
        }

        private bool isServerResponse(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 1 && e.Buffer[0] == 1)
            {
                return true;
            }
            else return false;
        }

    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}