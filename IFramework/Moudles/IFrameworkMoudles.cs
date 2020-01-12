using IFramework.Moudles.Coroutine;
using IFramework.Moudles.Fsm;
using IFramework.Moudles.Loom;
using IFramework.Moudles.Message;
using IFramework.Moudles.Timer;
using System;

namespace IFramework.Moudles
{
    public interface IFrameworkMoudles
    {
        FsmMoudle Fsm { get; set; }
        TimerMoudle Timer { get; set; }
        LoomMoudle Loom { get; set; }
        CoroutineMoudle Coroutine { get; set; }
        MessageMoudle Message { get; set; }
        FrameworkMoudle this[Type type, string name] { get; }
        FrameworkMoudle FindMoudle(Type type, string name);
        T FindMoudle<T>(string name) where T : FrameworkMoudle;
    }
}
