/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class WSServerToken : IDisposable
    {
        private Encoding _encoding = Encoding.UTF8;
       private TcpServerToken _token = null;
       private List<ConnectionInfo> _connectionPool = null;
       private Timer _threadingTimer = null;
       private int _timeout = 1000 * 60 * 6;
       private object _lockobject = new object();

        public OnReceivedString onRecieveString { get; set; }
        public OnReceieve onReceieve { get; set; }
        public OnAccept onAccept { get; set; }
        public OnDisConnect onDisConnect { get; set; }
        public OnSendCallBack onSendCallBack { get; set; }

        public WSServerToken(int maxCon = 32, int bufferSize = 4096)
        {
            _connectionPool = new List<ConnectionInfo>(maxCon);

            _token = new TcpServerToken(maxCon, bufferSize);
            //serverProvider.AcceptedCallback = new OnAcceptedHandler(AcceptedHandler);
            _token.onDisConnect = new OnDisConnect(OnDisConnect);
            _token.onReceive = new OnReceieve(OnReceieve);
            _token.onSendCallBack = new OnSendCallBack(OnSendCallBack);

            _threadingTimer = new Timer(new TimerCallback(TimingEvent), null, -1, -1);
        }

        public static WSServerToken CreateProvider(int maxConnections = 32, int bufferSize = 4096)
        {
            return new WSServerToken(maxConnections, bufferSize);
        }

        public void Dispose()
        {
            _threadingTimer.Dispose();
        }

        private void TimingEvent(object obj)
        {
            lock (_lockobject)
            {
                var items = _connectionPool.FindAll(x => DateTime.Now.Subtract(x.ConnectedTime).TotalMilliseconds >= (_timeout >> 1));

                foreach (var node in items)
                {
                    if (DateTime.Now.Subtract(node.ConnectedTime).TotalMilliseconds >= _timeout)
                    {
                        CloseAndRemove(node);
                        continue;
                    }
                    SendPing(node.sToken);
                }
            }
        }

        public bool Start(int port, string ip = "0.0.0.0")
        {
            bool isOk = _token.Start(port, ip);
            if (isOk)
            {
                _threadingTimer.Change(_timeout >> 1, _timeout);
            }
            return isOk;
        }

        public void Stop()
        {
            _threadingTimer.Change(-1, -1);
            lock (_lockobject)
            {
                foreach (var node in _connectionPool)
                {
                    CloseAndRemove(node);
                }
            }
        }

        public void Close(SocketToken sToken)
        {
            _token.Close(sToken);
        }

        public bool Send(SocketToken sToken, string content, bool waiting = true)
        {
            var buffer = new WebsocketFrame().ToSegmentFrame(content);

            return _token.SendAsync(sToken,buffer, waiting);
        }

        public bool Send(SocketToken token, BufferSegment seg, bool waiting = true)
        {
            return this._token.SendAsync(token, seg, waiting);
        }

        private void SendPing(SocketToken sToken)
        {
            _token.SendAsync(sToken,new BufferSegment( new byte[] { 0x89, 0x00 }));
        }

        private void SendPong(SocketToken token, BufferSegment seg)
        {
            var buffer = new WebsocketFrame().ToSegmentFrame(seg, OpCodeType.Bong);

            this._token.SendAsync(token, buffer);
        }

        //private void AcceptedHandler(SocketToken sToken)
        //{

        //}

        private void OnDisConnect(SocketToken sToken)
        {
            // ConnectionPool.Remove(new ConnectionInfo() { sToken = sToken });
            Remove(sToken);

            if (onDisConnect != null) onDisConnect(sToken);
        }

        private void RefreshTimeout(SocketToken sToken)
        {
            foreach (var item in _connectionPool)
            {
                if (item.sToken.tokenId == sToken.tokenId)
                {
                    item.ConnectedTime = DateTime.Now;
                    break;
                }
            }
        }

        private void OnReceieve(SocketToken token, BufferSegment seg)
        {
            var connection = _connectionPool.Find(x => x.sToken.tokenId == token.tokenId);
            if (connection == null)
            {
                connection = new ConnectionInfo() { sToken = token };

                _connectionPool.Add(connection);
            }

            if (connection.IsHandShaked == false)
            {
                var serverFrame = new WebsocketFrame();

                var access = serverFrame.GetHandshakePackage(seg);
                connection.IsHandShaked = access.IsHandShaked();

                if (connection.IsHandShaked == false)
                {
                    CloseAndRemove(connection);
                    return;
                }
                connection.ConnectedTime = DateTime.Now;

                var rsp = serverFrame.RspAcceptedFrame(access);

                this._token.SendAsync(token, rsp);

                connection.accessInfo = access;

                if (onAccept != null) onAccept(token);
            }
            else
            {
                RefreshTimeout(token);

                WebsocketFrame packet = new WebsocketFrame();
                bool isOk = packet.DecodingFromBytes(seg, true);
                if (isOk == false) return;

                if (packet.opCode == 0x01)//text
                {
                    if (onRecieveString != null)
                        onRecieveString(token, _encoding.GetString(seg.buffer,
                        seg.offset, seg.count));

                    return;
                }
                else if (packet.opCode == 0x08)//close
                {
                    CloseAndRemove(connection);
                    return;
                }
                else if (packet.opCode == 0x09)//ping
                {
                    SendPong(token,seg);
                }
                else if (packet.opCode == 0x0A)//pong
                {
                    //  SendPing(session.sToken);
                }

                if (onReceieve != null && packet.payload.count > 0)
                    onReceieve(token, packet.payload);
            }
        }

        private void OnSendCallBack(SocketToken token, BufferSegment seg)
        {
            if (onSendCallBack != null)
            {
                onSendCallBack(token,seg);
            }
        }

        private void CloseAndRemove(ConnectionInfo connection)
        {
            bool isOk = Remove(connection);
            if (isOk)
            {
                _token.Close(connection.sToken);
            }
        }

        private bool Remove(ConnectionInfo info)
        {

            return _connectionPool.Remove(info);

        }

        private bool Remove(SocketToken sToken)
        {

            return _connectionPool.RemoveAll(x => x.sToken.tokenId == sToken.tokenId) > 0;

        }

        internal class ConnectionInfo : IComparable<SocketToken>
        {
            public SocketToken sToken { get; set; }

            public bool IsHandShaked { get; set; }

            public AccessInfo accessInfo { get; set; }
            private DateTime conTime = DateTime.MinValue;
            public DateTime ConnectedTime { get { return conTime; } set { conTime = value; } }
            public int CompareTo(SocketToken info)
            {
                return sToken.tokenId - info.tokenId;
            }
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
