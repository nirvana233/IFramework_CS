namespace IFramework
{
    /// <summary>
    /// 可回收
    /// </summary>
    public interface IRecyclable:IFrameworkObject
    {
        /// <summary>
        /// 环境
        /// </summary>
        FrameworkEnvironment env { get; }
        /// <summary>
        /// 是否被回收
        /// </summary>
        bool recyled { get; }

        /// <summary>
        /// 回收
        /// </summary>
        void Recyle();
    }
}