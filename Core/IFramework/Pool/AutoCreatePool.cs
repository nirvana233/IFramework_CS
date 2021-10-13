using System;

namespace IFramework
{
    /// <summary>
    /// Activator 创建实例 对现池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActivatorCreatePool<T> : ObjectPool<T>
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected override T CreatNew(IEventArgs arg)
        {
            return Activator.CreateInstance<T>();
        }
    }
}
