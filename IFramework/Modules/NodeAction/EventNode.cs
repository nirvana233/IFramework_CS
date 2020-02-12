using System;

namespace IFramework.Modules.NodeAction
{
    [FrameworkVersion(3)]
    public class EventNode : ActionNode
    {
        public void Config(Action func, bool autoRecyle)
        {
            this.func = func;
            base.Config(autoRecyle);
        }
       
        private Action func;

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
            
            func.Invoke();
            return false;
        }

        protected override void OnDispose()
        {
            
        }

        public override void OnNodeReset()
        {
            
        }
    }

}
