using System;

namespace IFramework
{
    /// <summary>
    ///  框架代码版本默认有 1
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    [ScriptVersionAttribute(2)]
    public class ScriptVersionAttribute : Attribute, IVersion
    {
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="version"></param>
        public ScriptVersionAttribute(int version = 1)
        {
            this.version = version;
        }
    }
}
