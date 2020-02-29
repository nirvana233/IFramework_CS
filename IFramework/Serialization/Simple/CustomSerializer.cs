using System;

namespace IFramework.Serialization.Simple
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public abstract class CustomSerializer<T> : Serializer<T>
    {
        protected virtual T CreateDefault()
        {
            return Activator.CreateInstance<T>();
        }

      
    }
    public class Point2Serializer : CustomSerializer<Point2>
    {
        protected override Point2 CreateDefault()
        {
            return new Point2();
        }

        private readonly FloatSerializer fs = Get<float>() as FloatSerializer;

        public override Point2 ReadValue(string name, DataReader reader)
        {
            Point2 value = CreateDefault();
            PackectContext context;
            reader.BeginComplexType(name, typeof(Point2), out context);
            value.x = fs.ReadValue("X", reader);
            value.y = fs.ReadValue("Y", reader);
            reader.EndComplexType();
            return value;
        }

        public override void WriteValue(string name, Point2 value, DataWriter writer)
        {
            writer.BeginComplexType(name, typeof(Point2), typeof(Point2), Guid.NewGuid());
            fs.WriteValue("X", value.x, writer);
            fs.WriteValue("Y", value.y, writer);

            writer.EndComplexType();
        }
    }
    public class Point3Serializer : CustomSerializer<Point3>
    {
        protected override Point3 CreateDefault()
        {
            return new Point3();
        }

        private readonly FloatSerializer fs = Get<float>() as FloatSerializer;

        public override Point3 ReadValue(string name, DataReader reader)
        {
            Point3 value = CreateDefault();
            PackectContext context;
            reader.BeginComplexType(name, typeof(Point3), out context);
            value.x = fs.ReadValue("X", reader);
            value.y = fs.ReadValue("Y", reader);
            value.z = fs.ReadValue("Z", reader);
            reader.EndComplexType();
            return value;
        }

        public override void WriteValue(string name, Point3 value, DataWriter writer)
        {
            writer.BeginComplexType(name, typeof(Point3), typeof(Point3), Guid.NewGuid());
            fs.WriteValue("X", value.x, writer);
            fs.WriteValue("Y", value.y, writer);
            fs.WriteValue("Z", value.z, writer);
            writer.EndComplexType();
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
