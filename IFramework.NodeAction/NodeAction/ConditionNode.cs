using System;

namespace IFramework.NodeAction
{
    class ConditionNode : ActionNode, IConditionNode
    {
        private Func<bool> _condition;
        private Action _callback;
        //public Func<bool> condition { get { return _condition; } }
        //public Action callback { get { return _callback; } }
        internal void Config(Func<bool> condition, Action callback, bool autoRecyle)
        {
            this._condition = condition;
            this._callback = callback;
            base.Config(autoRecyle);
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            _callback = null;
            _condition = null;
        }



        protected override bool OnMoveNext()
        {
            if (_condition.Invoke())
                _callback();
            return false;
        }

        protected override void OnNodeReset() { }
        protected override void OnBegin() { }
        protected override void OnCompelete() { }

        //protected override void OnNodeDispose()
        //{
        //    _callback = null;
        //    _condition = null;
        //}
    }

}
