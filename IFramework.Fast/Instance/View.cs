using IFramework.Injection;

namespace IFramework.Fast
{
    /// <summary>
    /// 界面
    /// </summary>
    /// <typeparam name="TSystemEntity"></typeparam>
    /// <typeparam name="TEnvironmentEntity"></typeparam>
    public abstract class View<TSystemEntity,TEnvironmentEntity>:FastEntity<TEnvironmentEntity>
        where TEnvironmentEntity : EnvironmentEntity<TEnvironmentEntity>
        where TSystemEntity:ISystemEntity
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
        protected View()
        {
            this.Inject();
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
            return systemEntity.GetUtility<TUtility>();
        }
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
        /// 获取数据处理
        /// </summary>
        /// <typeparam name="TModelProcessor"></typeparam>
        /// <returns></returns>
        public TModelProcessor GetViewProcessor<TModelProcessor>() where TModelProcessor : class, IProcessor
        {
            return systemEntity.GetModelProcessor<TModelProcessor>();
        }

    }
}
