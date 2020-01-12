using System;

namespace IFramework.Moudles.NodeAction
{
    public class UntilNode : ActionNode
    {
        public static UntilNode Allocate(Func<bool> func, bool autoDispose)
        {
            UntilNode node = ActionNodePool<UntilNode>.Get(autoDispose);
            node.func = func;
            return node;
        }

        private Func<bool> func;

        protected override void OnBegin()
        {

        }

        protected override void OnCompelete()
        {
        }

        protected override void OnDispose()
        {
            func = null;
            ActionNodePool<UntilNode>.Set(this);
        }

        protected override bool OnMoveNext()
        {
            return !func.Invoke();
        }

        protected override void OnReset()
        {

        }
    }

}
