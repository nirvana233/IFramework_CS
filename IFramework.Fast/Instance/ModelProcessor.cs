using IFramework.Injection;

namespace IFramework.Fast
{
    /// <summary>
    /// 数据处理
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ModelProcessor<TEntity>: IModelProcessor,IBelongToEntity<TEntity> where TEntity:ISubEntity
    {
        /// <summary>
        /// 环境
        /// </summary>
        [Inject]public IEnvironment env { get; set; }
        /// <summary>
        /// 实体
        /// </summary>
        [Inject]public TEntity entity { get; set; }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public TModel GetModel<TModel>() where TModel : class, IModel
        {
           return  env.container.GetValue <TModel>(entity.flag);
        }
        /// <summary>
        /// 获取跟数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public TModel GetRootModel<TModel>() where TModel : class, IModel
        {
            return env.container.GetValue<TModel>(Entity.rootFlag);
        }
        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return env.container.GetValue<TUtility>(entity.flag);
        }
        /// <summary>
        /// 获取跟工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        public TUtility GetRootUtility<TUtility>() where TUtility : class, IUtility
        {
            return env.container.GetValue<TUtility>(Entity.rootFlag);
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
