using System;

namespace IFramework
{
    /// <summary>
    /// 依赖描述
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [FrameworkVersion(6)]
    public class DependenceAttribute : DescriptionAttribute
    {
        /// <summary>
        /// types
        /// </summary>
        public readonly Type type;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="type"></param>
        public DependenceAttribute(Type type) : base("")
        {
            this.type = type;
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        public DependenceAttribute(Type type, string description) : base(description)
        {
            this.type = type;
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
