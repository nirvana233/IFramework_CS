namespace IFramework.Queue
{
    /// <summary>
    /// 稳定优先级队列节点
    /// </summary>
    public class StablePriorityQueueNode : FastPriorityQueueNode
    {
        /// <summary>
        /// Represents the order the node was inserted in
        /// </summary>
        public long insertPosition { get; internal set; }
    }
}
