using IFramework.Modules.Config;
using IFramework.Modules.Coroutine;
using IFramework.Modules.ECS;
using IFramework.Modules.Fsm;
using IFramework.Modules.Message;
using IFramework.Modules.MVVM;
using IFramework.Modules.Recorder;

namespace IFramework.Modules
{
    /// <summary>
    /// 框架提供的模块
    /// </summary>
    [ScriptVersionAttribute(6)]
    class FrameworkModules : FrameworkModuleContainer, IFrameworkModules
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public IFsmModule Fsm { get { return GetModule<FsmModule>("default"); } }
        public ICoroutineModule Coroutine { get { return GetModule<CoroutineModule>("default"); } }
        public IMessageModule Message { get { return GetModule<MessageModule>("default"); } }
        public IECSModule ECS { get { return GetModule<ECSModule>("default"); } }
        public IMVVMModule MVVM { get { return GetModule<MVVMModule>("default"); } }
        public IConfigModule Config { get { return GetModule<ConfigModule>("default"); } }
        public IOperationRecorderModule Recoder { get { return GetModule<OperationRecorderModule>("default"); } }


#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        internal FrameworkModules(FrameworkEnvironment env) : base(env, true)
        {

        }

    }
}
