using IFramework.Singleton;

namespace IFramework.Fast
{
    /// <summary>
    /// 属于系统实例
    /// </summary>
    /// <typeparam name="TSystemEntity"></typeparam>
    public interface IBelongToSystemEntity<TSystemEntity> where TSystemEntity : ISystemEntity
    {
        /// <summary>
        /// 实例
        /// </summary>
        TSystemEntity systemEntity { get; set; }
    }

}
