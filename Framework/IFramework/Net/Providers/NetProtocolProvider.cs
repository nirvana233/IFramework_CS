﻿using IFramework.Packets;

namespace IFramework.Net
{
    internal class NetProtocolProvider : INetProtocolProvider
    {
        public static NetProtocolProvider CreateProvider()
        {
            return new NetProtocolProvider();
        }

        public NetProtocolProvider()
        { }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Packet Decode(byte[] buffer, int offset, int size)
        {
            Packet pkg = new Packet();
            if (pkg.UnPack(buffer, offset, size)) return pkg;
            else return null;
        }

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="pkg"></param>
        /// <returns></returns>
        public byte[] Encode(Packet pkg)
        {
            return pkg.Pack();
        }
    }
}
