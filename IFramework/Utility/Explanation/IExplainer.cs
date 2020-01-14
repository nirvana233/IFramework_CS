namespace IFramework.Utility
{
    public interface IExplainer<Type1, Type2>
    {
        Type2 Explain(Type1 type1, IEventArgs arg, params object[] param);
        Type1 Explain(Type2 type2, IEventArgs arg, params object[] param);
    }
}
