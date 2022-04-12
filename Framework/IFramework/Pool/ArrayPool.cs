using System.Collections.Generic;

namespace IFramework
{
    /// <summary>
    /// ArrayPoolArg
    /// </summary>
    public class ArrayPoolArg : IEventArgs
    {
        /// <summary>
        /// 长度
        /// </summary>
        public int length;
    }
    /// <summary>
    /// 数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayPool<T> : ObjectPool<T[]>
    {
        private Queue<int> __lengthqueue;
        private Queue<int> _lengthqueue
        {
            get
            {
                if (__lengthqueue == null)
                {
                    __lengthqueue = Framework.GlobalAllocate<Queue<int>>();
                }
                return __lengthqueue;
            }
        }
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected override T[] CreatNew(IEventArgs arg)
        {
            ArrayPoolArg len = arg as ArrayPoolArg;
            if (len != null) return new T[len.length];
            return null;
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override T[] Get(IEventArgs arg = null)
        {
            ArrayPoolArg len = arg as ArrayPoolArg;
            if (len == null) return null;
            int length = len.length;
            using (LockWait wait = new LockWait(ref lockParam))
            {
                T[] t;
                if (pool.Count > 0 && _lengthqueue.Contains(length))
                {
                    Queue<T[]> queue = Framework.GlobalAllocate<Queue<T[]>>();
                    while (_lengthqueue.Peek() != length)
                    {
                        _lengthqueue.Dequeue();
                        queue.Enqueue(pool.Dequeue());
                    }
                    t = pool.Dequeue();
                    while (pool.Count != 0) queue.Enqueue(pool.Dequeue());
                    int _count = queue.Count;
                    for (int i = 0; i < _count; i++)
                    {
                        var tmp = queue.Dequeue();
                        int _len = tmp.Length;
                        _lengthqueue.Enqueue(_len);
                        pool.Enqueue(tmp);
                    }
                    Framework.GlobalRecyle(queue);
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
        public override bool Set(T[] t, IEventArgs arg = null)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                if (!pool.Contains(t))
                {
                    if (OnSet(t, arg))
                    {
                        int _len = t.Length;
                        _lengthqueue.Enqueue(_len);
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
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();
            if (_lengthqueue!=null)
            {
                _lengthqueue.Clear();
                _lengthqueue.GlobalRecyle();
            }
        }
    }
}
