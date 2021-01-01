namespace IFramework.NodeAction
{
    /// <summary>
    /// 容器节点
    /// </summary>
    public interface IContainerNode : IActionNode
    {
        /// <summary>
        /// 最后一个节点
        /// </summary>
        IActionNode last { get; }
    }
}