namespace IFramework.Modules.NodeAction
{
    public class SequenceNode : ContainerNode
    {
        private int curIndex;
        public IActionNode curAction
        {
            get
            {
                return childNodes[curIndex];
            }
        }
     

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
