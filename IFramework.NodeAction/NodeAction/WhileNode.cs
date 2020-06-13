using System;

namespace IFramework.NodeAction
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class WhileNode : ActionNode
    {

        private Func<bool> _condition;
        public Func<bool> condition { get { return _condition; } }
        private Action _loop;
        public Action loop { get { return _loop; } }

        public void Config(Func<bool> condition,Action loop, bool autoRecyle)
        {
            this._loop = loop;
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
            bool bo = condition.Invoke();
            if (bo && _loop!=null)
            {
                _loop();
            }
            return bo;
        }

        protected override void OnBegin() { }
        protected override void OnCompelete() { }
        protected override void OnNodeReset() { }

        protected override void OnNodeDispose() { }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
