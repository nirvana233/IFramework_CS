using System;

namespace IFramework
{
    /// <summary>
    /// 环境类型
    /// </summary>
    [Flags]
    public enum EnvironmentType : int
    {
        /// <summary>
        /// 所有，配合环境初始化
        /// </summary>
        None = 1,
        /// <summary>
        /// 环境0
        /// </summary>
        Ev0 = 2,
        /// <summary>
        /// 环境1
        /// </summary>
        Ev1 = 4,
        /// <summary>
        /// 环境2
        /// </summary>
        Ev2 = 8,
        /// <summary>
        /// 环境3
        /// </summary>
        Ev3 = 16,
        /// <summary>
        /// 环境4
        /// </summary>
        Ev4 = 32
    }
}
