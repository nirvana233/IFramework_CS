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

namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class TcpSocket
    {
        private Socket _sock = null;
        private bool _connected = false;
        private EndPoint _endPoint = null;
        protected byte[] _recBuffer = null;
        private int _recTimeout = 1000 * 60 * 30;
        private int _sendTimeout = 1000 * 60 * 30;
        private int _connTimeout = 1000 * 60 * 30;
        private int _bufferSize = 4096;
        public TcpSocket(int bufferSize)
        {
            this._bufferSize = bufferSize;
        }

        public Socket sock { get { return _sock; } }
        public int recTimeout { get { return _recTimeout; } }
        public int sendTimeout { get { return _sendTimeout; } }
        public int connTimeout { get { return _connTimeout; } }
        public EndPoint endPoint { get { return _endPoint; } }
        public int bufferSize { get { return _bufferSize; } protected set { _bufferSize = value; } }
        public bool connected { get { return _connected; } protected set { _connected = value; } }

        protected void CreateTcpSocket(int port, string ip)
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _sock = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                LingerState = new LingerOption(true, 0),
                NoDelay = true,
                ReceiveTimeout = _recTimeout,
                SendTimeout = _sendTimeout
            };
            _sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }

        protected void SafeClose()
        {
            if (_sock == null) return;
            if (_sock.Connected)
            {
                try
                {
                    _sock.Disconnect(true);
                    _sock.Shutdown(SocketShutdown.Send);
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (Exception )
                {
                    throw ;
                }
            }
            try
            {
                _sock.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        protected void SafeClose(Socket s)
        {
            if (s == null) return;
            if (s.Connected)
            {
                try
                {
                    s.Disconnect(true);
                    s.Shutdown(SocketShutdown.Send);
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
            try
            {
                s.Close();
                //s.Dispose();
                s = null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
