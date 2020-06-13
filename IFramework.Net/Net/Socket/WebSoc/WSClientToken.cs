/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Text;
using System.Threading;

namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class WSClientToken : IDisposable
    {
       private TcpClientToken _token = null;
        private Encoding _encoding = Encoding.UTF8;
      private  ManualResetEvent _resetEvent = new ManualResetEvent(false);
       private int _waitingTimeout = 1000 * 60 * 30;
        public bool IsConnected { get; private set; }
       private AcceptInfo _acceptInfo = null;

        public OnDisConnect onDisConnect { get; set; }

        public OnConnect onConnect { get; set; }

        public OnReceivedString onReceivedString { get; set; }
        public OnReceieve onReceieve { get; set; }
        public OnSendCallBack onSendCallBack { get; set; }
        public WSClientToken(int bufferSize = 4096, int blocks = 8)
        {
            _token = new TcpClientToken(bufferSize, blocks);
            _token.onDisConnect = new OnDisConnect(DisconnectedHandler);
            _token.onReceive = new OnReceieve(ReceivedHanlder);
            _token.onSendCallBack = new OnSendCallBack(SentHandler);
        }

        public void Dispose()
        {
            _resetEvent.Dispose();
            //resetEvent.Close();
        }

        public static WSClientToken CreateProvider(int bufferSize = 4096, int blocks = 8)
        {
            return new WSClientToken(bufferSize, blocks);
        }

        /// <summary>
        /// wsUrl:ws://ip:port
        /// </summary>
        /// <param name="wsUrl"></param>
        /// <returns></returns>
        public bool Connect(string wsUrl)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            WSConnectionItem wsItem = new WSConnectionItem(wsUrl);

            bool isOk = _token.ConnectTo(wsItem.port, wsItem.domain);
            if (isOk == false) throw new Exception("连接失败...");

            string req = new AccessInfo()
            {
                host = wsItem.host,
                origin = "http://" + wsItem.host,
                secWebSocketKey = Convert.ToBase64String(_encoding.GetBytes(wsUrl + rand.Next(100, 100000).ToString()))
            }.ToString();

            isOk = _token.SendAsync(new BufferSegment(_encoding.GetBytes(req)));

            _resetEvent.WaitOne(_waitingTimeout);

            return IsConnected;
        }

        public bool Connect(WSConnectionItem wsUrl)
        {
            return Connect(wsUrl);
        }

        public bool Send(string msg, bool waiting = true)
        {
            if (IsConnected == false) return false;

            var buf = new WebsocketFrame().ToSegmentFrame(msg);
            _token.SendAsync(buf, waiting);
            return true;
        }

        public bool Send(BufferSegment data, bool waiting = true)
        {
            if (IsConnected == false) return false;

            _token.SendAsync(data, waiting);
            return true;
        }

        public void SendPong(BufferSegment buf)
        {
            var seg = new WebsocketFrame().ToSegmentFrame(buf, OpCodeType.Bong);
            _token.SendAsync(seg, true);
        }

        public void SendPing()
        {
            var buf = new WebsocketFrame().ToSegmentFrame(new byte[] { }, OpCodeType.Bing);
            _token.SendAsync(buf, true);
        }

        private void DisconnectedHandler(SocketToken sToken)
        {
            IsConnected = false;
            if (onDisConnect != null) onDisConnect(sToken);
        }

        private void ReceivedHanlder(SocketToken token,BufferSegment seg)
        {
            if (IsConnected == false)
            {
                string msg = _encoding.GetString(seg.buffer, seg.offset, seg.count);
                _acceptInfo = new WebsocketFrame().ParseAcceptedFrame(msg);

                if ((IsConnected = _acceptInfo.IsHandShaked()))
                {
                    _resetEvent.Set();
                    if (onConnect != null) onConnect(token, IsConnected);
                }
                else
                {
                    this._token.DisConnect();
                }
            }
            else
            {
                WebsocketFrame packet = new WebsocketFrame();
                bool isOk = packet.DecodingFromBytes(seg, true);
                if (isOk == false) return;

                if (packet.opCode == 0x01)
                {
                    if (onReceivedString != null)
                        onReceivedString(token, _encoding.GetString(packet.payload.buffer,
                        packet.payload.offset, packet.payload.count));

                    return;
                }
                else if (packet.opCode == 0x08)//close
                {
                    IsConnected = false;
                    this._token.DisConnect();
                }
                else if (packet.opCode == 0x09)//ping
                {
                    SendPong(seg);
                }
                else if (packet.opCode == 0x0A)//pong
                {
                    SendPing();
                }

                if (onReceieve != null && packet.payload.count > 0)
                    onReceieve(token, packet.payload);
            }
        }
        private void SentHandler(SocketToken token, BufferSegment seg)
        {
            if (onSendCallBack != null)
            {
                onSendCallBack(token ,seg);
            }
        }
        //private void ConnectedHandler(SocketToken sToken,bool isConnected)
        //{

        //}
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
