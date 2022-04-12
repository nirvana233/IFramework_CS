using System;
using System.Collections.Generic;
namespace IFramework.Modules.Message
{
    public partial class MessageModule
    {
        private partial class HandlerQueue : IDisposable
        {
            private Queue<DelegateSubscribe> _subscribeQueue = Framework.GlobalAllocate<Queue<DelegateSubscribe>>();
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
                    var s = Framework.GlobalAllocate<DelegateUnsubscribe>();
                    s.type = type;
                    s.value = listener;
                    _subscribeQueue.Enqueue(s);
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
                        if (value is DelegateUnsubscribe)
                        {
                            if (subjectMap.TryGetValue(type, out index))
                            {
                                subjects[subjectMap[type]].UnSubscribe(value.value);
                            }
                        }
                        else
                        {
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
                _subscribeQueue.GlobalRecyle();
                subjects.GlobalRecyle();
                subjectMap.GlobalRecyle();
            }
        }
    }
}
