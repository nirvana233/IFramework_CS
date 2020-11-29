using System;

namespace IFramework.NodeAction
{
    [ScriptVersion(3)]
    class EventNode : ActionNode, IEventNode
    {
        private Action _callback;
        //public Action callback { get { return _callback; } }

        internal void Config(Action callback, bool autoRecyle)
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
        protected override void OnNodeReset() { }

        //protected override void OnNodeDispose() { }
    }

}
