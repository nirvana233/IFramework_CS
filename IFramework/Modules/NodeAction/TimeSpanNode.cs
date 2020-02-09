using System;

namespace IFramework.Modules.NodeAction
{
    public class TimeSpanNode : ActionNode
    {
        public void Config(TimeSpan timeSpan, bool autoRecyle)
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
            return DateTime.Now - setTime < timeSpan;
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            setTime = DateTime.Now;
            timeSpan = TimeSpan.Zero;
        }
        protected override void OnCompelete() { }

        protected override void OnDispose()
        {
            
        }

        public override void OnNodeReset()
        {
            setTime = DateTime.Now;
        }
    }

}
