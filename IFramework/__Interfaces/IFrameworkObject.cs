using System;

namespace IFramework
{
    /// <summary>
    /// 积累接口
    /// </summary>
    public interface IFrameworkObject:IEventArgs
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        Guid guid { get; }
        /// <summary>
        /// 是否释放
        /// </summary>
        bool disposed { get; }
    }
}
