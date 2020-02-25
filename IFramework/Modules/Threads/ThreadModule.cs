using IFramework.Pool;
using System;
using System.Collections.Generic;
using System.Threading;
namespace IFramework.Modules.Threads
{/// <summary>
/// 线程池模块
/// </summary>
    public class ThreadModule : FrameworkModule
    {
        private class InnerThread : IDisposable
        {
            public Thread Thread { get { return thread; } }
            public bool IsRunning { get { return isRunning; } }
            private bool isRunning;
            private Thread thread;
            private ManualResetEvent eve;
            private ThreadArgs Arg;

            public InnerThread()
            {
                eve = new ManualResetEvent(false);//为trur,一开始就可以执行
                thread = new Thread(ThreadLoop);
                thread.IsBackground = true;
                thread.Start();
            }

            private void Run()
            {
                this.eve.Set();
                isRunning = true;
            }
            private void Stop()
            {
                this.eve.Reset();
                Arg.mou.Set(Arg, this);
                isRunning = false;
            }

            public void RunTask(ThreadArgs arg)
            {
                this.Arg = arg;
                if (!isRunning)
                    Run();
            }
            private void ThreadLoop()
            {
                while (true)
                {
                    this.eve.WaitOne();
                    if (Arg.task != null)
                        Arg.task();
                    if (Arg.callback != null)
                        Arg.callback();
                    Stop();
                }
            }
            public void Dispose()
            {
                thread.Abort();
                thread.Join();
                thread = null;
            }
        }
        private class ThreadArgs : FrameworkArgs, IDisposable
        {
            public Action task;
            public Action callback;
            public ThreadModule mou;

            public void Dispose()
            {
                ResetData();
            }

            public ThreadArgs Config(Action task, Action callback, ThreadModule mou)
            {
                this.task = task;
                this.callback = callback;
                this.mou = mou;
                return this;
            }

            protected override void OnDataReset()
            {
                this.task = null; this.callback = null;

            }
        }

        private class Threads : SleepingPool<InnerThread>
        {
            protected override InnerThread CreatNew(IEventArgs arg)
            {
                return new InnerThread();
            }
        }

        private int maxThreads = 8;
        private int ThreadCount;
        private Semaphore semaphore;


        private Queue<ThreadArgs> CacheArgs;
        private Queue<ThreadArgs> RunningArgs;
        private CachePool<InnerThread> ThreadCache;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override void Awake()
        {
            ThreadCache = new CachePool<InnerThread>(new RunningPool<InnerThread>(), new Threads());
            semaphore = new Semaphore(maxThreads, ThreadCount);

            CacheArgs = new Queue<ThreadArgs>();
            RunningArgs = new Queue<ThreadArgs>();
        }
        protected override void OnDispose()
        {
            CacheArgs.Clear();
            RunningArgs.Clear();
            ThreadCache.Dispose();
            semaphore.Dispose();
            semaphore = null;
            RunningArgs = null;
            CacheArgs = null;
        }
        protected override void OnUpdate()
        {
            semaphore.WaitOne();
            lock (CacheArgs)
            {
                while (CacheArgs.Count != 0)
                    RunningArgs.Enqueue(CacheArgs.Dequeue());
            }
            while (RunningArgs.Count != 0)
            {
                Interlocked.Increment(ref ThreadCount);
                ThreadCache.Get().RunTask(RunningArgs.Dequeue());
            }
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        private void Set(ThreadArgs arg, InnerThread th)
        {
            arg.Recyle();
            ThreadCache.Set(th);
            Interlocked.Decrement(ref ThreadCount);
        }
        /// <summary>
        /// 跑一个线程任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callback"></param>
        public void RunTask(Action task, Action callback)
        {
            lock (CacheArgs)
                CacheArgs.Enqueue(RecyclableObject.Allocate<ThreadArgs>(this.container.env).Config(task, callback, this));
        }

    }
}
