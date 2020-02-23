
using System;
using EnvironmentType = IFramework.FrameworkEnvironment.EnvironmentType;

namespace IFramework
{
    /// <summary>
    /// 框架内传递的所有消息的基类
    /// </summary>
    public interface IEventArgs { }
    /// <summary>
    /// 可回收接口
    /// </summary>
    public interface IRecyclable
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        void Recyle();
        void ResetData();
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
    /// <summary>
    /// 可回收类
    /// </summary>
    public abstract class RecyclableObject : IRecyclable
    {
        private bool _recyled;
        private bool _datadirty;
        private FrameworkEnvironment _env;
        /// <summary>
        /// 是否被回收
        /// </summary>
        public bool recyled { get { return _recyled; } }
        /// <summary>
        /// 数据是否发生改变
        /// </summary>
        public bool dataDirty { get { return _datadirty; } }
        /// <summary>
        /// 当前所处环境
        /// </summary>
        public FrameworkEnvironment env { get { return _env; } set { _env = value; } }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name=" envType"></param>
        /// <returns></returns>
        public static RecyclableObject Allocate(Type type, EnvironmentType envType) 
        {
            FrameworkEnvironment _env = Framework.GetEnv( envType);
            return Allocate(type,_env);
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static RecyclableObject Allocate(Type type, FrameworkEnvironment env)
        {
            RecyclableObject t = env.cyclePool.Get(type) as RecyclableObject;
            t._env = env;
            t.OnAllocate();
            return t;
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <typeparam name="T"> RecyclableObject </typeparam>
        /// <param name=" envType"></param>
        /// <returns></returns>
        public static T Allocate<T>(EnvironmentType envType) where T : RecyclableObject
        {
            FrameworkEnvironment _env = Framework.GetEnv( envType);
            return Allocate<T>(_env);
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="env"></param>
        /// <returns></returns>
        public static T Allocate<T>(FrameworkEnvironment env) where T : RecyclableObject
        {
            T t = env.cyclePool.Get<T>();
            t._env = env;
            t.OnAllocate();
            return t;
        }
        /// <summary>
        /// 被分配时
        /// </summary>
        protected virtual void OnAllocate() {
            _recyled = false;
            ResetData();
        }
        /// <summary>
        /// 回收
        /// </summary>
        public void Recyle()
        {
            if (_recyled) return;
            OnRecyle();
            _recyled = true;
            _env.cyclePool.Set(this);
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        public void ResetData()
        {
            if (!_datadirty) return;
            OnDataReset();
            _datadirty = false;
        }
        /// <summary>
        /// 设置数据发生改动
        /// </summary>
        public void SetDataDirty()
        {
            _datadirty = true;
        }
        /// <summary>
        /// 被回收时
        /// </summary>
        protected virtual void OnRecyle() { }
        /// <summary>
        /// 数据重置时
        /// </summary>
        protected abstract void OnDataReset();
    }
    /// <summary>
    /// 可回收消息基类
    /// </summary>
    public abstract class FrameworkArgs : RecyclableObject, IEventArgs
    {
        private bool _argsDirty;
        /// <summary>
        /// 消息是否可用
        /// </summary>
        public bool argsDirty { get { return _argsDirty; }set { _argsDirty = value; } }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override void OnAllocate()
        {
            base.OnAllocate();
            _argsDirty = false;
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class RecyclableObjectPool : BaseTypePool<IRecyclable> { }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    
    ///// <summary>
    ///// IRecyclable容器
    ///// </summary>
    //public class RecyclableObjectPool : IDisposable
    //{
    //    interface IFrameworkObjectInnerPool { }
    //    class FrameworkObjectInnerPool<Object> : ObjectPool<Object>, IFrameworkObjectInnerPool where Object : IRecyclable
    //    {
    //        public  FrameworkObjectInnerPool(Type objType)
    //        {
    //            ObjType = objType;
    //        }
                
    //        private Type ObjType;
    //        protected override Object CreatNew(IEventArgs arg)
    //        {
    //            return (Object)Activator.CreateInstance(ObjType);
    //        }
    //        protected override void OnDispose()
    //        {
    //            base.OnDispose();
               

    //            for (int i = 0; i < pool.Count; i++)
    //            {
    //                IDisposable dispose = pool[i] as IDisposable;
    //                if (dispose != null)
    //                    dispose.Dispose();
    //            }
    //        }

    //    }

    //    private Dictionary<Type, IFrameworkObjectInnerPool> poolMap;
    //    private LockParam para = new LockParam();

    //    internal RecyclableObjectPool()
    //    {
    //        poolMap = new Dictionary<Type, IFrameworkObjectInnerPool>();
    //    }
    //    /// <summary>
    //    /// 释放
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        using (new LockWait(ref para))
    //        {
    //            foreach (var item in poolMap.Values)
    //                (item as FrameworkObjectInnerPool<IRecyclable>).Dispose();
    //            poolMap.Clear();
    //        }
           
    //    }

    //    private FrameworkObjectInnerPool<IRecyclable> GetPool(Type type)
    //    {
    //        using (new LockWait(ref para))
    //        {
    //            if (!poolMap.ContainsKey(type))
    //                poolMap.Add(type, new FrameworkObjectInnerPool<IRecyclable>(type));
    //            return poolMap[type] as FrameworkObjectInnerPool<IRecyclable>;
    //        }
    //    }
    //    /// <summary>
    //    /// 分配 一个 IRecyclable
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    public IRecyclable Allocate(Type type)
    //    {
    //        IRecyclable recyclable = GetPool(type).Get();
    //        return recyclable;
    //    }
    //    /// <summary>
    //    /// 分配 一个 IRecyclable
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    public T Allocate<T>() where T : IRecyclable
    //    {
            
    //        T t = (T)GetPool(typeof(T)).Get();
           
    //        return t;
    //    }
    //    /// <summary>
    //    /// 回收一个 IRecyclable
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <param name="t"></param>
    //    public void Recyle(Type type,IRecyclable t)
    //    {
    //        GetPool(type).Set(t);
    //    }
    //    /// <summary>
    //    /// 回收一个 IRecyclable
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="t"></param>
    //    public void Recyle<T>(T t) where T : IRecyclable
    //    {
    //        Recyle(t.GetType(), t);
    //    }
    //}

}
