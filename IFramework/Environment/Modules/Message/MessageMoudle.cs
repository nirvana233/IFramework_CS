using System;
using System.Collections.Generic;
using IFramework.Queue;
namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息模块
    /// </summary>
    [ScriptVersionAttribute(140)]
    [VersionUpdateAttribute(120, "加入消息优先级以及进程等待")]
    [VersionUpdateAttribute(140, "增加子类型匹配")]
    public class MessageModule : UpdateFrameworkModule, IMessageModule
    {
        private interface IMessageEntity : IDisposable
        {
            Type listenType { get; }
            void Publish(Type publishType, int code, IEventArgs args, params object[] param);
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
            public void Publish(Type publishType, int code, IEventArgs args, params object[] param)
            {
                if (_listeners.Count <= 0) return;
                for (int i = _listeners.Count-1; i >= 0; i--)
                {
                    var listener = _listeners[i];
                    if (listener != null) listener.Listen(publishType, code, args, param);
                    else _listeners.Remove(listener);
                }
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
            public void Publish(Type publishType, int code, IEventArgs args, params object[] param)
            {
                if (_listeners.Count <= 0) return;
                for (int i = _listeners.Count - 1; i >= 0; i--)
                {
                    var listener = _listeners[i];
                    if (listener != null) listener.Invoke(publishType, code, args, param);
                    else _listeners.Remove(listener);
                }
            }
            public void Dispose()
            {
                _listeners.Clear();
                _listeners = null;
            }
        }

        private class Message : StablePriorityQueueNode
        {
            public Type publishType;
            public int code;
            public IEventArgs args;
            public object[] param;
        }

        private class MessagePool : ObjectPool<Message>
        {
            protected override Message CreatNew(IEventArgs arg)
            {
                return new Message();
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
        private bool _fitSubType=true;

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
            _pool = new MessagePool();
            _list = new List<Message>();
            _entityMap = new Dictionary<Type, int>();
            _delEntityMap = new Dictionary<Type, int>();
        }

        private void EntityPublish(Message message)
        {
            var type = message.publishType;
            if (fitSubType)
            {
                foreach (var _listenType in _entityMap.Keys)
                {
                    if (type.IsExtendInterface(_listenType) || type.IsSubclassOf(_listenType))
                    {
                        _entitys[_entityMap[_listenType]].Publish(type, message.code, message.args, message.param);
                    }
                }
            }
            else
            {
                if (_entityMap.ContainsKey(type))
                {
                    _entitys[_entityMap[type]].Publish(type, message.code, message.args, message.param);
                }
            }
           
        }
        private void DelEntityPublish(Message message)
        {
            var type = message.publishType;
            if (fitSubType)
            {
                foreach (var _listenType in _delEntityMap.Keys)
                {
                    if (type.IsExtendInterface(_listenType) || type.IsSubclassOf(_listenType))
                    {
                        _delEntitys[_delEntityMap[_listenType]].Publish(type, message.code, message.args, message.param);
                    }
                }
            }
            else
            {
                if (_delEntityMap.ContainsKey(type))
                {
                    _delEntitys[_delEntityMap[type]].Publish(type, message.code, message.args, message.param);
                }
            }

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
                using (new LockWait(ref _lock_entity))
                {
                    EntityPublish(message);
                }
                using (new LockWait(ref _lock_entitydel))
                {
                    DelEntityPublish(message);
                }
                _pool.Set(message);
            }
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
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
        /// <param name="publishType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool Subscribe(Type publishType, IMessageListener listener)
        {
            using (new LockWait(ref _lock_entity))
            {
                int index;
                MessageEntity en;
                if (!_entityMap.TryGetValue(publishType, out index))
                {
                    index = _entitys.Count;
                    en = new MessageEntity(publishType);
                    _entityMap.Add(publishType, index);
                    _entitys.Add(en);
                }
                else
                {
                    en = _entitys[_entityMap[publishType]];
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
        /// <param name="publishType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool UnSubscribe(Type publishType, IMessageListener listener)
        {
            using (new LockWait(ref _lock_entity))
            {
                int index;
                if (!_entityMap.TryGetValue(publishType, out index)) return false;
                return _entitys[_entityMap[publishType]].UnSubscribe(listener);
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
        /// <param name="publishType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool Subscribe(Type publishType, MessageListener listener)
        {
            using (new LockWait(ref _lock_entitydel))
            {
                int index;
                DelgateMessageEntity en;
                if (!_delEntityMap.TryGetValue(publishType, out index))
                {
                    index = _delEntitys.Count;
                    en = new DelgateMessageEntity(publishType);
                    _delEntityMap.Add(publishType, index);
                    _delEntitys.Add(en);
                }
                else
                {
                    en = _delEntitys[_delEntityMap[publishType]];
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
        /// <param name="publishType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool UnSubscribe(Type publishType, MessageListener listener)
        {
            using (new LockWait(ref _lock_entitydel))
            {
                int index;
                if (!_delEntityMap.TryGetValue(publishType, out index))
                {
                    return false;
                }
                else
                {
                    return _delEntitys[_delEntityMap[publishType]].UnSubscribe(listener);
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
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public void Publish<T>(int code, IEventArgs args, MessageUrgencyType type = MessageUrgencyType.Common, params object[] param) where T : IMessagePublisher
        {
            Publish(typeof(T), code, args, type, param);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public void Publish<T>(T t, int code, IEventArgs args, MessageUrgencyType type = MessageUrgencyType.Common, params object[] param) where T : IMessagePublisher
        {
            Publish(t.GetType(), code, args, type, param);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public void PublishByNumber<T>(int code, IEventArgs args, int priority = MessageUrgency.Common, params object[] param) where T : IMessagePublisher
        {
            PublishByNumber(typeof(T), code, args, priority, param);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="priority"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public void PublishByNumber<T>(T t, int code, IEventArgs args, int priority = MessageUrgency.Common, params object[] param) where T : IMessagePublisher
        {
            PublishByNumber(t.GetType(), code, args, priority, param);
        }


        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="type"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public void Publish(Type publishType, int code, IEventArgs args, MessageUrgencyType type = MessageUrgencyType.Common, params object[] param)
        {
            PublishByNumber(publishType, code, args, (int)type, param);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="priority">越大处理越晚</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public void PublishByNumber(Type publishType, int code, IEventArgs args, int priority = MessageUrgency.Common, params object[] param)
        {
            var message = _pool.Get();
            message.publishType = publishType;
            message.code = code;
            message.args = args;
            message.param = param;
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
    }
}
