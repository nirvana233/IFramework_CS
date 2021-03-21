using IFramework.Injection;

namespace IFramework.Fast
{
    /// <summary>
    /// 界面处理
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ViewProcessor<TEntity>:IViewProcessor, IBelongToEntity<TEntity> where TEntity : ISubEntity
    {
        /// <summary>
        /// 环境
        /// </summary>
        [Inject] public IEnvironment env { get; set; }
        /// <summary>
        /// 实体
        /// </summary>
        [Inject] public TEntity entity { get; set; }
        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        protected TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return env.container.GetValue<TUtility>(entity.flag);
        }
        /// <summary>
        /// 获取跟工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        protected TUtility GetRootUtility<TUtility>() where TUtility : class, IUtility
        {
            return env.container.GetValue<TUtility>(Entity.rootFlag);
        }

        /// <summary>
        /// 获取数据处理
        /// </summary>
        /// <typeparam name="TModelProcessor"></typeparam>
        /// <returns></returns>
        protected TModelProcessor GetModelProcessor<TModelProcessor>() where TModelProcessor : class, IModelProcessor
        {
            return env.container.GetValue<TModelProcessor>(entity.flag);
        }
        /// <summary>
        /// 获取跟数据处理
        /// </summary>
        /// <typeparam name="TModelProcessor"></typeparam>
        /// <returns></returns>
        protected TModelProcessor GetRootModelProcessor<TModelProcessor>() where TModelProcessor : class, IModelProcessor
        {
            return env.container.GetValue<TModelProcessor>(Entity.rootFlag);
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
