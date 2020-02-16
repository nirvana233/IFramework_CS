using System;

namespace IFramework.Modules.NodeAction
{
    [FrameworkVersion(3)]
    public class EventNode : ActionNode
    {
        private Action _callback;
        public Action callback { get { return _callback; } }

        public void Config(Action callback, bool autoRecyle)
        {
            this._callback = callback;
            base.Config(autoRecyle);
        }
      


        protected override void OnDataReset()
        {
            base.OnDataReset();
            _callback = null;
        }

        protected override bool OnMoveNext()
        {
            _callback.Invoke();
            return false;
        }

        protected override void OnBegin() { }
        protected override void OnCompelete() { }
        protected override void OnDispose() { }
        protected override void OnNodeReset() { }
    }

}
