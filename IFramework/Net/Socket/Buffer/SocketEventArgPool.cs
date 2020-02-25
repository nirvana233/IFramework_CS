/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Pool;
using System;
using System.Net.Sockets;
using System.Threading;

namespace IFramework.Net
{
     class SocketEventArgPool : CapicityPool<SocketAsyncEventArgs>
    {
        public SocketEventArgPool(int capcity) : base(capcity) { }
        protected override void OnClear(SocketAsyncEventArgs t, IEventArgs arg)
        {
            base.OnClear(t,arg);
            t.Dispose();
        }
        public SocketAsyncEventArgs GetFreeArg(Func<object, bool> func, bool wait)
        {
            int retry = 1;
            while (true)
            {
                var tArgs = Get();
                if (tArgs != null) return tArgs;
                if (wait == false)
                {
                    if (retry > 16) break;
                    retry++;
                }
                var isContinue = func(retry);
                if (isContinue == false) break;
                Thread.Sleep(1000 * retry);
            }
            return default(SocketAsyncEventArgs);
        }

        protected override SocketAsyncEventArgs CreatNew(IEventArgs arg)
        {
            return default(SocketAsyncEventArgs);
        }
    }

}
