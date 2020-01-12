using System;

namespace IFramework.Moudles.NodeAction
{
    public class EventNode : ActionNode
    {
        public static EventNode Allocate(Action func, bool autoDispose)
        {
            EventNode node = ActionNodePool<EventNode>.Get(autoDispose);
            node.func = func;
            return node;
        }

        private Action func;

        protected override void OnBegin()
        {

        }

        protected override void OnCompelete()
        {

        }

        protected override void OnDispose()
        {
            func = null;
            ActionNodePool<EventNode>.Set(this);
        }

        protected override bool OnMoveNext()
        {
            func.Invoke();
            return false;
        }

        protected override void OnReset()
        {

        }
    }

}
