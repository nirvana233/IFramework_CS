using IFramework.Injection;

namespace IFramework.Fast
{
    /// <summary>
    /// 数据处理
    /// </summary>
    /// <typeparam name="TSystemEntity"></typeparam>
    public abstract class Processor<TSystemEntity>: IProcessor,IBelongToSystemEntity<TSystemEntity> where TSystemEntity:ISystemEntity
    {
        /// <summary>
        /// 环境
        /// </summary>
        [Inject]public IEnvironment env { get; set; }
        /// <summary>
        /// 实体
        /// </summary>
        [Inject]public TSystemEntity systemEntity { get; set; }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return systemEntity.GetModel<TModel>();
        }

        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return systemEntity.GetUtility<TUtility>();
        }
        void IProcessor.Awake()
        {
            Awake();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Awake();
    }
}
