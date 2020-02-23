using System;

namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class RunningPool<T> : ListPool<T>
    {
        protected override T CreatNew(IEventArgs arg)
        {
            return default(T);
        }
    }
    public abstract class SleepingPool<T> : ListPool<T>
    {
    }

    public class CachePool<T> : IDisposable
    {
        public Type Type { get { return typeof(T); } }
        public bool AutoClear { get; set; }
        public int SleepCapcity { get; set; }

        protected SleepingPool<T> sleepPool;
        protected RunningPool<T> runningPoool;
        public virtual RunningPool<T> RunningPoool { get { return runningPoool; } set { runningPoool = value; } }
        public virtual SleepingPool<T> SleepPool { get { return sleepPool; } set { sleepPool = value; } }

        public int SleepCount { get { return SleepPool.count; } }
        public int RunningCount { get { return RunningPoool.count; } }

        public event Action<T, IEventArgs> OnRunningPoolClearObject { add { runningPoool.onClearObject += value; } remove { runningPoool.onClearObject -= value; } }
        public event Action<T, IEventArgs> OnRunningPoolGetObject { add { runningPoool.onGetObject += value; } remove { runningPoool.onGetObject -= value; } }
        public event Action<T, IEventArgs> OnRunningPoolSetObject { add { runningPoool.onSetObject += value; } remove { runningPoool.onSetObject -= value; } }
        public event Action<T, IEventArgs> OnRunningPoolCreateObject { add { runningPoool.onCreateObject += value; } remove { runningPoool.onCreateObject -= value; } }

        public event Action<T, IEventArgs> OnClearObject { add { SleepPool.onClearObject += value; } remove { SleepPool.onClearObject -= value; } }
        public event Action<T, IEventArgs> OnGetObject { add { SleepPool.onGetObject += value; } remove { SleepPool.onGetObject -= value; } }
        public event Action<T, IEventArgs> OnSetObject { add { SleepPool.onSetObject += value; } remove { SleepPool.onSetObject -= value; } }
        public event Action<T, IEventArgs> OnCreateObject { add { SleepPool.onCreateObject += value; } remove { SleepPool.onCreateObject -= value; } }

        public CachePool(SleepingPool<T> sleepPool) : this(new RunningPool<T>(), sleepPool, true, 16) { }
        public CachePool(RunningPool<T> runningPoool, SleepingPool<T> sleepPool) : this(runningPoool, sleepPool, true, 16) { }
        public CachePool(RunningPool<T> runningPoool, SleepingPool<T> sleepPool, bool autoClear, int sleepCapcity)
        {
            this.AutoClear = autoClear;
            this.SleepCapcity = sleepCapcity;
            this.RunningPoool = runningPoool;
            this.SleepPool = sleepPool;
        }
        public virtual void Dispose()
        {
            CycleRunningPool(null);
            SleepPool.Dispose();
            RunningPoool.Dispose();
        }

        public bool IsRunning(T t) { return RunningPoool.Contains(t); }
        public bool IsSleep(T t) { return SleepPool.Contains(t); }
        public bool Contains(T t) { return IsRunning(t) || IsSleep(t); }


        public T Get()
        {
            T t = SleepPool.Get();
            RunningPoool.Set(t);
            return t;
        }
        public void Set(T t)
        {
            RunningPoool.Clear(t);
            SleepPool.Set(t);
            AutoClean();
        }
        public T Get(IEventArgs arg)
        {
            T t = SleepPool.Get(arg);
            RunningPoool.Set(t);
            return t;
        }
        public void Set(T t, IEventArgs arg)
        {
            RunningPoool.Clear(t);
            SleepPool.Set(t, arg);
            AutoClean();
        }
        private void AutoClean()
        {
            if (!AutoClear) return;
            SleepPool.Clear(SleepPool.count - SleepCapcity);
        }

        public void Clear(T t, bool ignoreRun = true)
        {
            if (IsRunning(t))
            {
                if (ignoreRun) return;
                Set(t);
            }
            SleepPool.Clear(t);
        }
        public void Clear(T[] ts, bool ignoreRun = true)
        {
            ts.ForEach((t) =>
            {
                Clear(t, ignoreRun);
            });
        }
        public void Clear(T t, IEventArgs arg, bool ignoreRun = true)
        {
            if (IsRunning(t))
            {
                if (ignoreRun) return;
                Set(t, arg);
            }
            SleepPool.Clear(t, arg);
        }
        //public void Clear(T[] ts, IEventArgs arg, bool ignoreRun = true, params object[] param)
        //{
        //    ts.ForEach((t) =>
        //    {
        //        Clear(t, arg, ignoreRun, param);
        //    });
        //}

        public void CycleRunningPool(int count)
        {
            while (RunningPoool.count > 0)
            {
                T t = RunningPoool.Get();
                SleepPool.Set(t);
                AutoClean();
            }
        }

        public void CycleRunningPool()
        {
            while (RunningPoool.count > 0)
            {
                T t = RunningPoool.Get();
                SleepPool.Set(t);
                AutoClean();
            }
        }
        public void ClearSleepPool()
        {
            SleepPool.Clear();
        }
        public void CycleRunningPool(IEventArgs arg)
        {
            while (RunningPoool.count > 0)
            {
                T t = RunningPoool.Get();
                SleepPool.Set(t, arg);
                AutoClean();
            }
        }
        public void ClearSleepPool(IEventArgs arg)
        {
            SleepPool.Clear(arg);
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
