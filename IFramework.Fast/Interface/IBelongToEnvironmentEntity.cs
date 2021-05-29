namespace IFramework.Fast
{
    /// <summary>
    /// 属于环境实例
    /// </summary>
    public interface IBelongToEnvironmentEntity
    {
        IEvvironmentEntity GetEnvironmentEnitity();
    }
    /// <summary>
    /// 属于环境实例
    /// </summary>
    /// <typeparam name="TEnvironmentEntity"></typeparam>
    public interface IBelongToEnvironmentEntity<TEnvironmentEntity>: IBelongToEnvironmentEntity where TEnvironmentEntity :EnvironmentEntity<TEnvironmentEntity>
    {

    }
 
}
