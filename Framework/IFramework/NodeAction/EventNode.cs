using System;

namespace IFramework.NodeAction
{
    class EventNode : ActionNode, IEventNode
    {
        private Action _body;

        internal void Config(Action body, bool autoRecyle)
        {
            this._body = body;
            base.Config(autoRecyle);
        }



        protected override void OnDataReset()
        {
            base.OnDataReset();
            _body = null;
        }

        protected override bool OnMoveNext()
        {
            _body.Invoke();
            return false;
        }

        protected override void OnNodeReset() { }

    }

}
