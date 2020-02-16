using System;

namespace IFramework.Modules.Fsm
{
    /// <summary>
    /// 状态机过度线
    /// </summary>
    public class FsmTransition
    {
        private Func<bool> conditions;
        /// <summary>
        /// 头状态
        /// </summary>
        public IFsmState head { get; set; }
        /// <summary>
        /// 指向状态
        /// </summary>
        public IFsmState trail { get; set; }
        /// <summary>
        /// 绑定条件
        /// </summary>
        /// <param name="condtion"></param>
        public void BindCondition(ITransitionCondition condtion)
        {
            if (condtion == null) throw new Exception("Func is" + condtion);
            conditions += condtion.IsMetCondition;
        }
        /// <summary>
        /// 绑定条件
        /// </summary>
        /// <param name="condtion"></param>
        public void BindCondition(Func<bool> condtion)
        {
            if (condtion == null) throw new Exception("Func is" + condtion);
            conditions += condtion;
        }
        /// <summary>
        /// 解绑条件
        /// </summary>
        /// <param name="condtion"></param>
        public void UnBindCondition(ITransitionCondition condtion)
        {
            if (condtion == null) throw new Exception("Func is" + condtion);
            conditions -= condtion.IsMetCondition;
        }
        /// <summary>
        ///  解绑条件
        /// </summary>
        /// <param name="condtion"></param>
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
            head.OnExit();
            if (trail != null)
                trail.OnEnter();
            return trail;
        }
    }

}
