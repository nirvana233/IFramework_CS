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
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
