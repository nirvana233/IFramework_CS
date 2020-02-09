namespace IFramework.Modules.NodeAction
{
    public class RepeatNode : ActionNode
    {
        private int curRepeat;
        private int repeat;
        public ActionNode node;

        public void Config(int repeat, bool autoRecyle)
        {
            this.repeat = repeat;
            base.Config(autoRecyle);
        }

        protected override void OnBegin()
        {
            curRepeat = 0;

        }

        protected override void OnCompelete()
        {
        }

        protected override bool OnMoveNext()
        {
            if (repeat == -1)
            {
                if (node.MoveNext())
                {
                    node.NodeReset();
                }
                return true;
            }
            if (!node.MoveNext())
            {
                node.NodeReset();
                curRepeat++;
            }
            if (curRepeat == repeat)
                return false;
            return true;
        }
        protected override void OnDispose()
        {
            if (node != null)
            {
                node.Dispose();
                node = null;
            }
        }
        protected override void OnRecyle()
        {
            base.OnRecyle();
            node.Recyle();
            node = null;
        }
        protected override void OnDataReset()
        {
            repeat = -1;
            curRepeat = 0;
            node.ResetData();
        }

        public override void OnNodeReset()
        {
            curRepeat = 0;
            node.NodeReset();
        }
    }

}
