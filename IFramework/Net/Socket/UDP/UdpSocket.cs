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

namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class UdpSocket
    {
        internal Socket _sock;
        protected bool _isConnected;
        protected EndPoint _endPoint;
        protected byte[] _recBuffer;
        internal bool _broadcast = false;

        protected int _bufferSize = 4096;
        protected int _receiveTimeout = 1000 * 60 * 30;
        protected int _sendTimeout = 1000 * 60 * 30;

        public UdpSocket(int bufferSize, bool broadcast = false)
        {
            _broadcast = broadcast;
            this._bufferSize = bufferSize;
            this._recBuffer = new byte[bufferSize];
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
            }
            try
            {
                _sock.Close();
                _sock.Dispose();
            }
            catch
            { }
        }
        protected void CreateUdpSocket(int port, IPAddress ip)
        {
            if (_broadcast)
                _endPoint = new IPEndPoint(IPAddress.Broadcast, port);
            else
                _endPoint = new IPEndPoint(ip, port);

            _sock = new Socket(_endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveTimeout = _receiveTimeout,
                SendTimeout = _sendTimeout
            };
            try
            {
                if (_broadcast)
                    _sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                else
                    _sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
            }
            catch (Exception)
            {
                 
            }
           
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
