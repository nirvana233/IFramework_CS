using System.Collections.Generic;

namespace IFramework.Moudles.NodeAction
{
    public abstract class ContainerNode : ActionNode
    {
        protected ContainerNode()
        {
            childNodes = new List<IActionNode>();
        }
        public int NodesCount { get { return childNodes.Count; } }
        protected List<IActionNode> childNodes;
        public void Append(IActionNode node)
        {
            childNodes.Add(node);
        }
        protected override void OnDispose()
        {
            for (int i = 0; i < childNodes.Count; i++)
                childNodes[i].Dispose();
            childNodes.Clear();
            childNodes = null;
        }
        protected override void OnReset()
        {
            for (int i = 0; i < childNodes.Count; i++)
                childNodes[i].Reset();
        }
    }

}
