namespace IFramework.Fast
{
    /// <summary>
    /// 数据逻辑处理
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// 环境
        /// </summary>
        IEnvironment env { get; set; }
    }
}
