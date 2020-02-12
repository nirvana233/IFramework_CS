namespace IFramework.Modules.NodeAction
{
    [FrameworkVersion(3)]

    public class SequenceNode : ContainerNode
    {
        private int curIndex;
        public ActionNode curAction
        {
            get
            {
                return nodeList[curIndex];
            }
        }
     
        public SequenceNode():base()
        { }
        protected override bool OnMoveNext()
        {
            if (curIndex >= count) return false;
            bool bo = curAction.MoveNext();
            if (bo) return true;
            curIndex++;
            if (curIndex >= count) return false;
            return true;
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            curIndex = 0;
        }

        protected override void OnBegin()
        {
            curIndex = 0;
        }
        public override void OnNodeReset()
        {
            base.OnNodeReset();
            curIndex = 0;
        }
        protected override void OnCompelete()
        {

        }
    }

}
