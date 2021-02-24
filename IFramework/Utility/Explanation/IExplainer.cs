namespace IFramework.Utility
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public interface IExplainer<Type1, Type2>
    {
        Type2 Explain(Type1 type1, IEventArgs arg, params object[] param);
        Type1 Explain(Type2 type2, IEventArgs arg, params object[] param);
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
