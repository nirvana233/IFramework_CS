using IFramework.Injection;
using IFramework.Singleton;

namespace IFramework.Fast
{

    /// <summary>
    /// 实体
    /// </summary>
    /// <typeparam name="TEnvironmentEntity"></typeparam>
    public abstract class Entity<TEnvironmentEntity> where TEnvironmentEntity : EnvironmentEntity<TEnvironmentEntity>
    {
        /// <summary>
        /// 跟实例
        /// </summary>
        protected static TEnvironmentEntity EnvEntity { get { return SingletonProperty<TEnvironmentEntity>.instance; } }
    }
}
