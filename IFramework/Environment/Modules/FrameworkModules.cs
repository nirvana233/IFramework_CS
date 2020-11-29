using IFramework.Modules.Config;
using IFramework.Modules.Coroutine;
using IFramework.Modules.ECS;
using IFramework.Modules.Fsm;
using IFramework.Modules.Message;
using IFramework.Modules.MVVM;
namespace IFramework.Modules
{
    /// <summary>
    /// 框架提供的模块
    /// </summary>
    [ScriptVersionAttribute(6)]
    internal class FrameworkModules : FrameworkModuleContainer, IFrameworkModules
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public IFsmModule Fsm { get { return FindModule<FsmModule>("default"); } }
        public ICoroutineModule Coroutine { get { return FindModule<CoroutineModule>("default"); } }
        public IMessageModule Message { get { return FindModule<MessageModule>("default"); } }
        public IECSModule ECS { get { return FindModule<ECSModule>("default"); } }
        public IMVVMModule MVVM { get { return FindModule<MVVMModule>("default"); } }
        public IConfigModule Config { get { return FindModule<ConfigModule>("default"); } }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        internal FrameworkModules(FrameworkEnvironment env) : base("Framework", env, true)
        {

        }

    }
}
