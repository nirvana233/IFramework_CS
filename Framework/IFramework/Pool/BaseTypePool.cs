using System;
using System.Collections.Generic;
using System.Reflection;

namespace IFramework
{
    /// <summary>
    /// 统一类型的对象池
    /// </summary>
    /// <typeparam name="T">基础类型</typeparam>
    public abstract class BaseTypePool<T> : IDisposable
    {
        private Dictionary<Type, IObjectPool> _poolMap;

        private MethodInfo _getMethodInfo;
        private MethodInfo _setMethodInfo;
        private Dictionary<Type, MethodInfo> _getpools;
        private Dictionary<Type, MethodInfo> _setpools;

        /// <summary>
        /// 自旋锁
        /// </summary>
        protected LockParam para = new LockParam();

        /// <summary>
        /// Ctor
        /// </summary>
        public BaseTypePool()
        {
            _poolMap = new Dictionary<Type, IObjectPool>();
            _getMethodInfo = GetType().GetMethod(nameof(Get), new Type[] { typeof(IEventArgs) });
            _setMethodInfo = GetType().GetMethod(nameof(Set), new Type[] { typeof(IEventArgs) });
            _getpools = new Dictionary<Type, MethodInfo>();
            _setpools = new Dictionary<Type, MethodInfo>();
        }
        /// <summary>
        /// 设置内部对象池
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="pool"></param>
        public void SetPool<Object>(ObjectPool<Object> pool) where Object : T
        {
            SetPool(typeof(Object), pool);
        }
        /// <summary>
        /// 获取内部对象池
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        public ObjectPool<Object> GetPool<Object>() where Object : T
        {
            Type type = typeof(Object);
            var pool = GetPool(type);
            return pool as ObjectPool<Object>;
        }
        /// <summary>
        /// 设置内部对象池
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pool"></param>
        public void SetPool(Type type, IObjectPool pool)
        {
            using (new LockWait(ref para))
            {
                if (!_poolMap.ContainsKey(type))
                    _poolMap.Add(type, pool);
                else
                    _poolMap[type] = pool;
            }
        }
        /// <summary>
        /// 获取内部对象池
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IObjectPool GetPool(Type type)
        {
            using (new LockWait(ref para))
            {
                IObjectPool pool;
                if (!_poolMap.TryGetValue(type, out pool))
                {
                    pool = CreatePool(type);
                    if (pool == null)
                    {
                        var pooType = typeof(ActivatorCreatePool<>).MakeGenericType(type);
                        pool = Activator.CreateInstance(pooType, null) as IObjectPool;
                    }
                    _poolMap.Add(type, pool);
                }
                return pool;
            }
        }
        /// <summary>
        /// 創建对象池
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual IObjectPool CreatePool(Type type)
        {
            return null;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        [Tip("少用,内部反射")]
        public T Get(Type type, IEventArgs arg = null)
        {
            MethodInfo m2;
            if (!_getpools.TryGetValue(type, out m2))
            {
                m2 = _getMethodInfo.MakeGenericMethod(type);
                _getpools.Add(type, m2);
            }
            return (T)m2.Invoke(this, new object[] { arg });
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Object Get<Object>(IEventArgs arg = null) where Object : T
        {
            var pool = GetPool<Object>();

            Object t = pool.Get(arg);
            return t;
        }
        /// <summary>
        /// 回收数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        [Tip("少用,内部反射")]
        public void Set(Type type, T t, IEventArgs arg = null)
        {
            MethodInfo m2;
            if (!_setpools.TryGetValue(type, out m2))
            {
                m2 = _setMethodInfo.MakeGenericMethod(type);
                _setpools.Add(type, m2);
            }
            m2.Invoke(this, new object[] { t, arg });
        }
        /// <summary>
        /// 回收数据
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public void Set<Object>(Object t, IEventArgs arg = null) where Object : T
        {
            var pool = GetPool<Object>();
            pool.Set(t, arg);
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
                    (item as IObjectPool).Dispose();
                _poolMap.Clear();
                _getpools.Clear();
                _setpools.Clear();
            }
        }
        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() { }
    }

}
