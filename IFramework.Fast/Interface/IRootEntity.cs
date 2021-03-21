using IFramework.Singleton;

namespace IFramework.Fast
{
    /// <summary>
    /// 总实例
    /// </summary>
    public interface IRootEntity : ISubEntity , IBelongToEnvironment , ISingleton { }
}
