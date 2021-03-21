using IFramework.Injection;
using IFramework.Singleton;

namespace IFramework.Fast
{
    /// <summary>
    /// 实例
    /// </summary>
    public abstract class Entity:DisposableObject, IEntity
    {
        /// <summary>
        /// 标记
        /// </summary>
        public virtual string flag { get { return GetType().Name; } }
        /// <summary>
        /// 根标记
        /// </summary>
        public const string rootFlag = "root";
    }
    /// <summary>
    /// 实体
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class Entity<TEntity> : Entity where TEntity : class, IRootEntity
    {
        /// <summary>
        /// 跟实例
        /// </summary>
        protected static TEntity root { get { return SingletonProperty<TEntity>.instance; } }
        /// <summary>
        /// 数据容器
        /// </summary>
        protected virtual IValuesContainer container { get { return root.env.container; } }
    }
}
