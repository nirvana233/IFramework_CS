using System;
using System.Collections.Generic;
using IFramework.Queue;
namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息模块
    /// </summary>
    [ScriptVersionAttribute(230)]
    [VersionUpdateAttribute(120, "加入消息优先级以及进程等待")]
    [VersionUpdateAttribute(140, "增加子类型匹配")]
    [VersionUpdateAttribute(180, "抽象出IMessage")]
    [VersionUpdateAttribute(200, "增加立刻处理")]
    [VersionUpdateAttribute(230, "注册延时处理")]
    public class MessageModule : UpdateFrameworkModule, IMessageModule
    {
        private interface IMessageEntity : IDisposable
        {
            Type listenType { get; }
            bool Publish(IMessage message);
        }
        private class MessageSubject : IMessageEntity
        {
            private readonly Type _listenType;
            public Type listenType { get { return _listenType; } }
            private List<IMessageListener> _listeners;
            public MessageSubject(Type listenType)
            {
                this._listenType = listenType;
                _listeners = new List<IMessageListener>();
            }
            public bool Subscribe(IMessageListener listener)
            {
                if (_listeners.Contains(listener)) return false;
                else _listeners.Add(listener); return true;
            }
            public bool UnSubscribe(IMessageListener listener)
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
                    if (listener != null) listener.Listen(message);
                    else _listeners.Remove(listener);
                }
                return true;
            }
            public void Dispose()
            {
                _listeners.Clear();
                _listeners = null;
            }
        }
        private class DelgateMessageSubject : IMessageEntity
        {
            private readonly Type _listenType;
            public Type listenType { get { return _listenType; } }
            private List<MessageListener> _listeners;
            public DelgateMessageSubject(Type listenType)
            {
                this._listenType = listenType;
                _listeners = new List<MessageListener>();
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
                _listeners = null;
            }
        }

        private class Message : StablePriorityQueueNode, IMessage
        {

            private int _code = 0;
            private Type _subject;
            private IEventArgs _args;
            private MessageModule _module;
            private MessageState _state;
            private MessageErrorCode _errCode;
            private event Action<IMessage> onRecycle;



            public int code { get { return _code; } }
            public Type subject { get { return _subject; } }
            public IEventArgs args { get { return _args; } }
            public MessageState state { get { return _state; } }
            public MessageErrorCode errorCode { get { return _errCode; } }

            public Message(MessageModule module)
            {
                _module = module;
            }

          

            private void Reset()
            {
                this._errCode =  MessageErrorCode.None;
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
            public IMessage SetPriority(int priority)
            {
                if (_state != MessageState.Wait)
                {
                    Log.E(string.Format("you can not set the priority now, the state is {0}", _state));
                    return this;
                }
                if (this.priority == priority) return this;
                _module.UpdateMessagePriority(this, priority);
                return this;
            }
            public IMessage OnRecycle(Action<IMessage> action)
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
                if (_state== MessageState.Wait)
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
                if (onRecycle != null)
                {
                    onRecycle.Invoke(this);
                }
                Reset();
                _state = MessageState.Rest;
            }

 
        }

        private class MessagePool : ObjectPool<Message>
        {
            private MessageModule _module;

            public MessagePool(MessageModule module)
            {
                this._module = module;
            }

            protected override Message CreatNew(IEventArgs arg)
            {
                return new Message(_module);
            }

            protected override void OnGet(Message t, IEventArgs arg)
            {
                t.Begin();
            }
            protected override bool OnSet(Message t, IEventArgs arg)
            {
                t.End();
                return true;
            }
        }

        private class SubscribeContainer<T>:IValueContainer<T>
        {
            public Type type;
            public T value { get; set; }
        }
        private class ObjectSubscribe : SubscribeContainer<IMessageListener> { }
        private class DelegateSubscribe : SubscribeContainer<MessageListener> { }
        private class ObjectSubscribePool : ObjectPool<ObjectSubscribe>
        {
            protected override ObjectSubscribe CreatNew(IEventArgs arg)
            {
                return new ObjectSubscribe();
            }
            protected override bool OnSet(ObjectSubscribe t, IEventArgs arg)
            {
                t.type = null;
                t.value = null;
                return base.OnSet(t, arg);
            }
        }
        private class DelegateSubscribePool : ObjectPool<DelegateSubscribe>
        {
            protected override DelegateSubscribe CreatNew(IEventArgs arg)
            {
                return new DelegateSubscribe();
            }
            protected override bool OnSet(DelegateSubscribe t, IEventArgs arg)
            {
                t.type = null;
                t.value = null;
                return base.OnSet(t, arg);
            }
        }


        private ObjectSubscribePool _subscribePool_object;
        private DelegateSubscribePool _subscribePool_delegate;
        private Queue<DelegateSubscribe> _subscribeQueue_delegate, _unsubscribeQueue_delegate;
        private Queue<ObjectSubscribe> _subscribeQueue_object, _unsubscribeQueue_object;


        private List<MessageSubject> _entitys;
        private List<DelgateMessageSubject> _delEntitys;
        private Dictionary<Type, int> _entityMap;
        private Dictionary<Type, int> _delEntityMap;


        private StablePriorityQueue<Message> _queue;
        private MessagePool _pool;
        private List<Message> _list;
        private LockParam _lock_message = new LockParam();
        private LockParam _lock_entity = new LockParam();
        private LockParam _lock_entitydel = new LockParam();

        private Queue<Message> _tmp = new Queue<Message>();

        private int _processesPerFrame = 40;
        private bool _fitSubType = true;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public override int priority { get { return 20; } }
        protected override void OnDispose()
        {
            _queue.Clear();
            _pool.Clear();
            _list.Clear();
            for (int i = 0; i < _entitys.Count; i++) _entitys[i].Dispose();
            _entitys.Clear();
            _entityMap.Clear();
            for (int i = 0; i < _delEntitys.Count; i++) _delEntitys[i].Dispose();
            _delEntitys.Clear();
            _delEntityMap.Clear();

            _subscribePool_object.Dispose();
            _subscribePool_delegate.Dispose();
            _subscribeQueue_delegate.Clear();
            _unsubscribeQueue_delegate.Clear();
            _subscribeQueue_object.Clear();
            _unsubscribeQueue_object.Clear();
            _entitys = null;
            _delEntitys = null;
        }
        protected override void Awake()
        {
            _entitys = new List<MessageSubject>();
            _delEntitys = new List<DelgateMessageSubject>();
            _queue = new StablePriorityQueue<Message>();
            _pool = new MessagePool(this);
            _list = new List<Message>();
            _entityMap = new Dictionary<Type, int>();
            _delEntityMap = new Dictionary<Type, int>();

            _subscribePool_object = new ObjectSubscribePool();
            _subscribePool_delegate = new DelegateSubscribePool();
            _subscribeQueue_delegate = new Queue<DelegateSubscribe>();
            _unsubscribeQueue_delegate = new Queue<DelegateSubscribe>();
            _subscribeQueue_object = new Queue<ObjectSubscribe>();
            _unsubscribeQueue_object = new Queue<ObjectSubscribe>();
        }
        private void UpdateMessagePriority(Message message, int priority)
        {
            using (new LockWait(ref _lock_message))
            {
                if (!_list.Contains(message)) return;
                if (message.priority == priority) return;
                _queue.UpdatePriority(message, priority);
            }
        }
        private bool EntityPublish(IMessage message)
        {
            bool success=false;
            var type = message.subject;
            if (fitSubType)
            {
                foreach (var _listenType in _entityMap.Keys)
                {
                    if (type.IsExtendInterface(_listenType) || type.IsSubclassOf(_listenType) || type == _listenType)
                    {
                        success |= _entitys[_entityMap[_listenType]].Publish(message);
                    }
                }
            }
            else
            {
                if (_entityMap.ContainsKey(type))
                {
                    success |= _entitys[_entityMap[type]].Publish(message);
                }
            }
            return success;
        }
        private bool DelEntityPublish(IMessage message)
        {
            bool success = false;
            var type = message.subject;
            if (fitSubType)
            {
                foreach (var _listenType in _delEntityMap.Keys)
                {
                    if (type.IsExtendInterface(_listenType) || type.IsSubclassOf(_listenType) || type == _listenType)
                    {
                        success |= _delEntitys[_delEntityMap[_listenType]].Publish(message);
                    }
                }
            }
            else
            {
                if (_delEntityMap.ContainsKey(type))
                {
                    success |= _delEntitys[_delEntityMap[type]].Publish(message);
                }
            }
            return success;
        }

        protected override void OnUpdate()
        {
            int count = 0;

            using (new LockWait(ref _lock_entity))
            {
                int index = 0;
                Type type = null;
                count = _subscribeQueue_object.Count;
                for (int i = 0; i < count; i++)
                {
                    var value = _subscribeQueue_object.Dequeue();
                    type = value.type;
                    MessageSubject en;
                    if (!_entityMap.TryGetValue(type, out index))
                    {
                        index = _entitys.Count;
                        en = new MessageSubject(type);
                        _entityMap.Add(type, index);
                        _entitys.Add(en);
                    }
                    else
                    {
                        en = _entitys[_entityMap[type]];
                    }
                    en.Subscribe(value.value);
                    _subscribePool_object.Set(value);
                }
                count = _unsubscribeQueue_object.Count;
                for (int i = 0; i < count; i++)
                {
                    var value = _unsubscribeQueue_object.Dequeue();
                    type = value.type;
                    if (_entityMap.TryGetValue(type, out index))
                    {
                        _entitys[_entityMap[type]].UnSubscribe(value.value);
                    }
                    _subscribePool_object.Set(value);
                }
            }
            using (new LockWait(ref _lock_entitydel))
            {
                int index = 0;
                Type type = null;
                count = _subscribeQueue_delegate.Count;
                for (int i = 0; i < count; i++)
                {
                    var value = _subscribeQueue_delegate.Dequeue();
                    type = value.type;
                    DelgateMessageSubject en;
                    if (!_delEntityMap.TryGetValue(type, out index))
                    {
                        index = _delEntitys.Count;
                        en = new DelgateMessageSubject(type);
                        _delEntityMap.Add(type, index);
                        _delEntitys.Add(en);
                    }
                    else
                    {
                        en = _delEntitys[_delEntityMap[type]];
                    }
                    en.Subscribe(value.value);
                    _subscribePool_delegate.Set(value);
                }
                count = _unsubscribeQueue_delegate.Count;
                for (int i = 0; i < count; i++)
                {
                    var value = _unsubscribeQueue_delegate.Dequeue();
                    type = value.type;
                    if (_delEntityMap.TryGetValue(type, out index))
                    {
                        _delEntitys[_delEntityMap[type]].UnSubscribe(value.value);
                    }
                    _subscribePool_delegate.Set(value);
                }
            }
            count = 0;
            using (new LockWait(ref _lock_message))
            {
                count = Math.Min(processesPerFrame, _queue.count);
                if (count == 0) return;
                for (int i = 0; i < count; i++)
                {
                    var message = _queue.Dequeue();
                    _tmp.Enqueue(message);
                    _list.Remove(message);
                }
                if (_list.Count > 0)
                {
                    for (int i = _list.Count - 1; i >= 0; i--)
                    {
                        _queue.UpdatePriority(_list[i], _list[i].priority - 1);
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                var message = _tmp.Dequeue();
                HandleMessage(message);
            }
        }

        private void HandleMessage(Message message)
        {
            message.Lock();
            bool sucess = false;
            sucess |= EntityPublish(message);
            sucess |= DelEntityPublish(message);
            message.SetErrorCode(sucess ? MessageErrorCode.Success : MessageErrorCode.NoneListen);
            _pool.Set(message);
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /// <summary>
        /// 剩余消息数目
        /// </summary>
        public int count { get {
                using (new LockWait(ref _lock_message))
                {
                    return _queue.count;
                }
            } }
        /// <summary>
        /// 适配子类型
        /// </summary>
        public bool fitSubType { get { return _fitSubType; } set { _fitSubType = value; } }
        /// <summary>
        /// 每帧处理消息个数
        /// </summary>
        public int processesPerFrame { get { return _processesPerFrame; } set { _processesPerFrame = value; } }
        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe(Type type, IMessageListener listener)
        {
            using (new LockWait(ref _lock_entity))
            {
                var s = _subscribePool_object.Get();
                s.type = type;
                s.value = listener;
                _subscribeQueue_object.Enqueue(s);
            }
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
            using (new LockWait(ref _lock_entity))
            {
                var s = _subscribePool_object.Get();
                s.type = type;
                s.value = listener;
                _unsubscribeQueue_object.Enqueue(s);
            }
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
            using (new LockWait(ref _lock_entitydel))
            {
                var s = _subscribePool_delegate.Get();
                s.type = type;
                s.value = listener;
                _subscribeQueue_delegate.Enqueue(s);
            }
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
            using (new LockWait(ref _lock_entitydel))
            {
                var s = _subscribePool_delegate.Get();
                s.type = type;
                s.value = listener;
                _unsubscribeQueue_delegate.Enqueue(s);
            }

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
        public IMessage Publish<T>(T t, IEventArgs args,  MessageUrgencyType priority = MessageUrgencyType.Common) 
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
        public IMessage Publish(Type type, IEventArgs args,  MessageUrgencyType priority = MessageUrgencyType.Common)
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
        public IMessage PublishByNumber(Type type, IEventArgs args,int priority = MessageUrgency.Common)
        {
            var message = _pool.Get();
            message.SetArgs(args).SetType(type);
            if (priority<0)
            {
                HandleMessage(message);
            }
            else
            {
                using (new LockWait(ref _lock_message))
                {
                    if (_queue.count == _queue.capcity)
                    {
                        _queue.Resize(_queue.capcity * 2);
                    }
                    _queue.Enqueue(message, priority);
                    _list.Add(message);
                }
            }
            return message;
        }
    }
}
