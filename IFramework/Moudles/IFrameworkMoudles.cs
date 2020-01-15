using IFramework.Moudles.Coroutine;
using IFramework.Moudles.Fsm;
using IFramework.Moudles.Loom;
using IFramework.Moudles.Message;
using IFramework.Moudles.Timer;

namespace IFramework.Moudles
{
    internal interface IFrameworkMoudles/*: IFrameworkMoudleContaner*/
    {
        FsmMoudle Fsm { get; set; }
        TimerMoudle Timer { get; set; }
        LoomMoudle Loom { get; set; }
        CoroutineMoudle Coroutine { get; set; }
        MessageMoudle Message { get; set; }
    }
}
