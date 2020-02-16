using System;
using System.Collections.Generic;

namespace IFramework
{
    internal interface IPoolObjectPool { }
    public delegate Object PoolObjCreaterDel<Object>(Type type, IEventArgs arg, params object[] param) where Object : IPoolObject;
    public delegate IPoolObject PoolObjCreaterDel(Type type, IEventArgs arg, params object[] param);
    public class PoolObjectPool: CachePool<IPoolObject>, IPoolObjectPool, IDisposable 
    {
        public PoolObjectPool() : this(new RunningPool<IPoolObject>(), new PoolObjectSleepingPool(), true, 16) { }
        public PoolObjectPool(PoolObjectSleepingPool sleepPool) : this(new RunningPool<IPoolObject>(), sleepPool, true, 16) { }
        public PoolObjectPool(RunningPool<IPoolObject> runningPoool, PoolObjectSleepingPool sleepPool) : this(runningPoool, sleepPool, true, 16) { }
        public PoolObjectPool(RunningPool<IPoolObject> runningPoool, PoolObjectSleepingPool sleepPool, bool autoClear, int cacheCapcity) : base(runningPoool, sleepPool, autoClear, cacheCapcity)
        {
        }

        public void AddCreater(PoolObjCreaterDel del) { (sleepPool as PoolObjectSleepingPool).AddCreaterDel(del); }
        public class PoolObjectSleepingPool : SleepingPool<IPoolObject> 
        {
            private List<PoolObjCreaterDel> CreaterDels;
            public void AddCreaterDel(PoolObjCreaterDel createrDel)
            {
                CreaterDels.Add(createrDel);
            }
            protected override void OnClear(IPoolObject t, IEventArgs arg)
            {
                base.OnClear(t, arg);
                t.OnClear(arg);
            }
            protected override void OnCreate(IPoolObject t, IEventArgs arg)
            {
                base.OnCreate(t, arg);
                t.OnCreate(arg);
            }
            protected override void OnGet(IPoolObject t, IEventArgs arg)
            {
                base.OnGet(t, arg);
                t.OnGet(arg);
            }
            protected override bool OnSet(IPoolObject t, IEventArgs arg)
            {
                base.OnSet(t, arg);
                t.OnSet(arg);
                return true;
            }
            protected override void OnDispose()
            {
                CreaterDels.Clear();
                CreaterDels = null;
                base.OnDispose();
            }
            protected override IPoolObject CreatNew(IEventArgs arg)
            {
                IPoolObject t = default(IPoolObject);
                bool have = false;
                for (int i = 0; i < CreaterDels.Count; i++)
                {
                    var del = CreaterDels[i];
                    var o = del.Invoke(type, arg);
                    if (o != null)
                    {
                        t = o;
                        have = true; break;
                    }
                }
                if (!have) t = (IPoolObject)Activator.CreateInstance(type);
                return t;
            }

            public PoolObjectSleepingPool() { CreaterDels = new List<PoolObjCreaterDel>(); }
        }
    }
    public class PoolObjectPool<T> : CachePool<T>, IPoolObjectPool, IDisposable where T : IPoolObject
    {
        public PoolObjectPool() : this(new RunningPool<T>(), new PoolObjectSleepingPool<T>(), true, 16) { }
        public PoolObjectPool(PoolObjectSleepingPool<T> sleepPool) : this(new RunningPool<T>(), sleepPool, true, 16) { }
        public PoolObjectPool(RunningPool<T> runningPoool, PoolObjectSleepingPool<T> sleepPool) : this(runningPoool, sleepPool, true, 16) { }
        public PoolObjectPool(RunningPool<T> runningPoool, PoolObjectSleepingPool<T> sleepPool, bool autoClear, int cacheCapcity) : base(runningPoool, sleepPool, autoClear, cacheCapcity)
        {
        }

        public void AddCreater(PoolObjCreaterDel<T> del) { (sleepPool as PoolObjectSleepingPool<T>).AddCreaterDel(del); }



        public class PoolObjectSleepingPool<Object> : SleepingPool<Object> where Object : IPoolObject
        {
            private List<PoolObjCreaterDel<Object>> CreaterDels;
            public void AddCreaterDel(PoolObjCreaterDel<Object> createrDel)
            {
                CreaterDels.Add(createrDel);
            }
            protected override void OnClear(Object t, IEventArgs arg)
            {
                base.OnClear(t, arg);
                t.OnClear(arg);
            }
            protected override void OnCreate(Object t, IEventArgs arg)
            {
                base.OnCreate(t, arg);
                t.OnCreate(arg);
            }
            protected override void OnGet(Object t, IEventArgs arg)
            {
                base.OnGet(t, arg);
                t.OnGet(arg);
            }
            protected override bool OnSet(Object t, IEventArgs arg)
            {
                base.OnSet(t, arg);
                t.OnSet(arg);
                return true;
            }
            protected override void OnDispose()
            {
                CreaterDels.Clear();
                CreaterDels = null;
                base.OnDispose();
            }
            protected override Object CreatNew(IEventArgs arg)
            {
                Object t = default(Object);
                bool have = false;
                for (int i = 0; i < CreaterDels.Count; i++)
                {
                    var o = CreaterDels[i].Invoke(type, arg);
                    if (o != null)
                    {
                        t = o;
                        have = true; break;
                    }
                }
                if (!have) t = (Object)Activator.CreateInstance(type);
                return t;
            }

            public PoolObjectSleepingPool() { CreaterDels = new List<PoolObjCreaterDel<Object>>(); }
        }
    }
}
