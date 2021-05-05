using IFramework.Injection;

namespace IFramework.Fast
{
    /// <summary>
    /// 界面
    /// </summary>
    /// <typeparam name="TSystemEntity"></typeparam>
    /// <typeparam name="TEnvironmentEntity"></typeparam>
    public abstract class View<TSystemEntity,TEnvironmentEntity>:Entity<TEnvironmentEntity> ,IBelongToEntity<TSystemEntity>
        where TEnvironmentEntity : EnvironmentEntity<TEnvironmentEntity>
        where TSystemEntity:ISystemEntity
    {
        /// <summary>
        /// 实体
        /// </summary>
        [Inject] public TSystemEntity entity { get; set; }
        /// <summary>
        /// 环境
        /// </summary>
        [Inject] public IEnvironment env { get; set; }
        /// <summary>
        /// ctor
        /// </summary>
        protected View()
        {
            EnvEntity.env.container.Inject(this);
            Awake();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Awake();
        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return entity.GetUtility<TUtility>();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return entity.GetModel<TModel>();
        }
        /// <summary>
        /// 获取数据处理
        /// </summary>
        /// <typeparam name="TModelProcessor"></typeparam>
        /// <returns></returns>
        public TModelProcessor GetViewProcessor<TModelProcessor>() where TModelProcessor : class, IModelProcessor
        {
            return entity.GetModelProcessor<TModelProcessor>();
        }

    }
}
