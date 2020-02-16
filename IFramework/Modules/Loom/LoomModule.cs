using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IFramework.Modules.Loom
{
    /// <summary>
    /// 线程反馈模块
    /// </summary>
    public class LoomModule : FrameworkModule
    {
        private struct DelayedTask
        {
            public float time;
            public Action action;
            public DelayedTask(float time, Action action)
            {
                this.time = time;
                this.action = action;
            }

        }
        private int MaxThreadCount = 8;
        private int ThreadCount;
        private Semaphore _semaphore;
        private List<DelayedTask> _tasks;
        private List<DelayedTask> _delayedTasks;
        private List<DelayedTask> _NoDelayTasks;


        private double GetUtcFrame(DateTime time)
        {
            DateTime timeZerro = new DateTime(1970, 1, 1);
            return (time - timeZerro).TotalMilliseconds;
        }
        /// <summary>
        /// 在主线程跑一个方法
        /// </summary>
        /// <param name="action"></param>
        /// <param name="time">延时</param>
        public void RunOnMainThread(Action action, float time = 0.0f)
        {
            if (time != 0)
            {
                lock (_delayedTasks)
                {
                    _delayedTasks.Add(new DelayedTask((float)GetUtcFrame(DateTime.Now) + time, action));
                }
            }
            else
            {
                lock (_NoDelayTasks)
                {
                    _NoDelayTasks.Add(new DelayedTask(0, action));
                }
            }
        }
        /// <summary>
        /// 在子线程跑一个方法
        /// </summary>
        /// <param name="action"></param>
        public void RunOnSubThread(Action action)
        {
            _semaphore.WaitOne();
            Interlocked.Increment(ref ThreadCount);
            ThreadPool.QueueUserWorkItem((ar) => {

                try
                {
                    action();
                }
                catch { }
                finally
                {
                    Interlocked.Decrement(ref ThreadCount);
                }
            });
        }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override void OnUpdate()
        {
            lock (_NoDelayTasks)
            {
                while (_NoDelayTasks.Count > 0)
                {
                    _NoDelayTasks.Dequeue().action();
                }
            }
            lock (_tasks)
            {
                lock (_delayedTasks)
                {
                    var temp = _delayedTasks.Where(d => d.time <= (float)GetUtcFrame(DateTime.Now)).ToList();
                    _tasks.AddRange(temp);
                    temp.ForEach((task) => { _delayedTasks.Remove(task); });
                }
                while (_tasks.Count != 0)
                {
                    _tasks.Dequeue().action();
                }
            }
        }
        protected override void OnDispose()
        {
            _NoDelayTasks.Clear();
            _tasks.Clear();
            _delayedTasks.Clear();
            _semaphore.Close();
        }
        protected override void Awake()
        {
            _semaphore = new Semaphore(MaxThreadCount, MaxThreadCount);
            _tasks = new List<DelayedTask>();
            _delayedTasks = new List<DelayedTask>();
            _NoDelayTasks = new List<DelayedTask>();
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
}
