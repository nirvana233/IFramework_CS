using System;

namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public interface ISingleton : IDisposable
    {
        void OnSingletonInit();
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
