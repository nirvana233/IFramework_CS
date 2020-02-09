namespace IFramework.Modules.NodeAction
{
    public class SpawnNode : ContainerNode
    {

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
                var node = childNodes[i];
                if (!node.MoveNext())
                    mFinishCount++;
            }
            return mFinishCount < count;
        }
    }

}
