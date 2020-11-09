using System;

namespace IFramework
{
    /// <summary>
    /// 代码升级说明
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [Version(2)]
    public class UpdateAttribute : VersionAttribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string description;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="version"></param>
        /// <param name="description"></param>
        public UpdateAttribute(int version, string description) : base(version)
        {
            this.description = description;
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
