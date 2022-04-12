namespace IFramework.Queue
{
    /// <summary>
    /// 泛型优先级队列节点
    /// </summary>
    /// <typeparam name="TPriority"></typeparam>
    public class GenericPriorityQueueNode<TPriority>
    {
        /// <summary>
        /// The Priority to insert this node at.
        /// Cannot be manually edited - see queue.Enqueue() and queue.UpdatePriority() instead
        /// </summary>
        public TPriority priority { get; protected internal set; }

        /// <summary>
        /// Represents the current position in the queue
        /// </summary>
        public int position { get; internal set; }

        /// <summary>
        /// Represents the order the node was inserted in
        /// </summary>
        public long insertPosition { get; internal set; }

    }

}
