using System.Collections.Generic;

namespace IFramework.Modules.NodeAction
{
    public abstract class ContainerNode : ActionNode
    {
        public int count
        {
            get
            {
                if (childNodes == null)
                    return -1;
                return childNodes.Count;
            }
        }
        protected List<ActionNode> childNodes=new List<ActionNode>();
        public void Append(ActionNode node)
        {
            if (childNodes == null)
                childNodes = new List<ActionNode>();
            childNodes.Add(node);
        }
        protected override void OnRecyle()
        {
            base.OnRecyle();
            for (int i = 0; i < childNodes.Count; i++)
                childNodes[i].Recyle();
            childNodes.Clear();
        }
        protected override void OnDispose()
        {
            for (int i = 0; i < childNodes.Count; i++)
                childNodes[i].Dispose();
            childNodes.Clear();
            childNodes = null;
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            for (int i = 0; i < childNodes.Count; i++)
                childNodes[i].ResetData();
        }
        public override void OnNodeReset()
        {
            for (int i = 0; i < childNodes.Count; i++)
                childNodes[i].NodeReset();
        }
       
    }

}
