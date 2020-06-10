using System;

namespace IFramework.Inject
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class InjectAttribute : Attribute
    {
        public InjectAttribute(string name)
        {
            this.name = name;
        }
        public string name { get; set; }

        public InjectAttribute()
        {
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
