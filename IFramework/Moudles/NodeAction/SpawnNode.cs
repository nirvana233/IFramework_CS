namespace IFramework.Moudles.NodeAction
{
    public class SpawnNode : ContainerNode
    {
        public static SpawnNode Allocate(bool autoDispose)
        {
            SpawnNode node = ActionNodePool<SpawnNode>.Get(autoDispose);
            return node;
        }

        private int mFinishCount;

        protected override void OnDispose()
        {
            base.OnDispose();
            ActionNodePool<SpawnNode>.Set(this);
        }
        protected override void OnReset()
        {
            base.OnReset();
            mFinishCount = 0;
        }
        protected override void OnBegin()
        {
            mFinishCount = 0;

        }

        protected override void OnCompelete()
        {
        }

        protected override bool OnMoveNext()
        {
            if (mFinishCount >= NodesCount) return false;
            for (var i = NodesCount - 1; i >= 0; i--)
            {
                var node = childNodes[i];
                if (!node.MoveNext())
                    mFinishCount++;
            }
            return mFinishCount < NodesCount;
        }
    }

}
