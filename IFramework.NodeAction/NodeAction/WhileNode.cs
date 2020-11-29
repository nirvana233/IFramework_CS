using System;

namespace IFramework.NodeAction
{
    class WhileNode : ActionNode, IWhileNode
    {

        private Func<bool> _condition;
        //public Func<bool> condition { get { return _condition; } }
        private Action _loop;
        //public Action loop { get { return _loop; } }

        internal void Config(Func<bool> condition, Action loop, bool autoRecyle)
        {
            this._loop = loop;
            this._condition = condition;
            base.Config(autoRecyle);
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            _condition = null;
        }
        protected override bool OnMoveNext()
        {
            bool bo = _condition.Invoke();
            if (bo && _loop != null)
            {
                _loop();
            }
            return bo;
        }

        protected override void OnBegin() { }
        protected override void OnCompelete() { }
        protected override void OnNodeReset() { }

        //protected override void OnNodeDispose() { }
    }

}
