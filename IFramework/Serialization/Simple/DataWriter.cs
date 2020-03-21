using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFramework.Serialization.Simple
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class RefrenceCollection
    {
        Dictionary<object, Guid> _map = new Dictionary<object, Guid>();

        public bool SubscribeObjeect(object obj, out Guid guid)
        {
            Guid _node;
            if (_map.TryGetValue(obj, out _node))
            {
                guid = _node;
                return false;
            }
            guid = Guid.NewGuid();
            return true;
        }


        Dictionary<Guid, object> _map2 = new Dictionary<Guid, object>();
        public bool SubscribeGUID(Guid guid, ref object obj)
        {
            // object _obj;
            if (_map2.TryGetValue(guid, out obj))
                return false;
            else
            {
                _map2.Add(guid, obj);
                return true;
            }
        }


        Dictionary<Guid, List<object>> _map3 = new Dictionary<Guid, List<object>>();
        public void SetResolveObject(Guid guid, object obj)
        {
            if (!_map3.ContainsKey(guid))
                _map3.Add(guid, new List<object>());
            _map3[guid].Add(obj);
        }

        public void Resolve()
        {
            var em = _map3.GetEnumerator();
            while (em.MoveNext())
            {
                var pair = em.Current;
                Guid guid = pair.Key;
                object result;
                if (_map2.TryGetValue(guid, out result))
                {
                    List<object> objs = pair.Value;
                    for (int i = 0; i < objs.Count; i++)
                    {
                        var obj = objs[i];
                        obj = result;
                    }

                }
                else
                {
                    throw new Exception("The GUID Could Not Find Refence " + guid);
                }

            }
        }
    }

    public class DataWriter
    {
        private static Encoding _en = Encoding.Default;

        private PacketElement _cur;
        private PacketDocument _doc;
        public RefrenceCollection refContext { get; set; }
        public DataWriter()
        {
            _doc = new PacketDocument();
            _cur = _doc;
            refContext = new RefrenceCollection();
        }
        private PacketNode PrepareBaseType(NodeValueType valueType, Type type, string name)
        {
            var node = _doc.CreateNode();
            _cur.Append(node);

            string typ= Framework.Assembly.GetTypeName(type);

            node.context.type = typ;
            node.context.realType = typ;
            node.context.name = name;
            node.context.valueType = valueType;

            return node;
        }

        public void WriteChar(string name, char value)
        {
            var node = PrepareBaseType(NodeValueType.Char, typeof(char), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteSbyte(string name, sbyte value)
        {
            var node = PrepareBaseType(NodeValueType.SByte, typeof(sbyte), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }

        public void WriteInt16(string name, short value)
        {
            var node = PrepareBaseType(NodeValueType.Int16, typeof(short), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteInt32(string name, int value)
        {
            var node = PrepareBaseType(NodeValueType.Int32, typeof(int), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteInt64(string name, long value)
        {
            var node = PrepareBaseType(NodeValueType.Int64, typeof(long), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteUInt16(string name, ushort value)
        {
            var node = PrepareBaseType(NodeValueType.UInt16, typeof(ushort), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteUInt32(string name, uint value)
        {
            var node = PrepareBaseType(NodeValueType.UInt32, typeof(uint), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteUInt64(string name, ulong value)
        {
            var node = PrepareBaseType(NodeValueType.UInt64, typeof(ulong), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }

        public void WriteFloat(string name, float value)
        {
            var node = PrepareBaseType(NodeValueType.Float, typeof(float), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteDouble(string name, double value)
        {
            var node = PrepareBaseType(NodeValueType.Double, typeof(double), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteDecimal(string name, decimal value)
        {
            var node = PrepareBaseType(NodeValueType.Decimal, typeof(decimal), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteGuid(string name, Guid value)
        {
            var node = PrepareBaseType(NodeValueType.Guid, typeof(Guid), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteByte(string name, byte value)
        {
            var node = PrepareBaseType(NodeValueType.Byte, typeof(byte), name);
            byte[] bytes = new byte[] { value };
            node.WriteBaseValue(bytes);
        }
        public void WriteBool(string name, bool value)
        {
            var node = PrepareBaseType(NodeValueType.Boolean, typeof(bool), name);
            byte[] bytes = FrameworkBitConverter.GetBytes(value);
            node.WriteBaseValue(bytes);
        }
        public void WriteNUll(string name)
        {
            var node = PrepareBaseType(NodeValueType.NULL, typeof(object), name);
            node.WriteBaseValue(new byte[0] { });
        }

        public void WriteString(string name, string value)
        {
            var node = PrepareBaseType(NodeValueType.String, typeof(float), name);
            byte[] bytes = _en.GetBytes(value);
            node.WriteBaseValue(bytes);

        }


        public void BeginComplexType(string name, Type type, Type realtype, Guid guid)
        {
            var element = _doc.CreateElement();
            _cur.Append(element);

            element.context.name = name;
            element.context.type = Framework.Assembly.GetTypeName(type);
            element.context.realType = Framework.Assembly.GetTypeName(realtype);
            element.context.valueType = NodeValueType.ComplexType;
            element.context.guid = guid;
            _cur = element;
        }
        public void EndComplexType()
        {
            _cur = _cur.parent as PacketElement;
        }

        public byte[] Write()
        {
            return _doc.WritePacket();
        }
        public void Dispose()
        {
        }
    }


    public class DataReader
    {
        private static Encoding _en = Encoding.Default;
        public RefrenceCollection refContext { get; set; }

        private PacketDocument _doc;
        public DataReader(byte[] buffer, int offset, int length)
        {
            refContext = new RefrenceCollection();
            _doc = new PacketDocument();
            _doc.Load(buffer, offset, length);
            _cur = _doc;
        }
        private PacketElement _cur;

        private PacketNode FindChildNode(PacketElement element, string name, Type type, NodeValueType valueType)
        {
            var children = _cur.children;

            return children.Find((c) => {
                return c.context.name == name &&
                        c.context.type == Framework.Assembly.GetTypeName(type) &&
                        c.context.valueType == valueType;
            });
        }
        public void BeginComplexType(string name, Type type, out PackectContext context)
        {
            var child = FindChildNode(_cur, name, type, NodeValueType.ComplexType);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, type));
            if (child.nodeType == NodeType.Element)
                _cur = child as PacketElement;
            else
                throw new Exception("The Child is Not Element Node");
            context = _cur.context;
        }
        public void EndComplexType()
        {
            _cur = _cur.parent as PacketElement;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public sbyte ReadSbyte(string name)
        {
            var child = FindChildNode(_cur, name, typeof(sbyte), NodeValueType.SByte);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(sbyte)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToSByte(child.valuePacket.message, 0);
        }
        public char ReadChar(string name)
        {
            var child = FindChildNode(_cur, name, typeof(bool), NodeValueType.Char);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(char)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToChar(child.valuePacket.message, 0);
        }
        public bool ReadBool(string name)
        {
            var child = FindChildNode(_cur, name, typeof(bool), NodeValueType.Boolean);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(bool)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToBoolean(child.valuePacket.message, 0);
        }
        public byte ReadByte(string name)
        {
            var child = FindChildNode(_cur, name, typeof(byte), NodeValueType.Byte);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(byte)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return child.valuePacket.message[0];
        }
        public decimal ReadDecimal(string name)
        {
            var child = FindChildNode(_cur, name, typeof(decimal), NodeValueType.Decimal);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(decimal)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToDecimal(child.valuePacket.message, 0);
        }
        public double ReadDouble(string name)
        {
            var child = FindChildNode(_cur, name, typeof(double), NodeValueType.Double);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(double)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToDouble(child.valuePacket.message, 0);
        }
        public float ReadFloat(string name)
        {
            var child = FindChildNode(_cur, name, typeof(float), NodeValueType.Float);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(float)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToSingle(child.valuePacket.message, 0);
        }
        public Guid ReadGuid(string name)
        {
            var child = FindChildNode(_cur, name, typeof(Guid), NodeValueType.Guid);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(Guid)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToGuid(child.valuePacket.message, 0);
        }
        public short ReadInt16(string name)
        {
            var child = FindChildNode(_cur, name, typeof(short), NodeValueType.Int16);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(short)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToInt16(child.valuePacket.message, 0);
        }
        public int ReadInt32(string name)
        {
            var child = FindChildNode(_cur, name, typeof(int), NodeValueType.Int32);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(int)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToInt32(child.valuePacket.message, 0);
        }
        public long ReadInt64(string name)
        {
            var child = FindChildNode(_cur, name, typeof(long), NodeValueType.Int64);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(long)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToInt64(child.valuePacket.message, 0);
        }
        public string ReadString(string name)
        {
            var child = FindChildNode(_cur, name, typeof(ushort), NodeValueType.UInt16);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(ushort)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return _en.GetString(child.valuePacket.message, 0, child.valuePacket.message.Length);
        }
        public ushort ReadUInt16(string name)
        {
            var child = FindChildNode(_cur, name, typeof(ushort), NodeValueType.UInt16);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(ushort)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToUInt16(child.valuePacket.message, 0);
        }
        public uint ReadUInt32(string name)
        {
            var child = FindChildNode(_cur, name, typeof(uint), NodeValueType.UInt32);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(uint)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToUInt32(child.valuePacket.message, 0);
        }
        public ulong ReadUInt64(string name)
        {
            var child = FindChildNode(_cur, name, typeof(ulong), NodeValueType.UInt64);
            if (child == null)
                throw new Exception(string.Format("The Node Can't Find  Name : {0} ,Type : {1}", name, typeof(ulong)));
            if (child.nodeType != NodeType.Node)
                throw new Exception("The Child is Not  Node");
            return FrameworkBitConverter.ToUInt64(child.valuePacket.message, 0);
        }

        internal void BeginComplexType(string name, Type type, out object context)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
