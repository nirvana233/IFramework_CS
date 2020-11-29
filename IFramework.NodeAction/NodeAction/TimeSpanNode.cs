using System;

namespace IFramework.NodeAction
{
    class TimeSpanNode : ActionNode, ITimeSpanNode
    {
        internal void Config(TimeSpan timeSpan, bool autoRecyle)
        {
            base.Config(autoRecyle);
            this.timeSpan = timeSpan;
        }
        private TimeSpan timeSpan;
        private DateTime setTime;

        protected override void OnBegin()
        {
            setTime = DateTime.Now;
        }

        protected override bool OnMoveNext()
        {
            return (DateTime.Now - setTime).TotalMilliseconds < timeSpan.TotalMilliseconds;
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            setTime = DateTime.Now;
            timeSpan = TimeSpan.Zero;
        }

        protected override void OnNodeReset()
        {
            setTime = DateTime.Now;
        }

        protected override void OnCompelete() { }

        //protected override void OnNodeDispose() { }
    }

}
