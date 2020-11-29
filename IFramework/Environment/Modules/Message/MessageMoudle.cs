using System;
using System.Collections.Generic;
using IFramework.Queue;
namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息模块
    /// </summary>
    [ScriptVersionAttribute(120)]
    [VersionUpdateAttribute(120,"加入消息优先级以及进程等待")]
    public class MessageModule : UpdateFrameworkModule, IMessageModule
    {
        private interface IMessageEntity : IDisposable
        {
            Type listenType { get; }
            bool Publish(Type publishType, int code, IEventArgs args, params object[] param);
        }
        private class MessageEntity : IMessageEntity
        {
            private readonly Type _listenType;
            public Type listenType { get { return _listenType; } }
            private List<IMessageListener> _listenners;
            private LockParam _lock = new LockParam();
            public MessageEntity(Type listenType)
            {
                using (new LockWait(ref _lock))
                {
                    this._listenType = listenType;
                    _listenners = new List<IMessageListener>();
                }
            }
            public bool Subscribe(IMessageListener listener)
            {
                using (new LockWait(ref _lock))
                {
                    if (_listenners.Contains(listener)) return false;
                    else _listenners.Add(listener); return true;
                }
            }
            public bool UnSubscribe(IMessageListener listener)
            {
                using (new LockWait(ref _lock))
                {
                    if (!_listenners.Contains(listener)) return false;
                    else _listenners.Remove(listener); return true;
                }
            }
            public bool Publish(Type publishType, int code, IEventArgs args, params object[] param)
            {
                using (new LockWait(ref _lock))
                {
                    if (_listenners.Count <= 0)
                        return false;
                    if (!publishType.IsSubClassOfInterface(_listenType) &&
                        !publishType.IsSubclassOf(_listenType) &&
                        publishType != _listenType) return false;

                    _listenners.ForEach((index, listener) => {
                        if (listener != null) listener.Listen(publishType, code, args, param);
                        else _listenners.Remove(listener);
                    });
                    return true;
                }
            }
            public void Dispose()
            {
                using (new LockWait(ref _lock))
                {
                    _listenners.Clear();
                    _listenners = null;
                }
            }
        }
        private class DelgateMessageEntity : IMessageEntity
        {
            private readonly Type _listenType;
            public Type listenType { get { return _listenType; } }
            private List<MessageListener> _listenners;
            private LockParam _lock = new LockParam();
            public DelgateMessageEntity(Type listenType)
            {
                using (new LockWait(ref _lock))
                {
                    this._listenType = listenType;
                    _listenners = new List<MessageListener>();
                }
            }
            public bool Subscribe(MessageListener listener)
            {
                using (new LockWait(ref _lock))
                {
                    if (_listenners.Contains(listener)) return false;
                    else _listenners.Add(listener); return true;
                }
            }
            public bool UnSubscribe(MessageListener listener)
            {
                using (new LockWait(ref _lock))
                {
                    if (!_listenners.Contains(listener)) return false;
                    else _listenners.Remove(listener); return true;
                }
            }
            public bool Publish(Type publishType, int code, IEventArgs args, params object[] param)
            {
                using (new LockWait(ref _lock))
                {
                    if (_listenners.Count <= 0)
                        return false;
                    if (!publishType.IsSubClassOfInterface(_listenType) &&
                        !publishType.IsSubclassOf(_listenType) &&
                        publishType != _listenType) return false;

                    _listenners.ForEach((index, listener) => {
                        if (listener != null) listener(publishType, code, args, param);
                        else _listenners.Remove(listener);
                    });
                    return true;
                }
            }
            public void Dispose()
            {
                using (new LockWait(ref _lock))
                {
                    _listenners.Clear();
                    _listenners = null;
                }
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
        private List<DelgateMessageEntity> _entitydels;

        private StablePriorityQueue<Message> _queue;
        private MessagePool _pool;
        private List<Message> _list;

        private int _processesPerFrame=20;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public override int priority { get { return 20; } }
        protected override void OnDispose()
        {
            _queue.Clear();
            _pool.Clear();
            _list.Clear();
            foreach (var item in _entitys)
                item.Dispose();
            _entitys.Clear();
            foreach (var item in _entitydels)
                item.Dispose();
            _entitydels.Clear();
            _entitys = null;
            _entitydels = null;
        }
        protected override void Awake()
        {
            _entitys = new List<MessageEntity>();
            _entitydels = new List<DelgateMessageEntity>();
            _queue = new StablePriorityQueue<Message>();
            _pool = new MessagePool();
            _list = new List<Message>();
        }

        private LockParam _lock = new LockParam();
        private Queue<Message> _tmp = new Queue<Message>();

        protected override void OnUpdate()
        {
            using (new LockWait(ref _lock))
            {
                int count = Math.Min(processesPerFrame, _queue.count);
                if (count == 0) return;
                for (int i = 0; i < count; i++)
                {
                    var message = _queue.Dequeue();
                    _tmp.Enqueue(message);
                    _list.Remove(message);
                }
                if (_list.Count > 0)
                {
                    for (int i = 0; i < _list.Count; i++)
                    {
                        _queue.UpdatePriority(_list[i], _list[i].priority - 1);
                    }
                }
            }

            while (_tmp.Count > 0)
            {
                var message = _tmp.Dequeue();
                _pool.Set(message);
                _entitys.ForEach((index, en) =>
                {
                    en.Publish(message.publishType, message.code, message.args, message.param);
                });
                _entitydels.ForEach((index, en) =>
                {
                    en.Publish(message.publishType, message.code, message.args, message.param);
                });
            }
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /// <summary>
        /// 每帧处理消息个数
        /// </summary>
        public int processesPerFrame { get { return _processesPerFrame; }set { _processesPerFrame = value; } }
        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool Subscribe(Type publishType, IMessageListener listener)
        {
            MessageEntity en = _entitys.Find((e) => { return e.listenType == publishType; });
            if (en == null)
            {
                en = new MessageEntity(publishType);
                _entitys.Add(en);
            }

            return en.Subscribe(listener);
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
            MessageEntity en = _entitys.Find((e) => { return e.listenType == publishType; });
            if (en == null)
            {
                return false;
            }
            return en.UnSubscribe(listener);

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
            DelgateMessageEntity en = _entitydels.Find((e) => { return e.listenType == publishType; });
            if (en == null)
            {
                en = new DelgateMessageEntity(publishType);
                _entitydels.Add(en);
            }

            return en.Subscribe(listener);
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
            DelgateMessageEntity en = _entitydels.Find((e) => { return e.listenType == publishType; });
            if (en == null)
            {
                return false;
            }
            return en.UnSubscribe(listener);
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
        public void PublishByNumber(Type publishType, int code, IEventArgs args, int priority= MessageUrgency.Common, params object[] param)
        {
            var message = _pool.Get();
            message.publishType = publishType;
            message.code = code;
            message.args = args;
            message.param = param;
            using (new LockWait(ref _lock))
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
