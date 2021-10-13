using System;

namespace IFramework.NodeAction
{
    class WhileNode : ActionNode, IWhileNode
    {

        private Func<bool> _condition;
        private Action _body;

        internal void Config(Func<bool> condition, Action body, bool autoRecyle)
        {
            this._body = body;
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
            if (bo && _body != null)
            {
                _body();
            }
            return bo;
        }

        protected override void OnNodeReset() { }

    }

}
