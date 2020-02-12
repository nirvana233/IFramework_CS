using System.Collections.Generic;

namespace IFramework.Modules.NodeAction
{
    [FrameworkVersion(3)]
    public class RepeatNode : ContainerNode
    {
        private int curRepeat;
        private int repeat;
        public ActionNode node { get { return nodeList[0]; }set { nodeList[0] = value; } }
        public RepeatNode() : base()
        {
           
        }
        public void Config(int repeat, bool autoRecyle)
        {
            this.repeat = repeat;
            base.Config(autoRecyle);
        }
        public override void Append(ActionNode node)
        {
            if (nodeList.Count >= 1)
                Log.E("RepeatNode Can Have One Inner Node");
            else
                nodeList.Add(node);
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
               // Log.E(node.GetType());
                if (!node.MoveNext())
                    node.NodeReset();
                return true;
            }
            if (!node.MoveNext())
            {
                node.NodeReset();
                curRepeat++;
            }
            if (curRepeat >= repeat)
                return false;
            return true;
        }
        

        protected override void OnDataReset()
        {
            base.OnDataReset();
            repeat = -1;
            curRepeat = -1;
        }

        public override void OnNodeReset()
        {
            base.OnNodeReset();
            curRepeat = -1;
        }
    }

}
