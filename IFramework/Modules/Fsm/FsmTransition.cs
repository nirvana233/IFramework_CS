using System;

namespace IFramework.Modules.Fsm
{
    public class FsmTransition
    {
        private Func<bool> conditions;
        public IFsmState Head { get; set; }
        public IFsmState Trail { get; set; }
        public void BindCondition(ITransitionCondition condtion)
        {
            if (condtion == null) throw new Exception("Func is" + condtion);
            conditions += condtion.IsMetCondition;
        }
        public void BindCondition(Func<bool> condtion)
        {
            if (condtion == null) throw new Exception("Func is" + condtion);
            conditions += condtion;
        }
        public void UnBindCondition(ITransitionCondition condtion)
        {
            if (condtion == null) throw new Exception("Func is" + condtion);
            conditions -= condtion.IsMetCondition;
        }
        public void UnBindCondition(Func<bool> condtion)
        {
            if (condtion == null) throw new Exception("Func is" + condtion);
            conditions -= condtion;
        }

        private bool RunConditions()
        {
            if (this.conditions == null) return true;
            bool finalReslt = true;
            conditions.GetInvocationList().ForEach((del) => {
                var result = (Func<bool>)del;
                finalReslt = finalReslt && result();
            });
            return finalReslt;
        }
        internal bool IsMetCondition { get { return RunConditions(); } }
        internal IFsmState GoToNextState()
        {
            Head.OnExit();
            if (Trail != null)
                Trail.OnEnter();
            return Trail;
        }
    }

}
