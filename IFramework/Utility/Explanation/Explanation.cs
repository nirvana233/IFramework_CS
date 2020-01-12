namespace IFramework.Utility
{
    public class Explanation<T, V>
    {
        private Explanation() { }
        public IExplainer<T, V> Explainer { get; set; }
        public static Explanation<T, V> CreateInstance()
        {
            return new Explanation<T, V>();
        }
        public Explanation<T, V> SetExplainer(IExplainer<T, V> Explainer)
        {
            this.Explainer = Explainer;
            return this;
        }
        public V Explain(T t, IEventArgs arg, params object[] param)
        {
            return Explainer.ExplainToV(t, arg, param);
        }
        public T Explain(V v, IEventArgs arg, params object[] param)
        {
            return Explainer.ExplainToT(v, arg, param);
        }
    }

}
