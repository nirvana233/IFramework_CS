using IFramework.Modules.APP;
using IFramework.Modules.Coroutine;
using IFramework.Modules.Fsm;
using IFramework.Modules.Loom;
using IFramework.Modules.Message;
using IFramework.Modules.Timer;
using IFramework.Modules.Pool;
using IFramework.Modules.Threads;
using IFramework.Modules.ECS;

namespace IFramework.Modules
{
    internal interface IFrameworkModules/*: IFrameworkModuleContaner*/
    {
        FrameworkAppModule App { get; set; }
        FsmModule Fsm { get; set; }
        TimerModule Timer { get; set; }
        LoomModule Loom { get; set; }
        CoroutineModule Coroutine { get; set; }
        MessageModule Message { get; set; }
        PoolModule Pool { get; set; }
        ThreadModule ThreadPool { get; set; }
        ECSModule ECS { get; set; }
    }
}
