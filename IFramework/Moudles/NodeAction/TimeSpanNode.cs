using System;

namespace IFramework.Moudles.NodeAction
{
    public class TimeSpanNode : ActionNode
    {
        public static TimeSpanNode Allocate(TimeSpan timeSpan, bool autoDispose)
        {
            TimeSpanNode node = ActionNodePool<TimeSpanNode>.Get(autoDispose);
            node.timeSpan = timeSpan;
            return node;
        }
        private TimeSpan timeSpan;
        private DateTime setTime;

        protected override void OnBegin()
        {
            setTime = DateTime.Now;
        }

        protected override bool OnMoveNext()
        {
            return DateTime.Now - setTime < timeSpan;
        }

        protected override void OnCompelete() { }

        protected override void OnDispose()
        {
            ActionNodePool<TimeSpanNode>.Set(this);
        }
        protected override void OnReset() { }
    }

}
