using System;

namespace IFramework.NodeAction
{
    class ConditionNode : ActionNode, IConditionNode
    {
        private Func<bool> _condition;
        private Action _body;

        internal void Config(Func<bool> condition, Action boday, bool autoRecyle)
        {
            this._condition = condition;
            this._body = boday;
            base.Config(autoRecyle);
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            _body = null;
            _condition = null;
        }



        protected override bool OnMoveNext()
        {
            if (_condition.Invoke())
                _body();
            return false;
        }

        protected override void OnNodeReset() { }

    }

}
