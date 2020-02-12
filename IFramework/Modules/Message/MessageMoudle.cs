using System;
using System.Collections.Generic;

namespace IFramework.Modules.Message
{
    public delegate void MessageLostener(IMessagePublisher publisher, Type type, int code, IEventArgs args, object[] param);
    [FrameworkVersion(88)]
    public class MessageModule : FrameworkModule, IMessageModule
    {
        private interface IMessageEnity : IDisposable
        {
            Type ObserveType { get; }
            //bool Subscribe(IObserver listener);
            //bool Unsubscribe(IObserver listener);
            bool Publish(IMessagePublisher publisher, Type type, int code, IEventArgs args, params object[] param);
        }
        private class MessageEnity : IMessageEnity
        {
            private LockParam para = new LockParam();
            private readonly Type observeType;
            //private IList<Type> typeTree;

            public Type ObserveType { get { return observeType; } }

            public MessageEnity(Type observeType) {
                //typeTree = observeType.GetTypeTree();

                this.observeType = observeType;
            }
            private List<IMessageListener> observers = new List<IMessageListener>();
            public bool Subscribe(IMessageListener listener)
            {
                using (new LockWait(ref para))
                {
                    if (observers.Contains(listener)) return false;
                    else observers.Add(listener); return true;
                }
            }
            public bool Unsubscribe(IMessageListener listener)
            {
                using (new LockWait(ref para))
                {
                    if (!observers.Contains(listener)) return false;
                    else observers.Remove(listener); return true;
                }
            }

            public bool Publish(IMessagePublisher publisher, Type type, int code, IEventArgs args, params object[] param)
            {
                using (new LockWait(ref para))
                {
                    if (observers.Count <= 0)
                        return false;
                    if (!type.IsSubClassOfInterface(observeType) &&
                        !type.IsSubclassOf(observeType) && 
                        type != observeType
                        /* &&*/ 
                        /*!typeTree.Contains(type)*/) return false;

                    observers.ForEach((listener) => {
                        if (listener != null) listener.Listen(publisher, type, code, args, param);
                        else observers.Remove(listener);
                    });
                    return true;
                }
            }

            public void Dispose()
            {
                using (new LockWait(ref para))
                {
                    observers.Clear();
                    observers = null;
                }
            }
        }
        private class DelgateMessageEnity: IMessageEnity
        {
            private LockParam para = new LockParam();
            private readonly Type observeType;
            //private IList<Type> typeTree;

            public Type ObserveType { get { return observeType; } }

            public DelgateMessageEnity(Type observeType)
            {
                //typeTree = observeType.GetTypeTree();

                this.observeType = observeType;
            }
            private List<MessageLostener> observers = new List<MessageLostener>();
            public bool Subscribe(MessageLostener listener)
            {
                using (new LockWait(ref para))
                {
                    if (observers.Contains(listener)) return false;
                    else observers.Add(listener); return true;
                }
            }
            public bool Unsubscribe(MessageLostener listener)
            {
                using (new LockWait(ref para))
                {
                    if (!observers.Contains(listener)) return false;
                    else observers.Remove(listener); return true;
                }
            }

            public bool Publish(IMessagePublisher publisher, Type type, int code, IEventArgs args, params object[] param)
            {
                using (new LockWait(ref para))
                {
                    if (observers.Count <= 0)
                        return false;
                    if (!type.IsSubClassOfInterface(observeType) &&
                        !type.IsSubclassOf(observeType) &&
                        type != observeType 
                       
                        /*&&*/
                        /*!typeTree.Contains(type)*/) return false;

                    observers.ForEach((listener) => {
                        if (listener != null) listener.Invoke(publisher, type, code, args, param);
                        else observers.Remove(listener);
                    });
                    return true;
                }
            }

            public void Dispose()
            {
                using (new LockWait(ref para))
                {
                    observers.Clear();
                    observers = null;
                }
            }
        }




        private LockParam para = new LockParam();
        private Dictionary<Type, MessageEnity> observes;
        private Dictionary<Type, DelgateMessageEnity> observedels;

        protected override bool needUpdate { get { return false; } }



        protected override void OnDispose()
        {
            foreach (var item in observes.Values)
                item.Dispose();
            observes.Clear();
            foreach (var item in observedels.Values)
                item.Dispose();
            observedels.Clear();
            observes = null;
            observedels = null;
        }
        protected override void Awake()
        {
            observes = new Dictionary<Type, MessageEnity>();
            observedels = new Dictionary<Type, DelgateMessageEnity>();
        }
        protected override void OnUpdate()
        {
           
        }

        public bool Subscribe(Type type, IMessageListener listener)
        {
            using (new LockWait(ref para))
            {
                if (!observes.ContainsKey(type))
                    observes.Add(type, new MessageEnity(type));
                return observes[type].Subscribe(listener);
            }
        }
        public bool Subscribe<T>(IMessageListener listener) where T : IMessagePublisher
        {
            return Subscribe(typeof(T), listener);
        }
        public bool Unsubscribe(Type type, IMessageListener listener)
        {
            using (new LockWait(ref para))
            {
                if (!observes.ContainsKey(type)) return false;
                return observes[type].Unsubscribe(listener);
            }
        }
        public bool Unsubscribe<T>(IMessageListener listener) where T : IMessagePublisher
        {
            return Unsubscribe(typeof(T), listener);
        }

        public bool Subscribe(Type type, MessageLostener listener)
        {
            using (new LockWait(ref para))
            {
                if (!observedels.ContainsKey(type))
                    observedels.Add(type, new DelgateMessageEnity(type));
                return observedels[type].Subscribe(listener);
            }
        }
        public bool Subscribe<T>(MessageLostener listener) where T : IMessagePublisher
        {
            return Subscribe(typeof(T), listener);
        }
        public bool Unsubscribe(Type type, MessageLostener listener)
        {
            using (new LockWait(ref para))
            {
                if (!observedels.ContainsKey(type)) return false;
                return observedels[type].Unsubscribe(listener);
            }
        }
        public bool Unsubscribe<T>(MessageLostener listener) where T : IMessagePublisher
        {
            return Unsubscribe(typeof(T), listener);
        }

        public bool Publish<T>(int code, IEventArgs args, params object[] param) where T : IMessagePublisher
        {
            return Publish(typeof(T), code, args, param);
        }
        public bool Publish<T>(T t, int code, IEventArgs args, params object[] param) where T : IMessagePublisher
        {
            return Publish(t, typeof(T), code, args, param);
        }
        public bool Publish(Type type, int code, IEventArgs args, params object[] param)
        {
            return Publish(null, type, code, args, param);
        }
        public bool Publish(IMessagePublisher publisher, Type type, int code, IEventArgs args, params object[] param)
        {
            using (new LockWait(ref para))
            {
                bool value = false;
                foreach (var item in observes.Values)
                {
                    if (item.Publish(publisher, type, code, args, param))
                        value=true;
                }
                foreach (var item in observedels.Values)
                {
                    if (item.Publish(publisher, type, code, args, param))
                        value = true;
                }
                return value;
            }

        }

       
    }
    [FrameworkVersion(3)]
    public static class MessageMouduleExtension
    {
        public static bool Subscribe(this IMessageListener listener, Type type, int envIndex)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envIndex);
            return _env.modules.Message.Subscribe(type, listener);
        }
        public static bool Subscribe<T>(this IMessageListener listener, int envIndex) where T : IMessagePublisher
        {
            return Subscribe(listener, typeof(T), envIndex);
        }
        public static bool Unsubscribe(this IMessageListener listener, Type type, int envIndex)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envIndex);
            return _env.modules.Message.Unsubscribe(type, listener);
        }
        public static bool Unsubscribe<T>(this IMessageListener listener, int envIndex) where T : IMessagePublisher
        {
            return Unsubscribe(listener, typeof(T), envIndex);
        }
        public static bool Subscribe(this object obj, Type type, IMessageListener listener, int envIndex)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envIndex);
            return _env.modules.Message.Subscribe(type, listener);
        }
        public static bool Subscribe<T>(this object obj, IMessageListener listener, int envIndex) where T : IMessagePublisher
        {
            return Subscribe(obj, typeof(T), listener, envIndex);
        }
        public static bool Unsubscribe(this object obj, Type type, IMessageListener listener, int envIndex)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envIndex);
            return _env.modules.Message.Unsubscribe(type, listener);
        }
        public static bool Unsubscribe<T>(this object obj, IMessageListener listener, int envIndex) where T : IMessagePublisher
        {
            return Unsubscribe(obj, typeof(T), listener, envIndex);
        }


        public static bool Subscribe(this object obj, Type type, MessageLostener listener, int envIndex)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envIndex);
            return _env.modules.Message.Subscribe(type, listener);
        }
        public static bool Subscribe<T>(this object obj,  MessageLostener listener, int envIndex) where T : IMessagePublisher
        {
            return Subscribe(obj, typeof(T), listener, envIndex);
        }
        public static bool Unsubscribe(this object obj, Type type, MessageLostener listener,int envIndex)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envIndex);
            return _env.modules.Message.Unsubscribe(type, listener);
        }
        public static bool Unsubscribe<T>(this object obj, MessageLostener listener, int envIndex) where T : IMessagePublisher
        {
            return Unsubscribe(obj, typeof(T), listener, envIndex);
        }

        public static bool Publish(this object obj, int envIndex, Type type, int code, IEventArgs args, params object[] param)
        {
            return Publish(obj, envIndex, null, type, code, args, param);
        }
        public static bool Publish(this object obj, int envIndex, IMessagePublisher publisher, Type type, int code, IEventArgs args, params object[] param)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envIndex);
            return _env.modules.Message.Publish(publisher, type, code, args, param);
        }
        public static bool Publish(this object obj, int envIndex, IMessagePublisher publisher, int code, IEventArgs args, params object[] param)
        {
            return Publish(obj, envIndex, publisher, publisher.GetType(), code, args, param);
        }
        public static bool Publish<T>(this object obj, int envIndex, int code, IEventArgs args, params object[] param) where T : IMessagePublisher
        {
            return Publish(obj, envIndex, null, typeof(T), code, args, param);
        }
        public static bool Publish<T>(this object obj, int envIndex, T t, int code, IEventArgs args, params object[] param) where T : IMessagePublisher
        {
            return Publish(obj, envIndex, t, typeof(T), code, args, param);
        }
        public static bool Publish<T>(this T obj, int envIndex, int code, IEventArgs args, params object[] param) where T : IMessagePublisher
        {
            return Publish(obj, envIndex, obj, typeof(T), code, args, param);
        }

    }
}
