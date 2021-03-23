namespace IFramework.Fast
{
    /// <summary>
    /// 处理器
    /// </summary>
    public interface IProcessor
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
        /// 获取跟工具
        /// </summary>
        /// <typeparam name="TUtility"></typeparam>
        /// <returns></returns>
        TUtility GetRootUtility<TUtility>() where TUtility : class, IUtility;
    }
}
