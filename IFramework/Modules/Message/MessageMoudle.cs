using System;
using System.Collections.Generic;

namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息监听
    /// </summary>
    /// <param name="publishType"></param>
    /// <param name="code"></param>
    /// <param name="args"></param>
    /// <param name="param"></param>
    public delegate void MessageListener(Type publishType, int code, IEventArgs args, object[] param);
    /// <summary>
    /// 消息模块
    /// </summary>
    [FrameworkVersion(101)]
    public class MessageModule : FrameworkModule
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

            public MessageEntity(Type listenType)
            {
                this._listenType = listenType;
                _listenners = new List<IMessageListener>();
            }
            public bool Subscribe(IMessageListener listener)
            {
                if (_listenners.Contains(listener)) return false;
                else _listenners.Add(listener); return true;
            }
            public bool UnSubscribe(IMessageListener listener)
            {
                if (!_listenners.Contains(listener)) return false;
                else _listenners.Remove(listener); return true;
            }

            public bool Publish(Type publishType, int code, IEventArgs args, params object[] param)
            {
                if (_listenners.Count <= 0)
                    return false;
                if (!publishType.IsSubClassOfInterface(_listenType) &&
                    !publishType.IsSubclassOf(_listenType) &&
                    publishType != _listenType) return false;
                
                _listenners.ForEach((index,listener) => {
                    if (listener != null) listener.Listen(publishType, code, args, param);
                    else _listenners.Remove(listener);
                });
                return true;
            }

            public void Dispose()
            {
                _listenners.Clear();
                _listenners = null;
            }
        }
        private class DelgateMessageEntity : IMessageEntity
        {
            private readonly Type _listenType;
            public Type listenType { get { return _listenType; } }
            private List<MessageListener> _listenners;

            public DelgateMessageEntity(Type listenType)
            {
                this._listenType = listenType;
                _listenners = new List<MessageListener>();
            }
            public bool Subscribe(MessageListener listener)
            {
                if (_listenners.Contains(listener)) return false;
                else _listenners.Add(listener); return true;
            }
            public bool UnSubscribe(MessageListener listener)
            {
                if (!_listenners.Contains(listener)) return false;
                else _listenners.Remove(listener); return true;
            }

            public bool Publish(Type publishType, int code, IEventArgs args, params object[] param)
            {
                if (_listenners.Count <= 0)
                    return false;
                if (!publishType.IsSubClassOfInterface(_listenType) &&
                    !publishType.IsSubclassOf(_listenType) &&
                    publishType != _listenType) return false;

                _listenners.ForEach((index,listener) => {
                    if (listener != null) listener.Invoke(publishType, code, args, param);
                    else _listenners.Remove(listener);
                });
                return true;
            }

            public void Dispose()
            {
                _listenners.Clear();
                _listenners = null;
            }
        }

        private List</*Type,*/ MessageEntity> _entitys;
        private List</*Type,*/ DelgateMessageEntity> _entitydels;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public override int priority { get { return 20; } }
        protected override void OnDispose()
        {
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
        }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

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
        /// <param name="param"></param>
        /// <returns></returns>
        public bool Publish<T>(int code, IEventArgs args, params object[] param) where T : IMessagePublisher
        {
            return Publish(typeof(T), code, args, param);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool Publish<T>(T t, int code, IEventArgs args, params object[] param) where T : IMessagePublisher
        {
            return Publish(t.GetType(), code, args, param);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="publishType"></param>
        /// <param name="code"></param>
        /// <param name="args"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool Publish(Type publishType, int code, IEventArgs args, params object[] param)
        {
            bool value = false;
            _entitys.ForEach((index, en) =>
            {

                if (en.Publish(publishType, code, args, param))
                    value = true;
            });
            _entitydels.ForEach((index, en) =>
            {

                if (en.Publish(publishType, code, args, param))
                    value = true;
            });
            return value;
        }

    }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    [FrameworkVersion(5)]
    public static class MessageMouduleExtension

    {
        public static bool Subscribe(this IMessageListener listener, EnvironmentType envType, Type publishType)
        {
            FrameworkEnvironment _env = Framework.GetEnv( envType);
            return _env.modules.Message.Subscribe(publishType, listener);
        }
        public static bool Subscribe<T>(this IMessageListener listener, EnvironmentType envType) where T : IMessagePublisher
        {
            return Subscribe(listener,  envType, typeof(T));
        }
        public static bool UnSubscribe(this IMessageListener listener, EnvironmentType envType, Type publishType)
        {
            FrameworkEnvironment _env = Framework.GetEnv( envType);
            return _env.modules.Message.UnSubscribe(publishType, listener);
        }
        public static bool UnSubscribe<T>(this IMessageListener listener, EnvironmentType envType) where T : IMessagePublisher
        {
            return UnSubscribe(listener,  envType, typeof(T));
        }

        public static bool Subscribe(this object obj, Type publishType, MessageListener listener, EnvironmentType envType)
        {
            FrameworkEnvironment _env = Framework.GetEnv( envType);
            return _env.modules.Message.Subscribe(publishType, listener);
        }
        public static bool Subscribe<T>(this object obj, MessageListener listener, EnvironmentType envType) where T : IMessagePublisher
        {
            return Subscribe(obj, typeof(T), listener,  envType);
        }
        public static bool UnSubscribe(this object obj, Type publishType, MessageListener listener, EnvironmentType envType)
        {
            FrameworkEnvironment _env = Framework.GetEnv( envType);
            return _env.modules.Message.UnSubscribe(publishType, listener);
        }
        public static bool UnSubscribe<T>(this object obj, MessageListener listener, EnvironmentType envType) where T : IMessagePublisher
        {
            return UnSubscribe(obj, typeof(T), listener,  envType);
        }


        public static bool Publish(this object obj, EnvironmentType envType, Type publishType, int code, IEventArgs args, params object[] param)
        {
            FrameworkEnvironment _env = Framework.GetEnv( envType);
            return _env.modules.Message.Publish(publishType, code, args, param);
        }
        public static bool Publish<T>(this object obj, EnvironmentType envType, int code, IEventArgs args, params object[] param) where T : IMessagePublisher
        {
            return Publish(obj,  envType, typeof(T), code, args, param);
        }
        public static bool Publish<T>(this T obj, EnvironmentType envType, int code, IEventArgs args, params object[] param) where T : IMessagePublisher
        {
            return Publish(obj,  envType, typeof(T), code, args, param);
        }

        //public static bool Subscribe(this object obj, EnvironmentType envType, Type publishType, IMessageListener listener)
        //{
        //    FrameworkEnvironment _env = Framework.GetEnv( envType);
        //    return _env.modules.Message.Subscribe(publishType, listener);
        //}
        //public static bool Subscribe<T>(this object obj, EnvironmentType envType, IMessageListener listener) where T : IMessagePublisher
        //{
        //    return Subscribe(obj,  envType, typeof(T), listener);
        //}
        //public static bool UnSubscribe(this object obj, EnvironmentType envType, Type publishType, IMessageListener listener)
        //{
        //    FrameworkEnvironment _env = Framework.GetEnv( envType);
        //    return _env.modules.Message.UnSubscribe(publishType, listener);
        //}
        //public static bool UnSubscribe<T>(this object obj, EnvironmentType envType, IMessageListener listener) where T : IMessagePublisher
        //{
        //    return UnSubscribe(obj,  envType, typeof(T), listener);
        //}
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
