using IFramework.Net;
using IFramework.Packets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
namespace IFramework.Serialization.Simple
{



#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public abstract class Serializer
    {
        private static readonly Dictionary<Type, Type> _map = new Dictionary<Type, Type>()
        {
            { typeof(char), typeof(CharSerializer) },
            { typeof(string), typeof(StringSerializer) },
            { typeof(sbyte), typeof(SByteSerializer) },
            { typeof(short), typeof(Int16Serializer) },
            { typeof(int), typeof(Int32Serializer) },
            { typeof(long), typeof(Int64Serializer) },
            { typeof(byte), typeof(ByteSerializer) },
            { typeof(ushort), typeof(UInt16Serializer) },
            { typeof(uint),   typeof(UInt32Serializer) },
            { typeof(ulong),  typeof(UInt64Serializer) },
            { typeof(decimal),   typeof(DecimalSerializer) },
            { typeof(bool),  typeof(BooleanSerializer) },
            { typeof(float),   typeof(FloatSerializer) },
            { typeof(double),  typeof(DoubleSerializer) },
            { typeof(IntPtr),   typeof(IntPtrSerializer) },
            { typeof(UIntPtr),  typeof(UIntPtrSerializer) },
            { typeof(Guid),  typeof(GuidSerializer) },
            { typeof(DateTime),  typeof(DateTimeSerializer) },
            { typeof(Point2),  typeof(Point2Serializer) },
            { typeof(Point3),  typeof(Point3Serializer) }

        };
        private static readonly Dictionary<Type, Serializer> _ins = new Dictionary<Type, Serializer>();
        private static readonly object _lock = new object();

        public static event Func<Type,Serializer> onSerialize;


        public static void SubscribeSerializer<T>(Type serializer)
        {
            SubscribeSerializer(typeof(T), serializer);
        }
        public static void SubscribeSerializer(Type type, Type serializer)
        {
            lock (_lock)
            {
                if (!_map.ContainsKey(type))
                {
                    _map.Add(type, serializer);
                }
                else
                {
                    _map[type] = serializer;
                }
            }
        }


        public static bool IsPrim(Type type)
        {
            return type.IsEnum || type.IsArray || _map.ContainsKey(type);
        }

        public static Serializer Get(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException();
            }
            Serializer result;
            lock (_lock)
            {
                if (_ins.TryGetValue(type, out result) == false)
                {
                    result = Create(type);
                    _ins.Add(type, result);
                }
            }
            return result;
        }
        public static Serializer<T> Get<T>()
        {
            return (Serializer<T>)Serializer.Get(typeof(T));
        }



        public abstract object ReadValueWeak(string name, DataReader reader);
        public void WriteValueWeak(object value, DataWriter writer)
        {
            this.WriteValueWeak(null, value, writer);
        }
        public abstract void WriteValueWeak(string name, object value, DataWriter writer);

        private static Serializer Create(Type type)
        {
            try
            {
                if (onSerialize!=null)
                {
                    var list = onSerialize.GetInvocationList();
                    foreach (var item in list)
                    {
                        var obj = (item as Func<Type, Serializer>).Invoke(type);
                        if (obj != null)
                        {
                            return obj;
                        }
                    }
                  
                }
                Type resultType = null;

                if (type.IsEnum)
                    resultType = typeof(EnumSerializer<>).MakeGenericType(type);

                else if (type .IsArray)
                    resultType = typeof(ArraySerializer<>).MakeGenericType(type);
                else if (type.IsSubclassOfGeneric(typeof(List<>)) )
                    resultType = typeof(ListSerializer<>).MakeGenericType(type);

                else if (_map.ContainsKey(type))
                {
                    try
                    {
                        resultType = _map[type];
                    }
                    catch (KeyNotFoundException)
                    {
                        Log.E("Failed to find primitive serializer for " + type.Name);
                    }
                }
                else
                {
                    resultType = typeof(ComplexTypeSerializer<>).MakeGenericType(type);
                }

                return (Serializer)Activator.CreateInstance(resultType);
            }
            catch (Exception )
            {
                throw ;
            }
        }
    }
    public abstract class Serializer<T> : Serializer
    {
        public override object ReadValueWeak(string name, DataReader reader)
        {
            return this.ReadValue(name, reader);
        }


        public abstract T ReadValue(string name, DataReader reader);



        public void WriteValue(T value, DataWriter writer)
        {
            this.WriteValue(null, value, writer);
        }
        public override void WriteValueWeak(string name, object value, DataWriter writer)
        {
            this.WriteValue(name, (T)value, writer);
        }
        public abstract void WriteValue(string name, T value, DataWriter writer);


    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class StringSerializer : Serializer<string>
    {
        public override string ReadValue(string name, DataReader reader)
        {
            return reader.ReadString(name);
        }

        public override void WriteValue(string name, string value, DataWriter writer)
        {
            writer.WriteString(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class UInt16Serializer : Serializer<ushort>
    {
        public override ushort ReadValue(string name, DataReader reader)
        {
            return reader.ReadUInt16(name);
        }

        public override void WriteValue(string name, ushort value, DataWriter writer)
        {
            writer.WriteUInt16(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class UInt32Serializer : Serializer<uint>
    {
        public override uint ReadValue(string name, DataReader reader)
        {
            return reader.ReadUInt32(name);
        }

        public override void WriteValue(string name, uint value, DataWriter writer)
        {
            writer.WriteUInt32(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class UInt64Serializer : Serializer<ulong>
    {
        public override ulong ReadValue(string name, DataReader reader)
        {
            return reader.ReadUInt64(name);
        }

        public override void WriteValue(string name, ulong value, DataWriter writer)
        {
            writer.WriteUInt64(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class Int16Serializer : Serializer<short>
    {
        public override short ReadValue(string name, DataReader reader)
        {
            return reader.ReadInt16(name);
        }

        public override void WriteValue(string name, short value, DataWriter writer)
        {
            writer.WriteInt16(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class Int32Serializer : Serializer<int>
    {
        public override int ReadValue(string name, DataReader reader)
        {
            return reader.ReadInt32(name);
        }

        public override void WriteValue(string name, int value, DataWriter writer)
        {
            writer.WriteInt32(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class Int64Serializer : Serializer<long>
    {
        public override long ReadValue(string name, DataReader reader)
        {
            return reader.ReadInt64(name);
        }

        public override void WriteValue(string name, long value, DataWriter writer)
        {
            writer.WriteInt64(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class FloatSerializer : Serializer<float>
    {
        public override float ReadValue(string name, DataReader reader)
        {
            return reader.ReadFloat(name);
        }

        public override void WriteValue(string name, float value, DataWriter writer)
        {
            writer.WriteFloat(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class DoubleSerializer : Serializer<double>
    {
        public override double ReadValue(string name, DataReader reader)
        {
            return reader.ReadDouble(name);
        }

        public override void WriteValue(string name, double value, DataWriter writer)
        {
            writer.WriteDouble(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class DecimalSerializer : Serializer<decimal>
    {
        public override decimal ReadValue(string name, DataReader reader)
        {
            return reader.ReadDecimal(name);
        }

        public override void WriteValue(string name, decimal value, DataWriter writer)
        {
            writer.WriteDecimal(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class ByteSerializer : Serializer<byte>
    {
        public override byte ReadValue(string name, DataReader reader)
        {
            return reader.ReadByte(name);
        }

        public override void WriteValue(string name, byte value, DataWriter writer)
        {
            writer.WriteByte(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class BooleanSerializer : Serializer<bool>
    {
        public override bool ReadValue(string name, DataReader reader)
        {
            return reader.ReadBool(name);
        }

        public override void WriteValue(string name, bool value, DataWriter writer)
        {
            writer.WriteBool(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class CharSerializer : Serializer<char>
    {
        public override char ReadValue(string name, DataReader reader)
        {
            return reader.ReadChar(name);
        }

        public override void WriteValue(string name, char value, DataWriter writer)
        {
            writer.WriteChar(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class GuidSerializer : Serializer<Guid>
    {
        public override Guid ReadValue(string name, DataReader reader)
        {
            return reader.ReadGuid(name);
        }

        public override void WriteValue(string name, Guid value, DataWriter writer)
        {
            writer.WriteGuid(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class SByteSerializer : Serializer<sbyte>
    {
        public override sbyte ReadValue(string name, DataReader reader)
        {
            return reader.ReadSbyte(name);
        }

        public override void WriteValue(string name, sbyte value, DataWriter writer)
        {
            writer.WriteSbyte(name, value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class IntPtrSerializer : Serializer<IntPtr>
    {
        public override IntPtr ReadValue(string name, DataReader reader)
        {
            long l = reader.ReadInt64(name);
            return new IntPtr(l);
        }

        public override void WriteValue(string name, IntPtr value, DataWriter writer)
        {
            writer.WriteInt64(name, (long)value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class UIntPtrSerializer : Serializer<UIntPtr>
    {
        public override UIntPtr ReadValue(string name, DataReader reader)
        {
            ulong l = reader.ReadUInt64(name);
            return new UIntPtr(l);
        }

        public override void WriteValue(string name, UIntPtr value, DataWriter writer)
        {
            writer.WriteUInt64(name, (ulong)value);
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]

    public class DateTimeSerializer : Serializer<DateTime>
    {
        public override DateTime ReadValue(string name, DataReader reader)
        {
            long l = reader.ReadInt64(name);
            return new DateTime(l);
        }

        public override void WriteValue(string name, DateTime value, DataWriter writer)
        {
            writer.WriteInt64(name, value.Ticks);
        }
    }




    public class EnumSerializer<T> : Serializer<T>
    {
        public EnumSerializer()
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("The Type is Not Enum " + typeof(T));
            }
        }
        public override T ReadValue(string name, DataReader reader)
        {
            ulong ul = reader.ReadUInt64(name);
            return (T)Enum.ToObject(typeof(T), ul);
        }
        public override void WriteValue(string name, T value, DataWriter writer)
        {
            Console.WriteLine(value);
            ulong ul;
            try
            {
                ul = Convert.ToUInt64(value as Enum);
            }
            catch (OverflowException)
            {
                unchecked
                {
                    ul = (ulong)Convert.ToInt64(value as Enum);
                }
            }
            ul = Convert.ToUInt64(value as Enum);
            writer.WriteUInt64(name, ul);
        }
    }
    public class ComplexTypeSerializer<T> : Serializer<T>
    {
        private readonly Type _type = typeof(T);
        private readonly bool _isAbstract = typeof(T).IsAbstract || typeof(T).IsInterface;
        private readonly bool _IsValueType = typeof(T).IsValueType;

        public override T ReadValue(string name, DataReader reader)
        {
            PackectContext context;
            reader.BeginComplexType(name, typeof(T), out context);

            if (context.valueType == NodeValueType.NULL)
            {
                reader.EndComplexType();
                return default(T);
            }
            else
            {
                Type realType = Framework.Assembly.GetType(context.realType);
                T t = CreateDefault(realType);

                object obj = t;
                if (reader.refContext.SubscribeGUID(context.guid, ref obj))
                {
                    var members = GetSerializeMembers(realType);
                    for (int i = 0; i < members.Count; i++)
                    {
                        var member = members[i];

                        if (member is PropertyInfo)
                        {
                            PropertyInfo pi = member as PropertyInfo;
                            string pn = pi.Name;
                            Type pt = pi.PropertyType;
                            var innerSer = Get(pt);
                            var piobj = innerSer.ReadValueWeak(pn, reader);
                            if (piobj != null)
                                pi.SetValue(t, piobj, null);
                        }
                        if (member is FieldInfo)
                        {
                            FieldInfo fi = member as FieldInfo;
                            string fn = fi.Name;
                            Type ft = fi.FieldType;
                            var innerSer = Get(ft);
                            var fiobj = innerSer.ReadValueWeak(fn, reader);

                            if (fiobj != null)
                                fi.SetValue(t, fiobj);
                        }
                    }
                }
                else
                {
                    reader.refContext.SetResolveObject(context.guid, t);
                }
                reader.EndComplexType();
                return t;

            }

           
        }

        public override void WriteValue(string name, T value, DataWriter writer)
        {

            if (value == null)
            {
                writer.BeginComplexType(name, _type, _type, Guid.NewGuid());
                writer.WriteNUll(name);
                writer.EndComplexType();
            }
            else
            {
                Guid guid;
                bool bo = writer.refContext.SubscribeObjeect(value, out guid);
                Type realType = value.GetType();
                writer.BeginComplexType(name, _type, realType, guid);
                if (bo)
                {
                    var members =GetSerializeMembers(realType);
                    for (int i = 0; i < members.Count; i++)
                    {
                        var member = members[i];

                        if (member is PropertyInfo)
                        {
                            PropertyInfo pi = member as PropertyInfo;
                            string pn = pi.Name;
                            Type pt = pi.PropertyType;
                            var piobj = pi.GetValue(value, null);
                            var innerSer = Get(pt);
                            innerSer.WriteValueWeak(pn, piobj, writer);
                        }
                        if (member is FieldInfo)
                        {
                            FieldInfo fi = member as FieldInfo;
                            string fn = fi.Name;
                            Type ft = fi.FieldType;
                            var fiobj = fi.GetValue(value);
                            var innerSer = Get(ft);
                            Console.WriteLine(innerSer+"             "+ft);
                            innerSer.WriteValueWeak(fn, fiobj, writer);
                        }
                    }
                }
                 writer.EndComplexType();
            }
        }

        private T CreateDefault(Type realType)
        {
            return (T)Activator.CreateInstance(realType);
        }

        private static List<MemberInfo> GetSerializeMembers(Type type)
        {
            var list = type.GetMembers().ToList();
            list.RemoveAll((member) => {
                if (member is PropertyInfo)
                {
                    PropertyInfo pi = member as PropertyInfo;
                    if (!pi.CanWrite)
                        return true;
                    return false;
                }
                else if (member is FieldInfo)
                {
                    FieldInfo fi = member as FieldInfo;
                    if (fi.IsStatic)
                        return true;
                    return false;
                }
                return true;
            });
            return list;
        }

    }
    public class ArraySerializer<T> : Serializer<T[]>
    {
       
        public override T[] ReadValue(string name, DataReader reader)
        {
            PackectContext context;
            reader.BeginComplexType(name, typeof(T), out context);

            if (context.valueType == NodeValueType.NULL)
            {
                reader.EndComplexType();
                return default(T[]);
            }
            else
            {
                T[] ts = new T[context.innerCount];
                object obj = ts;
                if (reader.refContext.SubscribeGUID(context.guid, ref obj))
                {
                    for (int i = 0; i < ts.Length; i++)
                    {
                        var innerSer = Get(typeof(T));
                        var piobj = innerSer.ReadValueWeak(i.ToString(), reader);
                        ts[i] = (T)piobj;
                    }
                }
                else
                {
                    reader.refContext.SetResolveObject(context.guid, ts);
                }
                reader.EndComplexType();

                return ts;
            }
        }

        public override void WriteValue(string name, T[] value, DataWriter writer)
        {
            if (value == null)
            {
                writer.BeginComplexType(name, typeof(T), typeof(T), Guid.NewGuid());
                writer.WriteNUll(name);
                writer.EndComplexType();
            }
            else
            {
                Guid guid;
                bool bo = writer.refContext.SubscribeObjeect(value, out guid);
                writer.BeginComplexType(name, typeof(T), typeof(T), guid);
                if (bo)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        int index = i;
                        T _item = value[index];
                        var innerSer = Get(typeof(T));

                        innerSer.WriteValueWeak(index.ToString(), _item, writer);
                    }
                }
                writer.EndComplexType();
            }
        }
    }
    public class ListSerializer<T> : Serializer<List<T>>
    {

        public override List<T> ReadValue(string name, DataReader reader)
        {
            PackectContext context;
            reader.BeginComplexType(name, typeof(T), out context);

            if (context.valueType == NodeValueType.NULL)
            {
                reader.EndComplexType();
                return default(List<T>);
            }
            else
            {
                List<T> ts = new List<T>();
                object obj = ts;
                if (reader.refContext.SubscribeGUID(context.guid, ref obj))
                {
                    for (int i = 0; i < ts.Count; i++)
                    {
                        var innerSer = Get(typeof(T));
                        var piobj = innerSer.ReadValueWeak(i.ToString(), reader);
                        ts[i] = (T)piobj;
                    }
                }
                else
                {
                    reader.refContext.SetResolveObject(context.guid, ts);
                }
                reader.EndComplexType();

                return ts;
            }
        }

        public override void WriteValue(string name, List<T> value, DataWriter writer)
        {
            if (value == null)
            {
                writer.BeginComplexType(name, typeof(T), typeof(T), Guid.NewGuid());
                writer.WriteNUll(name);
                writer.EndComplexType();
            }
            else
            {
                Guid guid;
                bool bo = writer.refContext.SubscribeObjeect(value, out guid);
                writer.BeginComplexType(name, typeof(T), typeof(T), guid);
                if (bo)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        int index = i;
                        T _item = value[index];
                        var innerSer = Get(typeof(T));

                        innerSer.WriteValueWeak(index.ToString(), _item, writer);
                    }
                }
                writer.EndComplexType();
            }
        }
    }




#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
