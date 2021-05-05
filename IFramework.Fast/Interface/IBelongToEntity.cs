using IFramework.Singleton;

namespace IFramework.Fast
{
    /// <summary>
    /// 属于实例
    /// </summary>
    /// <typeparam name="TSystemEntity"></typeparam>
    public interface IBelongToEntity<TSystemEntity> where TSystemEntity : ISystemEntity
    {
        /// <summary>
        /// 实例
        /// </summary>
        TSystemEntity entity { get; set; }
    }
}
