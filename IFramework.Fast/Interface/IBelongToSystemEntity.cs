using IFramework.Singleton;

namespace IFramework.Fast
{

    /// <summary>
    /// 属于 系统实例
    /// </summary>
    public interface IBelongToSystemEntity
    {
        /// <summary>
        /// 获取系统
        /// </summary>
        /// <returns></returns>
        ISystemEntity GetSystemEntity();
    }
    /// <summary>
    /// 属于系统实例
    /// </summary>
    /// <typeparam name="TSystemEntity"></typeparam>
    public interface IBelongToSystemEntity<TSystemEntity>: IBelongToSystemEntity where TSystemEntity : ISystemEntity
    {
        /// <summary>
        /// 实例
        /// </summary>
        TSystemEntity systemEntity { get; set; }
    }

}
