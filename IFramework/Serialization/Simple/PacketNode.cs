using IFramework.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFramework.Serialization.Simple
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class PackectContext
    {
        public string realType;
        public string type;
        public NodeValueType valueType;
        public string name;
        public Guid guid = Guid.NewGuid();
        public int innerCount
        {
            get
            {
                if (node is PacketElement)
                    return (node as PacketElement).children.Count;
                return 0;
            }
        }
        public PackectContext(PacketNode node)
        {
            this.node = node;
        }

        public PacketNode node { get; }
    }

    public class PacketNode
    {
        private static Encoding _en = Encoding.Default;
        private Packet _packet;
        private Packet _valuePacket;
        private PacketNode _parent;
        private PackectContext _context;

        public PackectContext context { get { return _context; } }
        protected Packet nodePacket { get { return _packet; } set { _packet = value; } }
        public Packet valuePacket { get { return _valuePacket; } set { _valuePacket = value; } }
        public NodeType nodeType { get; protected set; }
        public PacketDocument doc { get; protected set; }

        public PacketNode parent
        {
            get { return _parent; }
            set
            {
                if (this.nodeType == NodeType.Document)
                    throw new Exception(string.Format("Document can Not set Parent "));
                if (_parent != null)
                    throw new Exception(string.Format("Do Not Change Parent  {0} / {1}", context.name, nodeType));
                if (this.nodeType == NodeType.Element && value.nodeType == NodeType.Node)
                    throw new Exception(string.Format("Element can Not set Parent with Node"));
                else
                {
                    _parent = value;
                }
            }
        }
        public string path
        {
            get
            {
                string path = this.context.name;
                PacketNode node = this;
                while (node.parent != null)
                {
                    node = node.parent;
                    path = path.AppendHead("/").AppendHead(node.context.name);
                }
                return path;
            }
        }

        internal PacketNode(PacketDocument doc)
        {
            this.doc = doc;
            this.nodeType = NodeType.Node;
            this.context.valueType = NodeValueType.None;
            _packet = new Packet();
            _context = new PackectContext(this);
        }

        public virtual void ReadPacket(Packet node_pkg)
        {
            if (this.doc == null)
                throw new Exception("Document is null");
            else if (node_pkg.pkgType != (byte)PacketType.Node)
                throw new Exception("The Packet is Not Element");
            else
            {
                this.nodePacket = node_pkg;
                this.context.valueType = (NodeValueType)nodePacket.pkgID;

                PacketReader pr = new PacketReader(nodePacket.message.Length);
                pr.Set(nodePacket.message, 0, nodePacket.message.Length);
                var pkgs = pr.Get();
                if (nodePacket.pkgCount != 3 || pkgs.Count != 3)
                    throw new Exception("Value not Err");
                for (int i = 0; i < 3; i++)
                {
                    var pkg = pkgs[i];
                    switch ((PacketType)pkg.pkgType)
                    {
                        case PacketType.Node:
                        case PacketType.Element:
                        case PacketType.Document: throw new Exception(string.Format("Node Packet can not Have {0} Packet", (PacketType)pkg.pkgType));
                        case PacketType.Name:
                            ReadName(pkg);
                            break;
                        case PacketType.Guid:
                            ReadGUID(pkg);
                            break;
                        case PacketType.Type:
                            ReadType(pkg);
                            break;
                        case PacketType.RealType:
                            ReadRealType(pkg);
                            break;
                        case PacketType.Value:
                            valuePacket = pkg;
                            break;
                    }
                }
            }
        }
        public virtual byte[] WritePacket()
        {
            List<byte> buffer = new List<byte>();
            buffer.AddRange(CreateGUIDPacket().Pack());
            buffer.AddRange(CreateNamePacket().Pack());
            buffer.AddRange(CreateTypePacket().Pack());
            buffer.AddRange(CreateRealTypePacket().Pack());

            buffer.AddRange(valuePacket.Pack());

            nodePacket = new Packet(3, (ushort)context.valueType, (byte)PacketType.Node, buffer.ToArray());
            return nodePacket.Pack();
        }


        public void WriteBaseValue(byte[] bytes)
        {
            valuePacket = new Packet(0, (ushort)context.valueType, (byte)PacketType.Value, bytes);
        }


        protected void ReadGUID(Packet pkg)
        {
            context.guid = FrameworkBitConverter.ToGuid(pkg.message, 0);
        }
        protected void ReadType(Packet pkg)
        {
            context.type = _en.GetString(pkg.message, 0, pkg.message.Length);

        }
        protected void ReadRealType(Packet pkg)
        {
            context.realType = _en.GetString(pkg.message, 0, pkg.message.Length);
        }
        protected void ReadName(Packet pkg)
        {
            context.name = _en.GetString(pkg.message, 0, pkg.message.Length);
        }
        protected Packet CreateGUIDPacket()
        {
            Packet pkg = new Packet(0, (ushort)NodeValueType.Guid, (byte)PacketType.Guid, FrameworkBitConverter.GetBytes(context.guid));
            return pkg;
        }
        protected Packet CreateNamePacket()
        {
            Packet pkg = new Packet(0, (ushort)NodeValueType.String, (byte)PacketType.Name, _en.GetBytes(context.name));
            return pkg;
        }
        protected Packet CreateTypePacket()
        {
            Packet pkg = new Packet(0, (ushort)NodeValueType.String, (byte)PacketType.Type, _en.GetBytes(context.type));
            return pkg;
        }
        protected Packet CreateRealTypePacket()
        {
            Packet pkg = new Packet(0, (ushort)NodeValueType.String, (byte)PacketType.RealType, _en.GetBytes(context.realType));
            return pkg;
        }

    }
    public class PacketElement : PacketNode
    {
        public List<PacketNode> children
        {
            get
            {
                if (doc == null)
                    return default(List<PacketNode>);
                return doc.GetChildren(this);
            }
        }
        internal PacketElement(PacketDocument doc) : base(doc)
        {
            this.nodeType = NodeType.Element;
            this.context.valueType = NodeValueType.ComplexType;

        }
        public void Append(PacketNode node)
        {
            node.parent = this;
        }
        public override void ReadPacket(Packet ele_pkg)
        {
            if (this.doc == null)
                throw new Exception("Document is null");
            else if (ele_pkg.pkgType != (byte)PacketType.Element)
                throw new Exception("The Packet is Not Element");
            else
            {
                this.nodePacket = ele_pkg;
                this.context.valueType = (NodeValueType)nodePacket.pkgID;

                PacketReader pr = new PacketReader(nodePacket.message.Length);

                pr.Set(nodePacket.message, 0, nodePacket.message.Length);
                var pkgs = pr.Get();

                if (pkgs.Count != nodePacket.pkgCount)
                    throw new Exception("Inner Packets Count Err");
                else
                {
                    for (int i = 0; i < pkgs.Count; i++)
                    {
                        var pkg = pkgs[i];
                        switch ((PacketType)pkg.pkgType)
                        {
                            case PacketType.Node:
                                var node = this.doc.CreateNode();
                                this.Append(node);
                                node.ReadPacket(pkg);
                                break;
                            case PacketType.Element:
                                var element = this.doc.CreateElement();
                                this.Append(element);
                                element.ReadPacket(pkg);
                                break;
                            case PacketType.Name:
                                ReadName(pkg);
                                break;
                            case PacketType.Guid:
                                ReadGUID(pkg);
                                break;
                            case PacketType.Type:
                                ReadType(pkg);
                                break;
                            case PacketType.RealType:
                                ReadRealType(pkg);
                                break;
                            case PacketType.Document:
                            case PacketType.Value:
                                throw new Exception(string.Format("Node Packet can not Have {0} Packet", (PacketType)pkg.pkgType));
                        }

                    }

                }
            }
        }
        public override byte[] WritePacket()
        {
            //     var children = doc.GetChildren(this);
            if (children.Count >= ushort.MaxValue)
                throw new Exception("The Type Public Member is so much More");
            else
            {
                List<byte> buffer = new List<byte>();
                buffer.AddRange(CreateGUIDPacket().Pack());
                buffer.AddRange(CreateNamePacket().Pack());
                buffer.AddRange(CreateTypePacket().Pack());
                buffer.AddRange(CreateRealTypePacket().Pack());

                for (int i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    buffer.AddRange(child.WritePacket());
                }

                nodePacket = new Packet((ushort)(children.Count + 2), (ushort)context.valueType, (byte)PacketType.Element, buffer.ToArray());
                return nodePacket.Pack();
            }
        }

    }
    public class PacketDocument : PacketElement
    {
        public PacketDocument() : base(null)
        {
            this.context.valueType = NodeValueType.ComplexType;
            this.nodeType = NodeType.Document;
            _nodes = new List<PacketNode>();
            context.name = "root";
            context.type = "root";
            doc = this;
        }
        public List<PacketNode> _nodes;
        public void Load(byte[] buffer, int offset, int length)
        {
            PacketReader pr0 = new PacketReader(length);
            pr0.Set(buffer, offset, length);
            var pkgs = pr0.Get();
            if (pkgs.Count != 1)
                throw new Exception("Buffer Err: Can't Load As Document");
            else
                ReadPacket(pkgs[0]);
        }
        public override void ReadPacket(Packet doc_pkg)
        {

            if (doc_pkg.pkgType != (byte)PacketType.Document)
                throw new Exception("The Stream Could not Read");
            else
            {
                this.nodePacket = doc_pkg;
                this.context.valueType = (NodeValueType)nodePacket.pkgID;

                byte[] buff = nodePacket.message;

                PacketReader pr = new PacketReader(buff.Length * 2);

                pr.Set(buff, 0, buff.Length);
                var pkgs = pr.Get();

                if (pkgs.Count != nodePacket.pkgCount)
                    throw new Exception("Inner Packets Count Err");
                else
                {
                    for (int i = 0; i < pkgs.Count; i++)
                    {
                        var pkg = pkgs[i];
                        switch ((PacketType)pkg.pkgType)
                        {
                            case PacketType.Node:
                                var node = this.CreateNode();
                                this.Append(node);
                                node.ReadPacket(pkg);
                                break;
                            case PacketType.Element:
                                var element = this.CreateElement();
                                this.Append(element);
                                element.ReadPacket(pkg);
                                break;
                            case PacketType.Name:
                                ReadName(pkg);
                                break;
                            case PacketType.Guid:
                                ReadGUID(pkg);
                                break;
                            case PacketType.Type:
                                ReadType(pkg);
                                break;
                            case PacketType.RealType:
                                ReadRealType(pkg);
                                break;
                            case PacketType.Document:
                            case PacketType.Value:
                                throw new Exception(string.Format("Node Packet can not Have {0} Packet", (PacketType)pkg.pkgType));
                        }

                    }
                }
            }

        }

        public override byte[] WritePacket()
        {
            if (children.Count >= ushort.MaxValue)
                throw new Exception("The Type Public Member is so much More");
            else
            {
                List<byte> buffer = new List<byte>();
                buffer.AddRange(CreateGUIDPacket().Pack());
                buffer.AddRange(CreateNamePacket().Pack());
                buffer.AddRange(CreateTypePacket().Pack());
                buffer.AddRange(CreateRealTypePacket().Pack());

                for (int i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    buffer.AddRange(child.WritePacket());
                }

                nodePacket = new Packet((ushort)(children.Count + 2), (ushort)context.valueType, (byte)PacketType.Document, buffer.ToArray());
                return nodePacket.Pack();
            }
        }




        internal List<PacketNode> GetChildren(PacketElement ele_node)
        {
            return _nodes.FindAll((node) => { return node.parent == ele_node; });
        }
        public PacketNode CreateNode()
        {
            PacketNode node = new PacketNode(this);
            _nodes.Add(node);
            return node;
        }
        public PacketElement CreateElement()
        {
            PacketElement node = new PacketElement(this);
            _nodes.Add(node);
            return node;
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
