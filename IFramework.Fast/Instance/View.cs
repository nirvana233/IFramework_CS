using IFramework.Injection;

namespace IFramework.Fast
{
    /// <summary>
    /// 界面
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRootEntity"></typeparam>
    public abstract class View<TEntity,TRootEntity>:Entity<TRootEntity> ,IBelongToEntity<TEntity>,IProcessor
        where TRootEntity : class, IRootEntity
        where TEntity:ISubEntity
    {
        /// <summary>
        /// 实体
        /// </summary>
        [Inject] public TEntity entity { get; set; }
        /// <summary>
        /// 环境
        /// </summary>
        [Inject] public IEnvironment env { get; set; }
        /// <summary>
        /// ctor
        /// </summary>
        protected View()
        {
            root.env.container.Inject(this);
            Awake();
        }
        void IProcessor.Awake() { }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Awake();
        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        protected TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return container.GetValue<TUtility>(entity.flag);
        }
        /// <summary>
        /// 获取跟工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        protected TUtility GetRootUtility<TUtility>() where TUtility : class, IUtility
        {
            return container.GetValue<TUtility>(Entity.rootFlag);
        }

        /// <summary>
        /// 获取界面处理
        /// </summary>
        /// <typeparam name="TViewProcessor"></typeparam>
        /// <returns></returns>
        protected TViewProcessor GetViewProcessor<TViewProcessor>() where TViewProcessor : class, IViewProcessor
        {
            return container.GetValue<TViewProcessor>(entity.flag);
        }
        /// <summary>
        /// 获取跟界面处理
        /// </summary>
        /// <typeparam name="TViewProcessor"></typeparam>
        /// <returns></returns>
        protected TViewProcessor GetRootViewProcessor<TViewProcessor>() where TViewProcessor : class, IViewProcessor
        {
            return container.GetValue<TViewProcessor>(Entity.rootFlag);
        }
    }
}
