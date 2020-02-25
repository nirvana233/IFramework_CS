namespace IFramework.Pool
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public interface IPoolObject
    {
        void OnCreate(IEventArgs arg);
        void OnGet(IEventArgs arg);
        void OnSet(IEventArgs arg);
        void OnClear(IEventArgs arg);
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
