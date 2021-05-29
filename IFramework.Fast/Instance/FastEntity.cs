using IFramework.Injection;
using IFramework.Singleton;

namespace IFramework.Fast
{
    /// <summary>
    /// 快速实体
    /// </summary>
    /// <typeparam name="TEnvironmentEntity"></typeparam>
    public class FastEntity<TEnvironmentEntity> : IBelongToEnvironmentEntity<TEnvironmentEntity> where TEnvironmentEntity : EnvironmentEntity<TEnvironmentEntity>
    {
        /// <summary>
        /// 注入
        /// </summary>
        public void Inject()
        {
            EnvironmentEntity.env.container.Inject(this);
        }

        public IEvvironmentEntity GetEnvironmentEnitity()
        {
            return EnvironmentEntity;
        }

        /// <summary>
        /// 所属环境实体
        /// </summary>
        protected static TEnvironmentEntity EnvironmentEntity { get { return SingletonProperty<TEnvironmentEntity>.instance; } }
    }
}
