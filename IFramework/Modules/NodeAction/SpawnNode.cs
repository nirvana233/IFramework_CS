namespace IFramework.Modules.NodeAction
{
    [FrameworkVersion(3)]
    public class SpawnNode : ContainerNode
    {
        public SpawnNode():base ()
        { }
        private int mFinishCount;

        protected override void OnDataReset()
        {
            base.OnDataReset();
            mFinishCount = 0;
        }
        public override void OnNodeReset()
        {
            base.OnNodeReset();
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
            if (mFinishCount >= count) return false;
            for (var i = count - 1; i >= 0; i--)
            {
                var node = nodeList[i];
                if (!node.MoveNext())
                    mFinishCount++;
            }
            return mFinishCount < count;
        }
    }

}
