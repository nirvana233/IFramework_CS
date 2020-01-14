using System;
using System.Collections.Generic;

namespace IFramework.Moudles.Message
{
    public class MessageMoudle : FrameworkMoudle, IMessageMoudle
    {
        private interface IObserve : IDisposable
        {
            Type ObserveType { get; }
            bool Subscribe(IObserver observer);
            bool Unsubscribe(IObserver observer);
            bool Publish(IPublisher publisher, Type type, int code, IEventArgs args, params object[] param);
        }
        private class Observe : IObserve
        {
            private LockParam para = new LockParam();
            private readonly Type observeType;
            private IList<Type> typeTree;

            public Type ObserveType { get { return observeType; } }

            public Observe(Type observeType) {
                typeTree = observeType.GetTypeTree();

                this.observeType = observeType;
            }
            private List<IObserver> observers = new List<IObserver>();
            public bool Subscribe(IObserver observer)
            {
                using (new LockWait(ref para))
                {
                    if (observers.Contains(observer)) return false;
                    else observers.Add(observer); return true;
                }
            }
            public bool Unsubscribe(IObserver observer)
            {
                using (new LockWait(ref para))
                {
                    if (!observers.Contains(observer)) return false;
                    else observers.Remove(observer); return true;
                }
            }

            public bool Publish(IPublisher publisher, Type type, int code, IEventArgs args, params object[] param)
            {
                using (new LockWait(ref para))
                {
                    if (!type.IsSubClassOfInterface(observeType) &&
                        !type.IsSubclassOf(observeType) && 
                        type != observeType && 
                        !typeTree.Contains(type)) return false;

                    observers.ForEach((observer) => {
                        if (observer != null) observer.Listen(publisher, type, code, args, param);
                        else observers.Remove(observer);
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
        private Dictionary<Type, Observe> observes;

        private Queue<Action> delayPublish;
        private Queue<Action> tmpDelayPublish;



        protected override void OnDispose()
        {
            delayPublish.Clear();
            tmpDelayPublish.Clear();
            tmpDelayPublish = null;
            delayPublish = null;
            foreach (var item in observes.Values)
                item.Dispose();
            observes.Clear();
            observes = null;
        }
        protected override void Awake()
        {
            observes = new Dictionary<Type, Observe>();
            delayPublish = new Queue<Action>();
            tmpDelayPublish = new Queue<Action>();
        }
        protected override void OnUpdate()
        {
            if (delayPublish.Count == 0) return;
            while (delayPublish.Count > 0)
                tmpDelayPublish.Enqueue(delayPublish.Dequeue());
            while (tmpDelayPublish.Count > 0)
            {
                var action = tmpDelayPublish.Dequeue();
                if (action != null)
                    action();
            }
        }

        public bool Subscribe(Type type, IObserver observer)
        {
            using (new LockWait(ref para))
            {
                if (!observes.ContainsKey(type))
                    observes.Add(type, new Observe(type));
                return observes[type].Subscribe(observer);
            }
        }
        public bool Subscribe<T>(IObserver observer) where T : IPublisher
        {
            return Subscribe(typeof(T), observer);
        }
        public bool Unsubscribe(Type type, IObserver observer)
        {
            using (new LockWait(ref para))
            {
                if (!observes.ContainsKey(type)) return false;
                return observes[type].Unsubscribe(observer);
            }
        }
        public bool Unsubscribe<T>(IObserver observer) where T : IPublisher
        {
            return Unsubscribe(typeof(T), observer);
        }

        public bool Publish<T>(int code, IEventArgs args, params object[] param) where T : IPublisher
        {
            return Publish(typeof(T), code, args, param);
        }
        public bool Publish<T>(T t, int code, IEventArgs args, params object[] param) where T : IPublisher
        {
            return Publish(t, typeof(T), code, args, param);
        }
        public bool Publish(Type type, int code, IEventArgs args, params object[] param)
        {
            return Publish(null, type, code, args, param);
        }
        public bool Publish(IPublisher publisher, Type type, int code, IEventArgs args, params object[] param)
        {
            using (new LockWait(ref para))
            {
                foreach (var item in observes.Values)
                {
                    if (item.Publish(publisher, type, code, args, param))
                        return true;
                }
                return false;
            }

        }

        public void DelayPublish<T>(int code, IEventArgs args, params object[] param) where T : IPublisher
        {
            DelayPublish(typeof(T), code, args, param);
        }
        public void DelayPublish<T>(T t, int code, IEventArgs args, params object[] param) where T : IPublisher
        {
            DelayPublish(t, typeof(T), code, args, param);
        }
        public void DelayPublish(Type type, int code, IEventArgs args, params object[] param)
        {
            DelayPublish(null, type, code, args, param);
        }
        public void DelayPublish(IPublisher publisher, Type type, int code, IEventArgs args, params object[] param)
        {
            delayPublish.Enqueue(() => {
                Publish(publisher, type, code, args, param);
            });
        }
    }

}
