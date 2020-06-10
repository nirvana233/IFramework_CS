using System;
using System.Reflection;

namespace IFramework
{
    /// <summary>
    /// 描述
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    [FrameworkVersion(2)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string description;
        /// <summary>
        /// ctor
        /// </summary>
        public DescriptionAttribute(string description)
        {
            this.description = description;
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
