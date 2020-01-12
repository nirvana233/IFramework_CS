namespace IFramework.Moudles.NodeAction
{
    public class SequenceNode : ContainerNode
    {
        public static SequenceNode Allocate(bool autoDispose)
        {
            SequenceNode node = ActionNodePool<SequenceNode>.Get(autoDispose);
            return node;
        }
        private int curIndex;
        public IActionNode curAction
        {
            get
            {
                return childNodes[curIndex];
            }
        }
        public SequenceNode() : base()
        {
            curIndex = 0;
        }
        protected override void OnDispose()
        {
            base.OnDispose();
            curIndex = 0;
            ActionNodePool<SequenceNode>.Set(this);
        }
        protected override bool OnMoveNext()
        {
            if (curIndex >= NodesCount) return false;
            bool bo = curAction.MoveNext();
            if (bo) return true;
            curIndex++;
            if (curIndex >= NodesCount) return false;
            return true;
        }
        protected override void OnReset()
        {
            base.OnReset();
            curIndex = 0;
        }

        protected override void OnBegin()
        {
            curIndex = 0;
        }

        protected override void OnCompelete()
        {

        }
    }

}
