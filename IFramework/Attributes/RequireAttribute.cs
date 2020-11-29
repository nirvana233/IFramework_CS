using System;

namespace IFramework
{
    /// <summary>
    /// 依赖描述
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [ScriptVersionAttribute(6)]
    public class RequireAttribute :Attribute, IDescription
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; }
        /// <summary>
        /// types
        /// </summary>
        public Type type { get; }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="type"></param>
        public RequireAttribute(Type type) : this(type, "") { }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        public RequireAttribute(Type type, string description) 
        {
            this.type = type;
            this.description = description;
        }
    }
}
