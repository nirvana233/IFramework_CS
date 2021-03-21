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
    }
}
