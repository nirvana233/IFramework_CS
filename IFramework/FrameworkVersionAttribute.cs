using System;
using System.Reflection;

namespace IFramework
{
    /// <summary>
    ///  框架代码版本默认有 1
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    [FrameworkVersion(2)]
    public class FrameworkVersionAttribute : Attribute
    {
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="version"></param>
        public FrameworkVersionAttribute(int version = 1)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        {
            this.version = version;
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
