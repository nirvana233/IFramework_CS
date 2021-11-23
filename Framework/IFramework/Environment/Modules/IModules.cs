using IFramework.Modules.Config;
using IFramework.Modules.Coroutine;
using IFramework.Modules.ECS;
using IFramework.Modules.Fsm;
using IFramework.Modules.Message;
using IFramework.Modules.Recorder;

namespace IFramework.Modules
{
    /// <summary>
    /// 模块组
    /// </summary>
    public interface IModules:IModuleContainer
    {
        /// <summary>
        /// 协程
        /// </summary>
        ICoroutineModule Coroutine { get; }
        /// <summary>
        /// ecs
        /// </summary>
        IECSModule ECS { get; }
        /// <summary>
        /// fsm
        /// </summary>
        IFsmModule Fsm { get; }
        /// <summary>
        /// 消息
        /// </summary>
        IMessageModule Message { get; }

        /// <summary>
        /// config
        /// </summary>
        IConfigModule Config { get; }
        /// <summary>
        /// 操作记录
        /// </summary>
        IOperationRecorderModule Recoder { get; }
    }
}