using System;
using System.Threading;

namespace IFramework
{
    public sealed class LockWait : IDisposable
    {
        private LockParam lParam = null;
        public LockWait(ref LockParam lParam)
        {
            this.lParam = lParam;
            while (Interlocked.CompareExchange(ref lParam.Signal, 1, 0) == 1)
            {
                Thread.Sleep(lParam.SleepInterval);
            }
        }
        public  void Dispose()
        {
            Interlocked.Exchange(ref lParam.Signal, 0);
        }
    }


    public class LockParam
    {
        public int Signal = 0;
        public int SleepInterval = 1;
    }
}
