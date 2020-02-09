using System;

namespace IFramework.Modules.NodeAction
{
    public class UntilNode : ActionNode
    {
        public void Config(Func<bool> func, bool autoRecyle)
        {
            this.func = func;
            base.Config(autoRecyle);
        }

        private Func<bool> func;

        protected override void OnBegin()
        {

        }

        protected override void OnCompelete()
        {
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            func = null;
        }
        protected override bool OnMoveNext()
        {
            return !func.Invoke();
        }

        protected override void OnDispose()
        {
            
        }

        public override void OnNodeReset()
        {
            
        }
    }

}
