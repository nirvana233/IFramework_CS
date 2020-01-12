namespace IFramework.Moudles.NodeAction
{
    public class ActionNodePool<T> : SingletonPool<T, ActionNodePool<T>> where T : IActionNode, new()
    {
        private ActionNodePool() { }
        public static T Get(bool autoDispose)
        {
            T node = Instance.Get();
            node.AutoDispose = autoDispose;
            return node;
        }
        public static void Set(T t)
        {
            Instance.Set(t, null);
        }
        protected override T CreatNew(IEventArgs arg, params object[] param)
        {
            return new T();
        }
    }

}
