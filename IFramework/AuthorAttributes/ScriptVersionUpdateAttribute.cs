using System;
using System.Reflection;

namespace IFramework
{
    /// <summary>
    /// 代码升级说明
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [FrameworkVersion(2)]
    public class ScriptVersionUpdateAttribute : FrameworkVersionAttribute
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
        public ScriptVersionUpdateAttribute(int version, string description) : base(version)
        {
            this.description = description;
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
