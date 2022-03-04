using System;
using System.Collections.Generic;
using IFramework.Queue;
namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息模块
    /// </summary>
    [ScriptVersion(230)]
    [VersionUpdate(120, "加入消息优先级以及进程等待")]
    [VersionUpdate(140, "增加子类型匹配")]
    [VersionUpdate(180, "抽象出IMessage")]
    [VersionUpdate(200, "增加立刻处理")]
    [VersionUpdate(230, "注册延时处理")]
    public class MessageModule : UpdateModule, IMessageModule
    {
        private class MessageQueue : IDisposable
        {
            private class Message : IMessage
            {

                private int _code = 0;
                private Type _subject;
                private IEventArgs _args;
                private MessageState _state;
                private MessageErrorCode _errCode;
                private event Action<IMessage> onRecycle;

                public int code { get { return _code; } }
                public Type subject { get { return _subject; } }
                public IEventArgs args { get { return _args; } }
                public MessageState state { get { return _state; } }
                public MessageErrorCode errorCode { get { return _errCode; } }

                private void Reset()
                {
                    this._errCode = MessageErrorCode.None;
                    _code = int.MinValue;
                    _args = null;
                    _subject = null;
                    onRecycle = null;
                }
                internal void Begin()
                {
                    Reset();
                    _state = MessageState.Wait;
                }
                internal Message SetType(Type type)
                {
                    this._subject = type;
                    return this;
                }
                internal Message SetArgs(IEventArgs args)
                {
                    this._args = args;
                    return this;
                }
                public IMessage SetCode(int code)
                {
                    if (_state != MessageState.Wait)
                    {
                        Log.E(string.Format("you can not set the code now, the state is {0}", _state));
                        return this;
                    }
                    this._code = code;
                    return this;
                }

                public IMessage OnCompelete(Action<IMessage> action)
                {
                    if (_state != MessageState.Wait)
                    {
                        Log.E(string.Format("you can not bind the action now, the state is {0}", _state));
                        return this;
                    }
                    onRecycle += action;
                    return this;
                }
                internal void Lock()
                {
                    if (_state == MessageState.Wait)
                    {
                        _state = MessageState.Lock;
                    }
                    else
                    {
                        Log.E("unknown Exception occured with this message");
                    }
                }
                internal void SetErrorCode(MessageErrorCode code)
                {
                    if (_state == MessageState.Lock)
                    {
                        this._errCode = code;
                    }
                    else
                    {
                        Log.E("unknown Exception occured with this message");
                    }
                }
                internal void End()
                {
                    if (_state != MessageState.Lock)
                    {
                        Log.E("unknown Exception occured with this message");
                    }
                    _state = MessageState.Rest;
                    if (onRecycle != null)
                    {
                        onRecycle.Invoke(this);
                    }
                    Reset();
                }

                public MessageAwaiter GetAwaiter()
                {
                    return new MessageAwaiter(this);
                }
            }
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
        private class HandlerQueue : IDisposable
        {
            private struct Subject : IDisposable
            {
                private readonly Type _listenType;
                public Type listenType { get { return _listenType; } }
                private List<MessageListener> _listeners;
                public Subject(Type listenType)
                {
                    this._listenType = listenType;
                    _listeners = Framework.GlobalAllocate<List<MessageListener>>();
                }
                public bool Subscribe(MessageListener listener)
                {
                    if (_listeners.Contains(listener)) return false;
                    else _listeners.Add(listener); return true;
                }
                public bool UnSubscribe(MessageListener listener)
                {
                    if (!_listeners.Contains(listener)) return false;
                    else _listeners.Remove(listener); return true;
                }
                public bool Publish(IMessage message)
                {
                    if (_listeners.Count == 0) return false;
                    for (int i = _listeners.Count - 1; i >= 0; i--)
                    {
                        var listener = _listeners[i];
                        if (listener != null) listener.Invoke(message);
                        else _listeners.Remove(listener);
                    }
                    return true;
                }
                public void Dispose()
                {
                    _listeners.Clear();
                    _listeners.GlobalRecyle();
                }
            }
            private class DelegateSubscribe : IValueContainer<MessageListener>
            {
                public Type type;
                public MessageListener value { get; set; }
            }
            private Queue<DelegateSubscribe> _subscribeQueue = Framework.GlobalAllocate<Queue<DelegateSubscribe>>();
            private Queue<DelegateSubscribe> _unsubscribeQueue = Framework.GlobalAllocate<Queue<DelegateSubscribe>>();
            private List<Subject> subjects = Framework.GlobalAllocate<List<Subject>>();
            private Dictionary<Type, int> subjectMap = Framework.GlobalAllocate<Dictionary<Type, int>>();
            private LockParam _lock = new LockParam();
            private bool _fitSubType = false;

            public bool fitSubType { get { return _fitSubType; } set { _fitSubType = value; } }

            public void Subscribe(Type type, MessageListener listener)
            {
                using (new LockWait(ref _lock))
                {
                    var s = Framework.GlobalAllocate<DelegateSubscribe>();
                    s.type = type;
                    s.value = listener;
                    _subscribeQueue.Enqueue(s);
                }
            }
            public void UnSubscribe(Type type, MessageListener listener)
            {
                using (new LockWait(ref _lock))
                {
                    var s = Framework.GlobalAllocate<DelegateSubscribe>();
                    s.type = type;
                    s.value = listener;
                    _unsubscribeQueue.Enqueue(s);
                }
            }
            public bool Publish(IMessage message)
            {
                bool success = false;
                var type = message.subject;
                if (fitSubType)
                {
                    foreach (var _listenType in subjectMap.Keys)
                    {
                        if (type.IsExtendInterface(_listenType) || type.IsSubclassOf(_listenType) || type == _listenType)
                        {
                            success |= subjects[subjectMap[_listenType]].Publish(message);
                        }
                    }
                }
                else
                {
                    if (subjectMap.ContainsKey(type))
                    {
                        success |= subjects[subjectMap[type]].Publish(message);
                    }
                }
                return success;
            }
            public void Update()
            {
                int count = 0;

                using (new LockWait(ref _lock))
                {
                    int index = 0;
                    Type type = null;
                    count = _subscribeQueue.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var value = _subscribeQueue.Dequeue();
                        type = value.type;
                        Subject en;
                        if (!subjectMap.TryGetValue(type, out index))
                        {
                            index = subjects.Count;
                            en = new Subject(type);
                            subjectMap.Add(type, index);
                            subjects.Add(en);
                        }
                        else
                        {
                            en = subjects[subjectMap[type]];
                        }
                        en.Subscribe(value.value);
                        value.GlobalRecyle();
                    }
                    count = _unsubscribeQueue.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var value = _unsubscribeQueue.Dequeue();
                        type = value.type;
                        if (subjectMap.TryGetValue(type, out index))
                        {
                            subjects[subjectMap[type]].UnSubscribe(value.value);
                        }
                        value.GlobalRecyle();
                    }
                }

            }

            public void Dispose()
            {
                for (int i = 0; i < subjects.Count; i++) subjects[i].Dispose();
                subjects.Clear();
                subjectMap.Clear();
                _subscribeQueue.Clear();
                _unsubscribeQueue.Clear();
                _subscribeQueue.GlobalRecyle();
                _unsubscribeQueue.GlobalRecyle();
                subjects.GlobalRecyle();
                subjectMap.GlobalRecyle();
            }
        }

        private MessageQueue messages;
        private HandlerQueue handlers;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override ModulePriority OnGetDefaulyPriority()
        {
            return ModulePriority.Message;
        }
        protected override void OnDispose()
        {
            handlers.Dispose();
            messages.Dispose();
        }
        protected override void Awake()
        {
            messages = new MessageQueue(this);
            handlers = new HandlerQueue();
        }


        protected override void OnUpdate()
        {
            handlers.Update();
            messages.Update();
        }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /// <summary>
        /// 剩余消息数目
        /// </summary>
        public int count
        {
            get
            {
                return messages.count;
            }
        }
        /// <summary>
        /// 适配子类型
        /// </summary>
        public bool fitSubType { get { return handlers.fitSubType; } set { handlers.fitSubType = value; } }
        /// <summary>
        /// 每帧处理消息个数
        /// </summary>
        public int processesPerFrame { get { return messages.processesPerFrame; } set { messages.processesPerFrame = value; } }




        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe(Type type, IMessageListener listener)
        {
            Subscribe(type, listener.Listen);
        }
        /// <summary>
        /// 注册监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe<T>(IMessageListener listener)
        {
            Subscribe(typeof(T), listener);
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void UnSubscribe(Type type, IMessageListener listener)
        {
            UnSubscribe(type, listener.Listen);
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void UnSubscribe<T>(IMessageListener listener)
        {
            UnSubscribe(typeof(T), listener);
        }



        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe(Type type, MessageListener listener)
        {
            handlers.Subscribe(type, listener);
        }
        /// <summary>
        /// 注册监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe<T>(MessageListener listener)
        {
            Subscribe(typeof(T), listener);
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void UnSubscribe(Type type, MessageListener listener)
        {
            handlers.UnSubscribe(type, listener);
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void UnSubscribe<T>(MessageListener listener)
        {
            UnSubscribe(typeof(T), listener);
        }


        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage Publish<T>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return Publish(typeof(T), args, priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage Publish<T>(T t, IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return Publish(typeof(T), args, priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage PublishByNumber<T>(IEventArgs args, int priority = MessageUrgency.Common)
        {
            return PublishByNumber(typeof(T), args, priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage PublishByNumber<T>(T t, IEventArgs args, int priority = MessageUrgency.Common)
        {
            return PublishByNumber(typeof(T), args, priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage Publish(Type type, IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return PublishByNumber(type, args, (int)priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="priority">越大处理越晚</param>
        /// <returns></returns>
        public IMessage PublishByNumber(Type type, IEventArgs args, int priority = MessageUrgency.Common)
        {
            return messages.PublishByNumber(type, args, priority);
        }
    }
}
