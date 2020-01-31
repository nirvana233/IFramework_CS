using IFramework.Moudles.APP;
using IFramework.Moudles.Coroutine;
using IFramework.Moudles.Fsm;
using IFramework.Moudles.Loom;
using IFramework.Moudles.Message;
using IFramework.Moudles.Timer;
using IFramework.Moudles.Pool;
using IFramework.Moudles.Threads;

namespace IFramework.Moudles
{
    internal interface IFrameworkMoudles/*: IFrameworkMoudleContaner*/
    {
        FrameworkAppMoudle App { get; set; }
        FsmMoudle Fsm { get; set; }
        TimerMoudle Timer { get; set; }
        LoomMoudle Loom { get; set; }
        CoroutineMoudle Coroutine { get; set; }
        MessageMoudle Message { get; set; }
        PoolMoudle Pool { get; set; }
        ThreadMoudle ThreadPool { get; set; }
    }
}
