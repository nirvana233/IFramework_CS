using System;

namespace IFramework
{
    /// <summary>
    /// 代码升级说明
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [ScriptVersionAttribute(2)]
    public class VersionUpdateAttribute : ScriptVersionAttribute,IDescription
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="version"></param>
        /// <param name="description"></param>
        public VersionUpdateAttribute(int version, string description) : base(version)
        {
            this.description = description;
        }
    }
}
