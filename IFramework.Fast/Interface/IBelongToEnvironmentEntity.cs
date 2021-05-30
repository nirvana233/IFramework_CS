namespace IFramework.Fast
{
    /// <summary>
    /// 属于环境实例
    /// </summary>
    public interface IBelongToEnvironmentEntity
    {
        /// <summary>
        /// 获取环境实例
        /// </summary>
        /// <returns></returns>
        IEvvironmentEntity GetEnvironmentEnitity();
    }
    /// <summary>
    /// 属于环境实例
    /// </summary>
    /// <typeparam name="TEnvironmentEntity"></typeparam>
    public interface IBelongToEnvironmentEntity<TEnvironmentEntity> where TEnvironmentEntity :EnvironmentEntity<TEnvironmentEntity>
    {

    }
 
}
