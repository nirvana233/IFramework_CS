using System;

namespace IFramework.NodeAction
{
    class TimeSpanNode : ActionNode, ITimeSpanNode
    {
        internal void Config(TimeSpan time, bool autoRecyle)
        {
            base.Config(autoRecyle);
            this._time = time;
        }
        private TimeSpan _time;
        private DateTime setTime;

        protected override void OnBegin()
        {
            setTime = DateTime.Now;
        }

        protected override bool OnMoveNext()
        {
            return (DateTime.Now - setTime).TotalMilliseconds < _time.TotalMilliseconds;
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            setTime = DateTime.Now;
            _time = TimeSpan.Zero;
        }

        protected override void OnNodeReset()
        {
            setTime = DateTime.Now;
        }
    }

}
