namespace IFramework.NodeAction
{
    /// <summary>
    /// 重复节点
    /// </summary>
    public interface IRepeatNode:IContainerNode
    {
        /// <summary>
        /// 循环次数
        /// </summary>
        int repeat { get; set; }

    }
}