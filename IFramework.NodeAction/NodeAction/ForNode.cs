using System;

namespace IFramework.NodeAction
{
    class ForNode : ActionNode, IActionNode
    {
        private Func<bool> _condition;
        private Action _initializer;
        public Action _iterator;
        public Action _body;
        internal void Config(Action initializer, Func<bool> condition,Action iterator,Action body, bool autoRecyle)
        {
            if (condition==null)
            {
                Log.E("condition can not be null");
            }
            this._initializer = initializer;
            this._condition = condition;
            this._iterator = iterator;
            this._body = body;
            base.Config(autoRecyle);

        }
        protected override bool OnMoveNext()
        {
            bool bo = _condition();
            if (bo)
            {
                if (_body!=null)
                {
                    _body();
                }
                if (_iterator!=null)
                {
                    _iterator();
                }
            }
            return bo;
        }
        protected override void OnBegin()
        {
            if (_initializer!=null)
            {
                _initializer();
            }
        }
        protected override void OnNodeReset()
        {
           
        }
    }

}
