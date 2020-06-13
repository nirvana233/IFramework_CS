/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.Packets
{
     class PacketHeader
    {
        public UInt32 pkgID { get; set; }
        public byte pkgType { get; set; }
        public UInt16 pkgCount { get; set; } /*= 1;*/
        public UInt32 messageLen { get; internal set; }
    }
}
