namespace IFramework.Modules.NodeAction
{
    [FrameworkVersion(3)]
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
            _curIndex = 0;
        }

        protected override void OnNodeReset()
        {
            base.OnNodeReset();
            _curIndex = 0;
        }
        protected override void OnBegin() { }
        protected override void OnCompelete() { }
    }

}
