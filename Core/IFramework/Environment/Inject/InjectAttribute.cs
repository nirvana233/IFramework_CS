using System;

namespace IFramework.Injection
{
    /// <summary>
    /// 注入标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,Inherited =false,AllowMultiple =true)]
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// 注入名
        /// </summary>
        public string name { get;}

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        public InjectAttribute(string name="")
        {
            this.name = name;
        }
    }
}
