using System;
using System.Collections.Generic;

namespace IFramework
{
    /// <summary>
    /// 基础对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> : IDisposable
    {
        /// <summary>
        /// 清理数据时
        /// </summary>
        public event Action<T, IEventArgs> onClearObject;
        /// <summary>
        /// 获取数据时
        /// </summary>
        public event Action<T, IEventArgs> onGetObject;
        /// <summary>
        /// 当回收数据时
        /// </summary>
        public event Action<T, IEventArgs> onSetObject;
        /// <summary>
        /// 当清理创建数据时
        /// </summary>
        public event Action<T, IEventArgs> onCreateObject;
        /// <summary>
        /// 数据容器
        /// </summary>
        protected Queue<T> pool { get { return _lazy.Value; } }
        private Lazy<Queue<T>> _lazy=new Lazy<Queue<T>>(()=> { return new Queue<T>(); },true);
        /// <summary>
        /// 自旋锁
        /// </summary>
        protected LockParam lockParam;
        /// <summary>
        /// 存储数据类型
        /// </summary>
        public virtual Type type { get { return typeof(T); } }

        /// <summary>
        /// 池子数量
        /// </summary>
        public int count { get { return pool.Count; } }
        /// <summary>
        /// Ctor
        /// </summary>
        protected ObjectPool() { lockParam = new LockParam(); }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            _lazy = null;
            lockParam = null;
            onClearObject = null;
            onGetObject = null;
            onSetObject = null;
            onCreateObject = null;
        }
        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() {
            while (pool.Count>0)
            {
                IDisposable dispose = pool.Dequeue() as IDisposable;
                if (dispose != null)
                    dispose.Dispose();
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public T Get(IEventArgs arg = null)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                T t;
                if (pool.Count > 0)
                {
                    t = pool.Dequeue();
                }
                else
                {
                    t = CreatNew(arg);
                    OnCreate(t, arg);
                }
                OnGet(t, arg);
                return t;
            }
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool Set(T t, IEventArgs arg = null)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                if (!pool.Contains(t))
                {
                    if (OnSet(t, arg))
                    {
                        pool.Enqueue(t);
                    }
                    return true;
                }
                else
                {
                    Log.E("Set Err: Exist " + type);
                    return false;
                }
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="arg"></param>
        public void Clear(IEventArgs arg = null)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                while (pool.Count > 0)
                {
                    var t = pool.Dequeue();
                    OnClear(t, arg);
                }
            }
        }
        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="count"></param>
        /// <param name="arg"></param>
        public void Clear(int count, IEventArgs arg = null)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                count = count > pool.Count ? 0 : pool.Count - count;
                while (pool.Count > count)
                {
                    var t = pool.Dequeue();
                    OnClear(t, arg);
                }
            }
        }
        /// <summary>
        /// 创建一个新对象
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected abstract T CreatNew(IEventArgs arg);
        /// <summary>
        /// 数据被清除时
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        protected virtual void OnClear(T t, IEventArgs arg)
        {
            if (onClearObject != null) onClearObject(t, arg);

        }
        /// <summary>
        /// 数据被回收时，返回true可以回收
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual bool OnSet(T t, IEventArgs arg)
        {
            if (onSetObject != null) onSetObject(t, arg);
            return true;
        }
        /// <summary>
        /// 数据被获取时
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        protected virtual void OnGet(T t, IEventArgs arg)
        {
            if (onGetObject != null) onGetObject(t, arg);

        }
        /// <summary>
        /// 数据被创建时
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        protected virtual void OnCreate(T t, IEventArgs arg)
        {
            if (onCreateObject != null) onCreateObject(t, arg);
        }
    }
}
