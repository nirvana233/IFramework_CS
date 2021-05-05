namespace IFramework.Fast
{
    /// <summary>
    /// 数据逻辑处理
    /// </summary>
    public interface IModelProcessor
    {

        /// <summary>
        /// 所属环境
        /// </summary>
        IEnvironment env { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        void Awake();
        /// <summary>
        /// 获取工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        TUtility GetUtility<TUtility>() where TUtility : class, IUtility;
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        TModel GetModel<TModel>() where TModel : class, IModel;


    }
}
