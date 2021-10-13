namespace IFramework.Queue
{
    /// <summary>
    /// 优先级队列节点
    /// </summary>
    public class FastPriorityQueueNode
    {
        /// <summary>
        /// The Priority to insert this node at.
        /// Cannot be manually edited - see queue.Enqueue() and queue.UpdatePriority() instead
        /// </summary>
        public float priority { get; protected internal set; }

        /// <summary>
        /// Represents the current position in the queue
        /// </summary>
        public int position { get; internal set; }
    }

}
