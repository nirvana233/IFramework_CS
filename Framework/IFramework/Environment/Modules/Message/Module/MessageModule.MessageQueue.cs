using System;
using System.Collections.Generic;
using IFramework.Queue;
namespace IFramework.Modules.Message
{
    public partial class MessageModule
    {
        private partial class MessageQueue : IDisposable
        {
            public int count
            {
                get
                {
                    using (new LockWait(ref _lock_message))
                    {
                        return _priorityQueue.count;
                    }
                }
            }

            private readonly MessageModule module;
            private int _processesPerFrame = -1;
            private LockParam _lock_message = new LockParam();


            private StablePriorityQueue<StablePriorityQueueNode> _priorityQueue;
            private List<StablePriorityQueueNode> _updatelist;
            private Dictionary<StablePriorityQueueNode, Message> excuteMap;
            public int processesPerFrame { get { return _processesPerFrame; } set { _processesPerFrame = value; } }

            public MessageQueue(MessageModule module)
            {
                excuteMap = Framework.GlobalAllocate<Dictionary<StablePriorityQueueNode, Message>>();
                int count = Framework.GetGlbalPoolCount<StablePriorityQueue<StablePriorityQueueNode>>();
                _priorityQueue = count == 0 ? new StablePriorityQueue<StablePriorityQueueNode>() : Framework.GlobalAllocate<StablePriorityQueue<StablePriorityQueueNode>>();
                _updatelist = Framework.GlobalAllocate<List<StablePriorityQueueNode>>();
                this.module = module;
            }
            public IMessage PublishByNumber(Type type, IEventArgs args, int priority = MessageUrgency.Common)
            {
                var message = Framework.GlobalAllocate<Message>();
                message.Begin();
                message.SetArgs(args).SetType(type);
                if (priority < 0)
                {
                    HandleMessage(message);
                }
                else
                {
                    using (new LockWait(ref _lock_message))
                    {
                        if (_priorityQueue.count == _priorityQueue.capcity)
                        {
                            _priorityQueue.Resize(_priorityQueue.capcity * 2);
                        }
                        StablePriorityQueueNode node = Framework.GlobalAllocate<StablePriorityQueueNode>();
                        _priorityQueue.Enqueue(node, priority);
                        excuteMap.Add(node, message);
                        _updatelist.Add(node);
                    }
                }
                return message;
            }




            private void HandleMessage(Message message)
            {
                message.Lock();
                bool sucess = false;
                sucess |= module.handlers.Publish(message);
                message.SetErrorCode(sucess ? MessageErrorCode.Success : MessageErrorCode.NoneListen);
                message.End();
                message.GlobalRecyle();
            }
            public void Update()
            {
                int count = 0;
                Queue<Message> _tmp = Framework.GlobalAllocate<Queue<Message>>();
                using (new LockWait(ref _lock_message))
                {
                    count = processesPerFrame == -1 ? _priorityQueue.count : Math.Min(processesPerFrame, _priorityQueue.count);
                    if (count == 0) return;
                    for (int i = 0; i < count; i++)
                    {
                        StablePriorityQueueNode node = _priorityQueue.Dequeue();
                        Message message;
                        if (excuteMap.TryGetValue(node, out message))
                        {
                            _tmp.Enqueue(message);
                            excuteMap.Remove(node);
                        }
                        _updatelist.Remove(node);
                        node.GlobalRecyle();
                    }
                    if (_updatelist.Count > 0)
                    {
                        for (int i = _updatelist.Count - 1; i >= 0; i--)
                        {
                            _priorityQueue.UpdatePriority(_updatelist[i], _updatelist[i].priority - 1);
                        }
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    var message = _tmp.Dequeue();
                    HandleMessage(message);
                }
                _tmp.GlobalRecyle();
            }


            public void Dispose()
            {
                foreach (var item in excuteMap.Values) item.GlobalRecyle();
                while (_priorityQueue.count != 0) _priorityQueue.Dequeue().GlobalRecyle();
                _updatelist.Clear();
                excuteMap.Clear();
                _priorityQueue.GlobalRecyle();
                _updatelist.GlobalRecyle();
                excuteMap.GlobalRecyle();
            }
        }
    }
}
