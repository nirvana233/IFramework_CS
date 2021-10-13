using System.Collections.Generic;

namespace IFramework.NodeAction
{
    /// <summary>
    /// 容器节点
    /// </summary>
    [ScriptVersion(3)]
     abstract class ContainerNode : ActionNode, IContainerNode
    {
        /// <summary>
        /// 子级节点个数
        /// </summary>
        protected int count
        {
            get
            {
                if (nodeList == null)
                    return -1;
                return nodeList.Count;
            }
        }
        /// <summary>
        /// 子节点最后一个
        /// </summary>
        public IActionNode last { get { return nodeList[nodeList.Count - 1]; } }
        /// <summary>
        /// 所有子节点
        /// </summary>
        protected List<ActionNode> nodeList = new List<ActionNode>();
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="node"></param>
        public virtual void Append(ActionNode node)
        {
            node.depth = this.depth + 1;
            node.parent = this;
            nodeList.Add(node);
            SetDataDirty();
        }
        /// <summary>
        /// 回收时
        /// </summary>
        protected override void OnRecyle()
        {
            base.OnRecyle();
            for (int i = 0; i < nodeList.Count; i++)
                nodeList[i].Recyle();
            nodeList.Clear();
        }
        /// <summary>
        /// 调用时机：repeat 节点完成一次
        /// </summary>
        protected override void OnNodeReset()
        {
            for (int i = 0; i < nodeList.Count; i++)
                nodeList[i].NodeReset();
        }

    }

}
