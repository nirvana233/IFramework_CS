using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFramework.Serialization.Simple
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public enum NodeType : byte { Node, Element, Document }
    public enum PacketType : byte { Node, Element, Document, Guid, Name, Type, RealType, Value }
    public enum NodeValueType : ushort
    {
        None, NULL, ComplexType, Int16, Int32, Int64, UInt32, UInt16, UInt64, Float, Double, String,
        Decimal,
        Guid,
        Byte,
        Boolean,
        Char,
        SByte
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
