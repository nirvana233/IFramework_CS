namespace IFramework.Utility
{
    public class Explanation<Type1, Type2>
    {
        private Explanation() { }
        public IExplainer<Type1, Type2> Explainer { get; set; }
        public static Explanation<Type1, Type2> CreateInstance()
        {
            return new Explanation<Type1, Type2>();
        }
        public Explanation<Type1, Type2> SetExplainer(IExplainer<Type1, Type2> Explainer)
        {
            this.Explainer = Explainer;
            return this;
        }
        public Type2 Explain(Type1 type1, IEventArgs arg, params object[] param)
        {
            return Explainer.Explain(type1, arg, param);
        }
        public Type1 Explain(Type2 type2, IEventArgs arg, params object[] param)
        {
            return Explainer.Explain(type2, arg, param);
        }
    }

}
