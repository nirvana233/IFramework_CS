using System;
using System.Collections.Generic;
using System.Threading;
namespace IFramework.Moudles.Threads
{
    public class ThreadMoudle : FrameworkMoudle
    {
        private class InnerThread : IDisposable
        {
            private ThreadMoudle mou;
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
            public ThreadMoudle mou;

            public void Dispose()
            {
                Reset();
            }

            public ThreadArgs Config(Action task, Action callback, ThreadMoudle mou)
            {
                this.task = task;
                this.callback = callback;
                this.mou = mou;
                return this;
            }

            protected override void OnReset()
            {
                this.task = null; this.callback = null;

            }
        }

        private class Threads : SleepingPool<InnerThread>
        {
            protected override InnerThread CreatNew(IEventArgs arg, params object[] param)
            {
                return new InnerThread();
            }
        }

        public bool isRunning { get { return enable; } }
        private int maxThreads = 8;
        private int ThreadCount;
        private Semaphore semaphore;

        protected override bool needUpdate { get { return true; } }

        private Queue<ThreadArgs> CacheArgs;
        private Queue<ThreadArgs> RunningArgs;
        private CachePool<InnerThread> ThreadCache;

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


        private void Set(ThreadArgs arg, InnerThread th)
        {
            arg.Recyle();
            ThreadCache.Set(th);
            Interlocked.Decrement(ref ThreadCount);
        }
        public void RunTask(Action task, Action callback)
        {
            lock (CacheArgs)
                CacheArgs.Enqueue(FrameworkArgs.Allocate<ThreadArgs>().Config(task, callback, this));
        }

    }
}
