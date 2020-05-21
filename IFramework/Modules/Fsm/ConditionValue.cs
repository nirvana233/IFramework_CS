using System;

namespace IFramework.Modules.Fsm
{
    internal interface IConditionValue
    {
        string name { get; set; }
        Type valueType { get; }
        object value { get; set; }
    }
    /// <summary>
    /// 状态机条件值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConditionValue<T> : IConditionValue
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public Type valueType { get { return typeof(T); } }
        /// <summary>
        /// 数值
        /// </summary>
        public object value { get; set; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">数值名称</param>
        /// <param name="value">数值</param>
        public ConditionValue(string name,object value)
        {
            this.name = name;
            this.value = value;
        }

    }
}
