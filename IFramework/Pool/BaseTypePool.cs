using System;
using System.Collections.Generic;

namespace IFramework
{
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
        public T Get(Type type, IEventArgs arg = null)
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
            GetPool(type).Set(t, arg);
        }
        /// <summary>
        /// 回收数据
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <param name="t"></param>
        /// <param name="arg"></param>
        public void Set<Object>(Object t, IEventArgs arg = null) where Object : T
        {
            Set(t.GetType(), t, arg);
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
