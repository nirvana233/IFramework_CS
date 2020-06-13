namespace IFramework.Serialization.Simple
{
    static class SerializerUtility
    {
        public static byte[] Serialize<T>(T t, string name)
        {
            DataWriter dw = new DataWriter();
            Serializer<T> ser = Serializer.Get<T>();
            ser.WriteValue(name, t, dw);
            return dw.Write();
        }
        public static T DeSerialize<T>(string name, byte[] buffer, int offset, int length)
        {
            DataReader dr = new DataReader(buffer, offset, length);
            Serializer<T> ser = Serializer.Get<T>();
            T t= ser.ReadValue(name, dr);
            dr.refContext.Resolve();
            return t;
        }
    }

}
