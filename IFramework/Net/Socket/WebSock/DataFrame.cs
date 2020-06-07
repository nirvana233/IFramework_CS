/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Packets;
using System;
using System.Text;

namespace IFramework.Net
{
     class DataFrame
    {
        /// <summary>
        /// 如果为true则该消息为消息尾部,如果false为零则还有后续数据包;
        /// </summary>
        private bool _isEof = true;
        public bool isEof { get { return _isEof; } set { _isEof = value; } }
        /// <summary>
        /// RSV1,RSV2,RSV3,各1位，用于扩展定义的,如果没有扩展约定的情况则必须为0
        /// </summary>
        public bool rsv1 { get; set; }

        public bool rsv2 { get; set; }

        public bool rsv3 { get; set; }
        /// <summary>
        ///0x0表示附加数据帧
        ///0x1表示文本数据帧
        ///0x2表示二进制数据帧
        ///0x3-7暂时无定义，为以后的非控制帧保留
        ///0x8表示连接关闭
        ///0x9表示ping
        ///0xA表示pong
        ///0xB-F暂时无定义，为以后的控制帧保留
        /// </summary>
        private byte _opCode = 0x01;

        public byte opCode { get { return _opCode; } set { _opCode = value; } }
        /// <summary>
        /// true使用掩码解析消息
        /// </summary>
        private bool _mask = false;

        public bool mask { get { return _mask; } set { _mask = value; } }

        public long payloadLength { get; set; }

        //public int Continued { get; set; }

        public byte[] maskKey { get; set; }

        //public UInt16 MaskKeyContinued { get; set; }

        public BufferSegment payload { get; set; }

        public byte[] EncodingToBytes()
        {
            if (payload == null
                || payload.buffer.LongLength != payloadLength)
                throw new Exception("payload buffer error");


            if (payload.count > 0)
            {
                payloadLength = payload.count;
            }

            long headLen = (mask ? 6 : 2);
            if (payloadLength < 126)
            { }
            else if (payloadLength >= 126 && payloadLength < 127)
            {
                headLen += 2;
            }
            else if (payloadLength >= 127)
            {
                headLen += 8;
            }

            byte[] buffer = new byte[headLen + payloadLength];
            int pos = 0;

            buffer[pos] = (byte)(isEof ? 128 : 0);
            buffer[pos] += opCode;

            buffer[++pos] = (byte)(mask ? 128 : 0);
            if (payloadLength < 0x7e)//126
            {
                buffer[pos] += (byte)payloadLength;
            }
            else if (payloadLength < 0xffff)//65535
            {
                buffer[++pos] = 126;
                buffer[++pos] = (byte)(buffer.Length >> 8);
                buffer[++pos] = (byte)(buffer.Length);
            }
            else
            {
                var payLengthBytes = ((ulong)payloadLength).ToBytes();
                buffer[++pos] = 127;

                buffer[++pos] = payLengthBytes[0];
                buffer[++pos] = payLengthBytes[1];
                buffer[++pos] = payLengthBytes[2];
                buffer[++pos] = payLengthBytes[3];
                buffer[++pos] = payLengthBytes[4];
                buffer[++pos] = payLengthBytes[5];
                buffer[++pos] = payLengthBytes[6];
                buffer[++pos] = payLengthBytes[7];
            }

            if (mask)
            {
                buffer[++pos] = maskKey[0];
                buffer[++pos] = maskKey[1];
                buffer[++pos] = maskKey[2];
                buffer[++pos] = maskKey[3];

                for (long i = 0; i < payloadLength; ++i)
                {
                    buffer[headLen + i] = (byte)(payload.buffer[i + payload.offset] ^ maskKey[i % 4]);
                }
            }
            else
            {
                for (long i = 0; i < payloadLength; ++i)
                {
                    buffer[headLen + i] = payload.buffer[i + payload.offset];
                }
            }
            return buffer;
        }

        public bool DecodingFromBytes(BufferSegment data, bool isMaskResolve = true)
        {
            if (data.count < 4) return false;

            int pos = data.offset;

            isEof = (data.buffer[pos] >> 7) == 1;
            opCode = (byte)(data.buffer[pos] & 0xf);

            mask = (data.buffer[++pos] >> 7) == 1;
            payloadLength = (data.buffer[pos] & 0x7f);

            //校验截取长度
            if (payloadLength >= data.count) return false;

            ++pos;
            //数据包长度超过126，需要解析附加数据
            if (payloadLength < 126)
            {
                //直接等于消息长度
            }
            if (payloadLength == 126)
            {
                payloadLength = data.buffer.ToUInt16(pos);// BitConverter.ToUInt16(segOffset.buffer, pos);
                pos += 2;
            }
            else if (payloadLength == 127)
            {
                payloadLength = (long)data.buffer.ToUInt64(pos);
                pos += 8;
            }

            payload = new BufferSegment()
            {
                offset = pos,
                buffer = data.buffer,
                count = (int)payloadLength
            };

            //数据体
            if (mask)
            {
                //获取掩码密钥
                maskKey = new byte[4];
                maskKey[0] = data.buffer[pos];
                maskKey[1] = data.buffer[pos + 1];
                maskKey[2] = data.buffer[pos + 2];
                maskKey[3] = data.buffer[pos + 3];
                pos += 4;

                payload.buffer = data.buffer;
                payload.offset = pos;
                if (isMaskResolve)
                {
                    long p = 0;

                    for (long i = 0; i < payloadLength; ++i)
                    {
                        p = (long)pos + i;

                        payload.buffer[p] = (byte)(payload.buffer[p] ^ maskKey[i % 4]);
                    }
                }
            }
            else
            {
                payload.buffer = data.buffer;
                payload.offset = pos;
            }

            return true;
        }
    }
     class WebsocketFrame : DataFrame
    {
        private Encoding _encoding = Encoding.UTF8;
        private const string _acceptMask = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";//固定字符串
        private readonly char[] _splitChars = null;

        public WebsocketFrame()
        {
            _splitChars = BaseInfo.SplitChars.ToCharArray();
        }

        public BufferSegment RspAcceptedFrame(AccessInfo access)
        {
            var accept = new AcceptInfo()
            {
                connection = access.connection,
                upgrade = access.upgrade,
                secWebSocketLocation = access.host,
                secWebSocketOrigin = access.origin,
                secWebSocketAccept = (access.secWebSocketKey + _acceptMask).ToSha1Base64(_encoding)
            };

            return new BufferSegment(_encoding.GetBytes(accept.ToString()));
        }

        public AcceptInfo ParseAcceptedFrame(string msg)
        {
            string[] msgs = msg.Split(BaseInfo.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var acceptInfo = new AcceptInfo
            {
                httpProto = msgs[0]
            };

            foreach (var item in msgs)
            {
                string[] kv = item.Split(_splitChars, StringSplitOptions.RemoveEmptyEntries);
                switch (kv[0])
                {
                    case "Upgrade":
                        acceptInfo.upgrade = kv[1];
                        break;
                    case "Connection":
                        acceptInfo.connection = kv[1];
                        break;
                    case "Sec-WebSocket-Accept":
                        acceptInfo.secWebSocketAccept = kv[1];
                        break;
                    case "Sec-WebSocket-Location":
                        acceptInfo.secWebSocketLocation = kv[1];
                        break;
                    case "Sec-WebSocket-Origin":
                        acceptInfo.secWebSocketOrigin = kv[1];
                        break;
                }
            }
            return acceptInfo;
        }

        public BufferSegment ToSegmentFrame(string content)
        {
            var buf = _encoding.GetBytes(content);
            payload = new BufferSegment()
            {
                buffer = buf
            };

            payloadLength = payload.buffer.LongLength;

            return new BufferSegment(EncodingToBytes());
        }

        public BufferSegment ToSegmentFrame(byte[] buf, OpCodeType code = OpCodeType.Text)
        {
            opCode = (byte)code;

            payload = new BufferSegment()
            {
                buffer = buf
            };
            payloadLength = payload.buffer.LongLength;

            return new BufferSegment(EncodingToBytes());
        }

        public BufferSegment ToSegmentFrame(BufferSegment data, OpCodeType code = OpCodeType.Text)
        {
            opCode = (byte)code;

            payload = data;
            payloadLength = payload.buffer.LongLength;

            return new BufferSegment(EncodingToBytes());
        }

        public AccessInfo GetHandshakePackage(BufferSegment segOffset)
        {
            string msg = _encoding.GetString(segOffset.buffer, segOffset.offset, segOffset.count);

            string[] items = msg.Split(BaseInfo.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (items.Length < 6)
                throw new Exception("access format error..." + msg);

            AccessInfo access = new AccessInfo()
            {
                httpProto = items[0]
            };

            foreach (var item in items)
            {
                string[] kv = item.Split(_splitChars, StringSplitOptions.RemoveEmptyEntries);
                switch (kv[0])
                {
                    case "Connection":
                        access.connection = kv[1];
                        break;
                    case "Host":
                        access.host = kv[1];
                        break;
                    case "Origin":
                        access.origin = kv[1];
                        break;
                    case "Upgrade":
                        access.upgrade = kv[1];
                        break;
                    case "Sec-WebSocket-Key":
                        access.secWebSocketKey = kv[1];
                        break;
                    case "Sec-WebSocket-Version":
                        access.secWebSocketVersion = kv[1];
                        break;
                    case "Sec-WebSocket-Extensions":
                        access.secWebSocketExtensions = kv[1];
                        break;
                }
            }
            return access;
        }
    }

    [Flags]
     enum OpCodeType : byte
    {
        Attach = 0x0,
        Text = 0x1,
        Bin = 0x2,
        Close = 0x8,
        Bing = 0x9,
        Bong = 0xA,
    }
}
