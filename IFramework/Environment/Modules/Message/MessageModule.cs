using System;
using System.Collections.Generic;
using IFramework.Queue;
namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息模块
    /// </summary>
    [ScriptVersionAttribute(180)]
    [VersionUpdateAttribute(120, "加入消息优先级以及进程等待")]
    [VersionUpdateAttribute(140, "增加子类型匹配")]
    [VersionUpdateAttribute(180, "抽象出IMessage")]
    public class MessageModule : UpdateFrameworkModule, IMessageModule
    {
        private interface IMessageEntity : IDisposable
        {
            Type listenType { get; }
            bool Publish(IMessage message);
        }
        private class MessageEntity : IMessageEntity
        {
            private readonly Type _listenType;
            public Type listenType { get { return _listenType; } }
            private List<IMessageListener> _listeners;
            public MessageEntity(Type listenType)
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
        private class DelgateMessageEntity : IMessageEntity
        {
            private readonly Type _listenType;
            public Type listenType { get { return _listenType; } }
            private List<MessageListener> _listeners;
            public DelgateMessageEntity(Type listenType)
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
            private Type _type;
            private IEventArgs _args;
            private MessageModule _module;
            private MessageState _state;
            private MessageErrorCode _errCode;
            private event Action<IMessage> onRecycle;



            public int code { get { return _code; } }
            public Type type { get { return _type; } }
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
                _type = null;
                onRecycle = null;
            }
            internal void Begin()
            {
                Reset();
                _state = MessageState.Wait;
            }
            internal Message SetType(Type type)
            {
                this._type = type;
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


        private List<MessageEntity> _entitys;
        private List<DelgateMessageEntity> _delEntitys;
        private Dictionary<Type, int> _entityMap;
        private Dictionary<Type, int> _delEntityMap;


        private StablePriorityQueue<Message> _queue;
        private MessagePool _pool;
        private List<Message> _list;
        private LockParam _lock_message = new LockParam();
        private LockParam _lock_entity = new LockParam();
        private LockParam _lock_entitydel = new LockParam();

        private Queue<Message> _tmp = new Queue<Message>();

        private int _processesPerFrame = 20;
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
            _entitys = null;
            _delEntitys = null;
        }
        protected override void Awake()
        {
            _entitys = new List<MessageEntity>();
            _delEntitys = new List<DelgateMessageEntity>();
            _queue = new StablePriorityQueue<Message>();
            _pool = new MessagePool(this);
            _list = new List<Message>();
            _entityMap = new Dictionary<Type, int>();
            _delEntityMap = new Dictionary<Type, int>();
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
            var type = message.type;
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
            var type = message.type;
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
                message.Lock();
                bool sucess = false;
                using (new LockWait(ref _lock_entity))
                {
                    sucess|= EntityPublish(message);
                }
                using (new LockWait(ref _lock_entitydel))
                {
                    sucess|= DelEntityPublish(message);
                }
                message.SetErrorCode(sucess ? MessageErrorCode.Success : MessageErrorCode.NoneListen);
                _pool.Set(message);
            }
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
        public bool Subscribe(Type type, IMessageListener listener)
        {
            using (new LockWait(ref _lock_entity))
            {
                int index;
                MessageEntity en;
                if (!_entityMap.TryGetValue(type, out index))
                {
                    index = _entitys.Count;
                    en = new MessageEntity(type);
                    _entityMap.Add(type, index);
                    _entitys.Add(en);
                }
                else
                {
                    en = _entitys[_entityMap[type]];
                }
                return en.Subscribe(listener);
            }
        }
        /// <summary>
        /// 注册监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool Subscribe<T>(IMessageListener listener) where T : IMessagePublisher
        {
            return Subscribe(typeof(T), listener);
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool UnSubscribe(Type type, IMessageListener listener)
        {
            using (new LockWait(ref _lock_entity))
            {
                int index;
                if (!_entityMap.TryGetValue(type, out index)) return false;
                return _entitys[_entityMap[type]].UnSubscribe(listener);
            }
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool UnSubscribe<T>(IMessageListener listener) where T : IMessagePublisher
        {
            return UnSubscribe(typeof(T), listener);
        }



        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool Subscribe(Type type, MessageListener listener)
        {
            using (new LockWait(ref _lock_entitydel))
            {
                int index;
                DelgateMessageEntity en;
                if (!_delEntityMap.TryGetValue(type, out index))
                {
                    index = _delEntitys.Count;
                    en = new DelgateMessageEntity(type);
                    _delEntityMap.Add(type, index);
                    _delEntitys.Add(en);
                }
                else
                {
                    en = _delEntitys[_delEntityMap[type]];
                }
                return en.Subscribe(listener);
            }
        }
        /// <summary>
        /// 注册监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool Subscribe<T>(MessageListener listener) where T : IMessagePublisher
        {
            return Subscribe(typeof(T), listener);
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool UnSubscribe(Type type, MessageListener listener)
        {
            using (new LockWait(ref _lock_entitydel))
            {
                int index;
                if (!_delEntityMap.TryGetValue(type, out index))
                {
                    return false;
                }
                else
                {
                    return _delEntitys[_delEntityMap[type]].UnSubscribe(listener);
                }
            }

        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool UnSubscribe<T>(MessageListener listener) where T : IMessagePublisher
        {
            return UnSubscribe(typeof(T), listener);
        }


        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage Publish<T>(IEventArgs args, MessageUrgencyType priority = MessageUrgencyType.Common) where T : IMessagePublisher
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
        public IMessage Publish<T>(T t, IEventArgs args,  MessageUrgencyType priority = MessageUrgencyType.Common) where T : IMessagePublisher
        {
            return Publish(t.GetType(), args, priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage PublishByNumber<T>(IEventArgs args, int priority = MessageUrgency.Common) where T : IMessagePublisher
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
        public IMessage PublishByNumber<T>(T t, IEventArgs args, int priority = MessageUrgency.Common) where T : IMessagePublisher
        {
            return PublishByNumber(t.GetType(), args, priority);
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
            using (new LockWait(ref _lock_message))
            {
                if (_queue.count == _queue.capcity)
                {
                    _queue.Resize(_queue.capcity * 2);
                }
                _queue.Enqueue(message, priority);
                _list.Add(message);
            }
            return message;
        }
    }
}
