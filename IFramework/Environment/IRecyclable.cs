namespace IFramework
{
    /// <summary>
    /// 可回收
    /// </summary>
    [VersionAttribute(20)]
    [UpdateAttribute(20, "添加注释")]
    public interface IRecyclable
    {
        /// <summary>
        /// 回收
        /// </summary>
        void Recyle();
        /// <summary>
        /// 数据重置
        /// </summary>
        void ResetData();
        /// <summary>
        /// 是否被回收了
        /// </summary>
        bool recyled { get; }
    }
}
