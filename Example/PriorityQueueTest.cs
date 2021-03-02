using IFramework;
using IFramework.Queue;

namespace Example
{
    public class PriorityQueueTest : Test
    {
        public class Node:FastPriorityQueueNode
        {
            public int index;
        }
        protected override void Start()
        {
            FastPriorityQueue<Node> nodes = new FastPriorityQueue<Node>(100);
            Log.L("add Nodes");

            for (int i = 10; i >= 0; i--)
            {

                nodes.Enqueue(new Node() { index = i }, i);
            }
            Log.L("Get Nodes");

            while (nodes.count!=0)
            {
            Log.L(nodes.Dequeue().index);

            }
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
