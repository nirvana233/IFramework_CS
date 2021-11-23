using IFramework.Modules.Config;
using IFramework.Modules.Coroutine;
using IFramework.Modules.ECS;
using IFramework.Modules.Fsm;
using IFramework.Modules.Message;
using IFramework.MVVM;
using IFramework.Modules.Recorder;

namespace IFramework.Modules
{
    /// <summary>
    /// 框架提供的模块
    /// </summary>
    [ScriptVersionAttribute(6)]
    class Modules : ModuleContainer, IModules
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public ICoroutineModule Coroutine { get { return GetModule<CoroutineModule>(); } }
        public IMessageModule Message { get { return GetModule<MessageModule>(); } }
        public IOperationRecorderModule Recoder { get { return GetModule<OperationRecorderModule>(); } }
        public IConfigModule Config { get { return GetModule<ConfigModule>(); } }
        public IFsmModule Fsm { get { return GetModule<FsmModule>(); } }
        public IECSModule ECS { get { return GetModule<ECSModule>(); } }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        internal Modules(FrameworkEnvironment env) : base(env)
        {

        }

    }
}
