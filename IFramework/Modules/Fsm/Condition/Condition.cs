
using System;

namespace IFramework.Modules.Fsm
{
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    /// <summary>
    /// 状态机过度条件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Condition<T> : ICondition
    {
        private T _compareValue;
        private CompareType _compareType;
        private ConditionValue<T> _conditionValue { get; set; }

        /// <summary>
        /// 比较值（不变化）
        /// </summary>
        public T compareValue { get { return _compareValue; } }
        /// <summary>
        /// 过渡条件类型
        /// </summary>
        public Type type { get { return typeof(T); } }
        /// <summary>
        /// 比较值（变化）
        /// </summary>
        protected T value { get { return (T)_conditionValue.value; } }
        /// <summary>
        /// 条件的名称
        /// </summary>
        public string name { get { return _conditionValue.name; } }
        /// <summary>
        /// 比较方式
        /// </summary>
        public CompareType compareType { get { return _compareType; } }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cconditionValue">改变的数值</param>
        /// <param name="compareValue">比较的默认值</param>
        /// <param name="compareType">比较方式</param>
        protected Condition(ConditionValue<T> cconditionValue, object compareValue, CompareType compareType)
        {
            this._conditionValue = cconditionValue;
            this._compareValue = (T)compareValue;
            _compareType= SetConditionType(compareType);
            if (_compareType == CompareType.None) throw new Exception(string.Format("{0} could not use {1}", type, compareType));

        }
        /// <summary>
        /// 设置比较值
        /// </summary>
        /// <param name="compareType"></param>
        protected abstract CompareType SetConditionType(CompareType compareType);
        /// <summary>
        /// 是否条件成立
        /// </summary>
        /// <returns></returns>
        public abstract bool IsMetCondition();
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
