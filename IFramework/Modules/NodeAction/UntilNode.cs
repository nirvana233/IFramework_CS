using System;

namespace IFramework.Modules.NodeAction
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class UntilNode : ActionNode
    {

        private Func<bool> _condition;
        public Func<bool> condition { get { return _condition; }  }


        public void Config(Func<bool> condition, bool autoRecyle)
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
            return !condition.Invoke();
        }

        protected override void OnBegin() { }
        protected override void OnCompelete() { }
        protected override void OnDispose() { }
        protected override void OnNodeReset() { }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
