namespace IFramework.Moudles.NodeAction
{
    public class RepeatNode : ActionNode
    {
        private int curRepeat;
        private int repeat;
        public IActionNode node;

        public static RepeatNode Allocate(int repeat, bool autoDispose)
        {
            RepeatNode node = ActionNodePool<RepeatNode>.Get(autoDispose);
            node.repeat = repeat;
            return node;
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
                    node.Reset();
                }
                return true;
            }
            if (!node.MoveNext())
            {
                node.Reset();
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
            ActionNodePool<RepeatNode>.Set(this);
        }
        protected override void OnReset()
        {
            curRepeat = 0;
            node.Reset();
        }
    }

}
