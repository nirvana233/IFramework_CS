using System;
using System.Collections.Generic;

namespace IFramework
{
    /// <summary>
    /// 可回收集合
    /// </summary>
    [VersionAttribute(21)]
    [UpdateAttribute(20, "增加未回收实例的控制")]
    public class RecyclableObjectCollection : IDisposable
    {
        private class RecyclableObjectPool : BaseTypePool<RecyclableObject> { }

        private class RecyclableObjectMemory : IDisposable
        {
            private LockParam _lock = new LockParam();
            private Dictionary<Guid, RecyclableObject> _map;
            public RecyclableObjectMemory()
            {
                _map = new Dictionary<Guid, RecyclableObject>();
            }
            public void Dispose()
            {
                _map = null;
            }

            public void Set(RecyclableObject obj)
            {
                using (new LockWait(ref _lock))
                {
                    Guid id = obj.guid;
                    if (_map.ContainsKey(id))
                        throw new Exception("Same Key");
                    else
                        _map.Add(id, obj);
                }
            }

            public bool Exist(Guid id, out RecyclableObject obj)
            {
                using (new LockWait(ref _lock))
                {
                    return _map.TryGetValue(id, out obj);
                }
            }
            public void Remove(Guid id)
            {
                using (new LockWait(ref _lock))
                {
                    bool bo = _map.Remove(id);
                    if (!bo)
                        Log.E("Do not Exist ID  " + id);
                }
            }

            public Guid[] GetGuids()
            {
                using (new LockWait(ref _lock))
                {
                    Guid[] ids = new Guid[_map.Count];
                    int index = 0;
                    foreach (var item in _map.Keys)
                    {
                        ids[index++] = item;
                    }
                    return ids;
                }

            }
        }
        private RecyclableObjectPool _createPool;
        private RecyclableObjectMemory _memory;
        /// <summary>
        /// ctor
        /// </summary>
        public RecyclableObjectCollection()
        {
            _createPool = new RecyclableObjectPool();
            _memory = new RecyclableObjectMemory();
        }

        /// <summary>
        /// 获取一个实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public RecyclableObject Get(Type type, IEventArgs arg = null)
        {
            var obj = _createPool.Get(type, arg);
            _memory.Set(obj);
            return obj;
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Object Get<Object>(IEventArgs arg = null) where Object : RecyclableObject
        {
            Object t = _createPool.Get<Object>(arg);
            _memory.Set(t);
            return t;
        }

        /// <summary>
        /// 回收一个实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public void Set(Type type, RecyclableObject t, IEventArgs arg = null)
        {
            _memory.Remove(t.guid);
            _createPool.Set(t.GetType(), t, arg);
        }
        /// <summary>
        /// 回收一个实例
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public void Set<Object>(Object t, IEventArgs arg = null) where Object : RecyclableObject
        {
            Set(t.GetType(), t, arg);
        }

        /// <summary>
        /// 获取没有回收的实例
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool GetFromMemory(Guid id, out RecyclableObject obj)
        {
            return _memory.Exist(id, out obj);
        }

        /// <summary>
        /// 回收一个运行中的实例
        /// </summary>
        public void Recyle(Guid id)
        {
            RecyclableObject obj;
            bool bo = _memory.Exist(id, out obj);
            if (bo)
            {
                obj.Recyle();
            }
        }
        /// <summary>
        /// 回收所有运行中的实例
        /// </summary>
        public void RecyleAll()
        {
            var ids = _memory.GetGuids();
            for (int i = 0; i < ids.Length; i++)
            {
                Recyle(ids[i]);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            RecyleAll();
            _memory.Dispose();
            _createPool.Dispose();
        }
    }
}
