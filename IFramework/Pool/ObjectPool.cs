using System;
using System.Collections.Generic;

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
    /// <summary>
    /// 有容量的对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CapicityPool<T>: ObjectPool<T>
    {
        private int _capcity;
        /// <summary>
        /// 存储容量
        /// </summary>
        public int capcity { get { return _capcity; } set { _capcity = value; } }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="capcity"></param>
        protected CapicityPool(int capcity) : base() { this._capcity = capcity; }
        /// <summary>
        /// 回收，当数量超过回收失败
        /// </summary>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected override bool OnSet(T t, IEventArgs arg)
        {
             base.OnSet(t, arg);
            return count <= capcity;
        }
    }
    /// <summary>
    /// 基础对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> : IDisposable
    {
        /// <summary>
        /// 清理数据时
        /// </summary>
        public event Action<T,IEventArgs> onClearObject;
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
        protected List<T> pool;
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
        protected ObjectPool() { pool = new List<T>(); lockParam = new LockParam(); }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {

            OnDispose();
            Clear();
            pool = null;
            lockParam = null;
            onClearObject = null;
            onGetObject = null;
            onSetObject = null;
            onCreateObject = null;
        }
        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() { }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="param"></param>
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
    /// <summary>
    /// 统一类型的对象池
    /// </summary>
    /// <typeparam name="T">基础类型</typeparam>
    public abstract class BaseTypePool<T> : IDisposable
    {
        interface IBaseTypeInnerPool { }
        /// <summary>
        /// 内部池子
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        public class BaseTypeInnerPool<Object> : ObjectPool<Object>, IBaseTypeInnerPool where Object : T
        {
            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="objType"></param>
            public BaseTypeInnerPool(Type objType)
            {
                this.objType = objType;
            }

            private Type objType;
            /// <summary>
            /// 池子内部实际对象类型
            /// </summary>
            public override Type type { get { return objType; } }
            /// <summary>
            /// 创建实例
            /// </summary>
            /// <param name="arg"></param>
            /// <returns></returns>
            protected override Object CreatNew(IEventArgs arg)
            {
                return (Object)Activator.CreateInstance(objType);
            }
            /// <summary>
            /// 释放时
            /// </summary>
            protected override void OnDispose()
            {
                base.OnDispose();


                for (int i = 0; i < pool.Count; i++)
                {
                    IDisposable dispose = pool[i] as IDisposable;
                    if (dispose != null)
                        dispose.Dispose();
                }
            }

        }
        private Dictionary<Type, IBaseTypeInnerPool> _poolMap;
        /// <summary>
        /// 自旋锁
        /// </summary>
        protected LockParam para = new LockParam();
        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BaseTypeInnerPool<T> this[Type type]
        {
            get { return GetPool(type); }
            set { SetPool(type, value); }
        }
        /// <summary>
        /// Ctor
        /// </summary>
        public BaseTypePool()
        {
            _poolMap = new Dictionary<Type, IBaseTypeInnerPool>();
        }
        /// <summary>
        /// 设置内部对象池
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pool"></param>
        public virtual void SetPool(Type type, BaseTypeInnerPool<T> pool)
        {
            if (!_poolMap.ContainsKey(type))
                _poolMap.Add(type, pool);
            else
                _poolMap[type] = pool;
        }
        /// <summary>
        /// 获取内部对象池
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual BaseTypeInnerPool<T> GetPool(Type type)
        {
            using (new LockWait(ref para))
            {
                if (!_poolMap.ContainsKey(type))
                    _poolMap.Add(type, new BaseTypeInnerPool<T>(type));
                return _poolMap[type] as BaseTypeInnerPool<T>;
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public T Get(Type type,IEventArgs arg=null)
        {
            T recyclable = GetPool(type).Get(arg);
            return recyclable;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Object Get<Object>(IEventArgs arg = null) where Object : T
        {
            Object t = (Object)GetPool(typeof(Object)).Get(arg);
            return t;
        }
        /// <summary>
        /// 回收数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public void Set(Type type, T t, IEventArgs arg = null)
        {
            GetPool(type).Set(t,arg);
        }
        /// <summary>
        /// 回收数据
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public void Set<Object>(Object t, IEventArgs arg = null) where Object : T
        {
            Set(t.GetType(), t,arg);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            using (new LockWait(ref para))
            {
                OnDispose();
                foreach (var item in _poolMap.Values)
                    (item as BaseTypeInnerPool<T>).Dispose();
                _poolMap.Clear();
            }
        }
        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() { }
    }

}
