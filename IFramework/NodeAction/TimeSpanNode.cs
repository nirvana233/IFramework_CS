using System;

namespace IFramework.NodeAction
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
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

        protected override void OnNodeDispose() { }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
