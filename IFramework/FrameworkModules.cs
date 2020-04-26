using IFramework.Modules;
using IFramework.Modules.Coroutine;
using IFramework.Modules.ECS;
using IFramework.Modules.Fsm;
using IFramework.Modules.Message;
using IFramework.Modules.MVVM;
using IFramework.Modules.Resources;
namespace IFramework
{
    /// <summary>
    /// 框架提供的模块
    /// </summary>
    [FrameworkVersion(6)]
    public class FrameworkModules : FrameworkModuleContainer
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public FsmModule Fsm { get; set; }
        public LoomModule Loom { get; set; }
        public CoroutineModule Coroutine { get; set; }
        public MessageModule Message { get; set; }
        public FrameworkAppModule App { get; set; }
        public PoolModule Pool { get; set; }
        public ThreadModule ThreadPool { get; set; }
        public ECSModule ECS { get; set; }
        public MVVMModule MVVM { get; set; }
        public ResourceModule Resources { get; set; }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        internal FrameworkModules(FrameworkEnvironment env) : base("Framework", env, true)
        {

        }

    }
}
