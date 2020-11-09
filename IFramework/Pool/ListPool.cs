using System;

namespace IFramework
{
    /// <summary>
    /// list对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ListPool<T> : ObjectPool<T>
    {
        /// <summary>
        /// ctor
        /// </summary>
        protected ListPool() : base() { }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="p"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public T Get(Predicate<T> p, IEventArgs arg = null)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                var ts = pool.FindAll(p);
                if (ts.Count < 0)
                {
                    T t = ts[0];
                    pool.Remove(t);
                    OnGet(t, arg);
                    return t;
                }
                return default(T);
            }
        }
        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool Clear(T t, IEventArgs arg = null)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                if (pool.Contains(t))
                {
                    pool.Remove(t);
                    OnClear(t, arg);
                    return true;
                }
                else
                {
                    Log.E("Clear Err: Not Exist " + type);
                    return false;
                }
            }
        }
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Contains(T t)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                return pool.Contains(t);
            }
        }
        /// <summary>
        /// 遍历
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<T> action)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                pool.ForEach(action);
            }
        }
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Predicate<T> p)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                var list = pool.FindAll(p);
                return list != null && list.Count > 0;
            }
        }
        /// <summary>
        /// 获取第一个元素
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                return pool.QueuePeek();
            }
        }
    }

}
