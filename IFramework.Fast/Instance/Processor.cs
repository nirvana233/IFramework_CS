using IFramework.Injection;
using IFramework.Modules.Message;

namespace IFramework.Fast
{

    /// <summary>
    /// 数据处理
    /// </summary>
    /// <typeparam name="TSystemEntity"></typeparam>
    /// <typeparam name="TEnvironmentEntity"></typeparam>
    public abstract class Processor<TSystemEntity, TEnvironmentEntity> : FastEntity<TEnvironmentEntity>,IProcessor,
        IBelongToSystemEntity<TSystemEntity>,
        ICanGetModel, ICanPublishMessage, ICanListenMessage, ICanSendCommand, ICanGetUtility
        where TEnvironmentEntity : EnvironmentEntity<TEnvironmentEntity>
        where TSystemEntity : ISystemEntity
    {
        /// <summary>
        /// 实体
        /// </summary>
        [Inject] public TSystemEntity systemEntity { get; set; }
        /// <summary>
        /// 环境
        /// </summary>
        [Inject] public IEnvironment env { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        protected Processor()
        {
            this.Inject();
            Awake();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Awake();

        /// <summary>
        /// 获取所属系统实例
        /// </summary>
        /// <returns></returns>
        public ISystemEntity GetSystemEntity()
        {
            return systemEntity;
        }
    }
}
