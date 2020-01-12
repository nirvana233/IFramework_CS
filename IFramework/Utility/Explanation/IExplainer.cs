namespace IFramework.Utility
{
    public interface IExplainer<T, V>
    {
        V ExplainToV(T t, IEventArgs arg, params object[] param);
        T ExplainToT(V v, IEventArgs arg, params object[] param);
    }
}
