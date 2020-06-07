/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Net
{
     class AccessInfo : BaseInfo
    {
        /// <summary>
        /// 连接主机
        /// </summary>
        public string host { get; set; }
        /// <summary>
        /// 连接源
        /// </summary>
        public string origin { get; set; }
        /// <summary>
        /// 安全扩展
        /// </summary>
        public string secWebSocketExtensions { get; set; }
        /// <summary>
        /// 安全密钥
        /// </summary>
        public string secWebSocketKey { get; set; }
        /// <summary>
        /// 安全版本
        /// </summary>
        public string secWebSocketVersion { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(httpProto))
                httpProto = "GET / HTTP/1.1";

            if (string.IsNullOrEmpty(secWebSocketVersion))
                secWebSocketVersion = "13";

            return string.Format("{0}{1}{2}{3}{4}{5}{6}",
                httpProto + NewLine,
                "Host" + SplitChars + host + NewLine,
                "Connection" + SplitChars + connection + NewLine,
                "Upgrade" + SplitChars + upgrade + NewLine,
                "Origin" + SplitChars + origin + NewLine,
                "Sec-WebSocket-Version" + SplitChars + secWebSocketVersion + NewLine,
                "Sec-WebSocket-Key" + SplitChars + secWebSocketKey + NewLine + NewLine);
        }

        public bool IsHandShaked()
        {
            return string.IsNullOrEmpty(secWebSocketKey) == false;
        }
    }

     class BaseInfo
    {
        /// <summary>
        /// http连接协议
        /// </summary>
        public string httpProto { get; set; }
        /// <summary>
        /// 连接类型
        /// </summary>
        public string connection { get; set; }
        /// <summary>
        /// 连接方式
        /// </summary>
        public string upgrade { get; set; }

        internal const string NewLine = "\r\n";
        internal const string SplitChars = ": ";


        public BaseInfo()
        {
            upgrade = "websocket";
            connection = "Upgrade";
        }
    }
     class AcceptInfo : BaseInfo
    {
        /// <summary>
        /// 接入访问验证码
        /// </summary>
        public string secWebSocketAccept { get; set; }
        /// <summary>
        /// 客户端来源
        /// </summary>
        public string secWebSocketLocation { get; set; }
        /// <summary>
        /// 服务端来源
        /// </summary>
        public string secWebSocketOrigin { get; set; }


        public override string ToString()
        {
            if (string.IsNullOrEmpty(httpProto))
                httpProto = "HTTP/1.1 101 Switching Protocols";

            return string.Format("{0}{1}{2}{3}",
                httpProto + NewLine,
                "Connection" + SplitChars + connection + NewLine,
                "Upgrade" + SplitChars + upgrade + NewLine,
                 "Sec-WebSocket-Accept" + SplitChars + secWebSocketAccept + NewLine + NewLine//很重要，需要两个newline
                );
        }

        public bool IsHandShaked()
        {
            return string.IsNullOrEmpty(secWebSocketAccept) == false;
        }
    }
}
