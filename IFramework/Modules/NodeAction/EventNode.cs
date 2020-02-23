using System;

namespace IFramework.Modules.NodeAction
{
    [FrameworkVersion(3)]
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class EventNode : ActionNode
    {
        private Action _callback;
        public Action callback { get { return _callback; } }

        public void Config(Action callback, bool autoRecyle)
        {
            this._callback = callback;
            base.Config(autoRecyle);
        }
      


        protected override void OnDataReset()
        {
            base.OnDataReset();
            _callback = null;
        }

        protected override bool OnMoveNext()
        {
            _callback.Invoke();
            return false;
        }

        protected override void OnBegin() { }
        protected override void OnCompelete() { }
        protected override void OnDispose() { }
        protected override void OnNodeReset() { }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
