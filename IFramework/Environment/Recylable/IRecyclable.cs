using System;

namespace IFramework
{
    /// <summary>
    /// 可回收
    /// </summary>
    public interface IRecyclable
    {
        /// <summary>
        /// 环境
        /// </summary>
        FrameworkEnvironment env { get; }
        /// <summary>
        /// 是否被回收
        /// </summary>
        bool recyled { get; }
        /// <summary>
        /// 唯一ID
        /// </summary>
        Guid guid { get; }
        /// <summary>
        /// 名字
        /// </summary>
        string name { get; }
        /// <summary>
        /// 回收
        /// </summary>
        void Recyle();
    }
}