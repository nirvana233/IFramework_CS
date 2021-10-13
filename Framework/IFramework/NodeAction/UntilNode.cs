using System;

namespace IFramework.NodeAction
{
    class UntilNode : ActionNode, IUntilNode
    {

        private Func<bool> _condition;


        internal void Config(Func<bool> condition, bool autoRecyle)
        {
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
            return !_condition.Invoke();
        }


        protected override void OnNodeReset() { }

    }



}
