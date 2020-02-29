namespace IFramework.Modules.NodeAction
{
    [FrameworkVersion(3)]
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class SequenceNode : ContainerNode
    {
        private int _curIndex;
        public ActionNode curAction
        {
            get
            {
                return nodeList[_curIndex];
            }
        }

        public SequenceNode() : base() { }
        protected override bool OnMoveNext()
        {
            if (_curIndex >= count) return false;
            bool bo = curAction.MoveNext();
            if (bo) return true;
            _curIndex++;
            if (_curIndex >= count) return false;
            return true;
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
        //    Log.L(_curIndex + "_ " + count + "_" + isDone);

            _curIndex = 0;
        }

        protected override void OnNodeReset()
        {
            base.OnNodeReset();
            _curIndex = 0;
        }
        protected override void OnBegin()
        {
          //  Log.L(_curIndex+" "+count+""+isDone);
        }
        protected override void OnCompelete() { }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
