
using System;
using System.Collections.Generic;

namespace IFramework
{
    public interface IEventArgs { }
    public interface IEventArgs<T> : IEventArgs { Type EventType { get; } }
    public interface IRecyclable
    {
        void Recyle();
        void ResetData();
    }
    public abstract class RecyclableObject : IRecyclable
    {
        private bool _recyled;
        private bool _datadirty;
        private FrameworkEnvironment _env;

        public bool recyled { get { return _recyled; } }
        public bool dataDirty { get { return _datadirty; } }
        public FrameworkEnvironment env { get { return _env; } set { _env = value; } }

        public static RecyclableObject Allocate(Type type, int envIndex) 
        {
            FrameworkEnvironment _env = Framework.GetEnv(envIndex);
            return Allocate(type,_env);
        }
        public static RecyclableObject Allocate(Type type, FrameworkEnvironment env)
        {
            RecyclableObject t = env.cyclePool.Allocate(type) as RecyclableObject;
            t._env = env;
            t.OnAllocate();
            return t;
        }

        public static T Allocate<T>(int envIndex) where T : RecyclableObject
        {
            FrameworkEnvironment _env = Framework.GetEnv(envIndex);
            return Allocate<T>(_env);
        }
        public static T Allocate<T>(FrameworkEnvironment env) where T : RecyclableObject
        {
            T t = env.cyclePool.Allocate<T>();
            t._env = env;
            t.OnAllocate();
            return t;
        }

        protected virtual void OnAllocate() {
            _recyled = false;
            ResetData();
        }
        public void Recyle()
        {
            if (_recyled) return;
            OnRecyle();
            _recyled = true;
            _env.cyclePool.Recyle(this);
        }

        public void ResetData()
        {
            if (!_datadirty) return;
            OnDataReset();
            _datadirty = false;
        }

        public void SetDataDirty()
        {
            _datadirty = true;
        }

        protected virtual void OnRecyle() { }
        protected abstract void OnDataReset();
    }
    public abstract class FrameworkArgs : RecyclableObject, IEventArgs
    {
        public static FrameworkArgs Empty;

        private bool _argsDirty;

        public bool argsDirty { get { return _argsDirty; }set { _argsDirty = value; } }
        protected override void OnAllocate()
        {
            base.OnAllocate();
            _argsDirty = false;
        }
    }

    public class RecyclableObjectPool : IDisposable
    {
        interface IFrameworkObjectInnerPool { }
        class FrameworkObjectInnerPool<Object> : ObjectPool<Object>, IFrameworkObjectInnerPool where Object : IRecyclable
        {
            public  FrameworkObjectInnerPool(Type objType)
            {
                ObjType = objType;
            }
                
            private Type ObjType;
            protected override Object CreatNew(IEventArgs arg, params object[] param)
            {
                return (Object)Activator.CreateInstance(ObjType);
            }
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

        private Dictionary<Type, IFrameworkObjectInnerPool> poolMap;
        private LockParam para = new LockParam();
        public RecyclableObjectPool()
        {
            poolMap = new Dictionary<Type, IFrameworkObjectInnerPool>();
        }
        public void Dispose()
        {
            using (new LockWait(ref para))
            {
                foreach (var item in poolMap.Values)
                    (item as FrameworkObjectInnerPool<IRecyclable>).Dispose();
                poolMap.Clear();
            }
           
        }

        private FrameworkObjectInnerPool<IRecyclable> GetPool(Type type)
        {
            using (new LockWait(ref para))
            {
                if (!poolMap.ContainsKey(type))
                    poolMap.Add(type, new FrameworkObjectInnerPool<IRecyclable>(type));
                return poolMap[type] as FrameworkObjectInnerPool<IRecyclable>;
            }
        }

        public IRecyclable Allocate(Type type)
        {
            IRecyclable recyclable = GetPool(type).Get();
            return recyclable;
        }
        public T Allocate<T>() where T : IRecyclable
        {
            
            T t = (T)GetPool(typeof(T)).Get();
           
            return t;
        }
        public void Recyle(Type type,IRecyclable t)
        {
            GetPool(type).Set(t);
        }
        public void Recyle<T>(T t) where T : IRecyclable
        {
            Recyle(t.GetType(), t);
        }
    }
}
