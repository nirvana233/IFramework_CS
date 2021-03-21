using IFramework.Singleton;

namespace IFramework.Fast
{
    /// <summary>
    /// 属于实例
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBelongToEntity<TEntity> where TEntity : ISubEntity
    {
        /// <summary>
        /// 实例
        /// </summary>
        TEntity entity { get; set; }
    }
}
