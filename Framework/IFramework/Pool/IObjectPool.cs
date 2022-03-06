using System;

namespace IFramework
{
    /// <summary>
    /// 对象池接口
    /// </summary>
    [Tip("不要直接继承，请继承 ObjectPool<T>")]
    public interface IObjectPool:IDisposable {
        /// <summary>
        /// 数量
        /// </summary>
        int count { get; }
        /// <summary>
        /// 类型
        /// </summary>
        Type type { get; }
        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        void Set(object obj,IEventArgs args);
    }
}
