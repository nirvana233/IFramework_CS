using System;

namespace IFramework
{
    /// <summary>
    /// 对象池接口
    /// </summary>
    [Tip("不要直接继承，请继承 ObjectPool<T>")]
    public interface IObjectPool:IDisposable { }
}
