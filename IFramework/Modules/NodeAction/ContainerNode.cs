using System.Collections.Generic;

namespace IFramework.Modules.NodeAction
{
    [FrameworkVersion(3)]
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public abstract class ContainerNode : ActionNode
    {
        public int count
        {
            get
            {
                if (nodeList == null)
                    return -1;
                return nodeList.Count;
            }
        }
        protected ContainerNode()
        {
            nodeList = new List<ActionNode>();
        }
        protected List<ActionNode> nodeList;
        public virtual void Append(ActionNode node)
        {
            nodeList.Add(node);
        }
        protected override void OnRecyle()
        {
            base.OnRecyle();
            for (int i = 0; i < nodeList.Count; i++)
                nodeList[i].Recyle();
            nodeList.Clear();
        }
        protected override void OnDispose()
        {
            for (int i = 0; i < nodeList.Count; i++)
                nodeList[i].Dispose();
            nodeList.Clear();
            nodeList = null;
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            for (int i = 0; i < nodeList.Count; i++)
                nodeList[i].ResetData();
        }
        protected override void OnNodeReset()
        {
            for (int i = 0; i < nodeList.Count; i++)
                nodeList[i].NodeReset();
        }
       
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
