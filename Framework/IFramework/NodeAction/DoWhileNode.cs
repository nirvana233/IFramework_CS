using System;

namespace IFramework.NodeAction
{
    class DoWhileNode :ActionNode, IDoWhileNode
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
            _body = null;
        }
        protected override bool OnMoveNext()
        {
            if (_body != null)
            {
                _body();
            }
            return _condition.Invoke();
        }

        protected override void OnNodeReset() { }

    }

}
