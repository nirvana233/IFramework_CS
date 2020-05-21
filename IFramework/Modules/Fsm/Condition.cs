
using System;

namespace IFramework.Modules.Fsm
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    /// <summary>
    /// 比较方式
    /// </summary>
    public enum CompareType
    {
        None,Smaller, Bigger, Equals, NotEquals
    }
    public interface ICondition
    {
        Type type { get; }
        string name { get; }
        CompareType compareType { get;  }
        bool IsMetCondition();
    }
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

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    [System.Runtime.InteropServices.ComVisible(false)]
    public class IntCondition : Condition<int>
    {
        public IntCondition(ConditionValue<int> conditionValue, object compareValue, CompareType compareType) : base(conditionValue, compareValue, compareType) { }
        protected override CompareType SetConditionType(CompareType compareType)
        {
            return  compareType;
        }
        public override bool IsMetCondition()
        {
            switch (compareType)
            {
                case CompareType.Smaller:
                    return value < compareValue;
                case CompareType.Bigger:
                    return value > compareValue;
                case CompareType.Equals:
                    return compareValue == value;
                case CompareType.NotEquals:
                    return compareValue != value;
                default:
                    return false;
            }
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]
    public class FloatCondition : Condition<float>
    {
        public FloatCondition(ConditionValue<float> conditionValue, object compareValue, CompareType compareType) : base(conditionValue, compareValue, compareType) { }
        protected override CompareType SetConditionType(CompareType compareType)
        {
            return compareType;
        }
        public override bool IsMetCondition()
        {
            switch (compareType)
            {
                case CompareType.Smaller:
                    return value < compareValue;
                case CompareType.Bigger:
                    return value > compareValue;
                case CompareType.Equals:
                    return compareValue == value;
                case CompareType.NotEquals:
                    return compareValue != value;
                default:
                    return false;
            }
        }
    }
    [System.Runtime.InteropServices.ComVisible(false)]
    public class BoolCondition : Condition<bool>
    {
        public BoolCondition(ConditionValue<bool> conditionValue, object compareValue, CompareType compareType) : base(conditionValue, compareValue, compareType) { }
        public override bool IsMetCondition()
        {
            switch (compareType)
            {
                case CompareType.Equals:
                    return compareValue == value;
                case CompareType.NotEquals:
                    return compareValue != value;
                case CompareType.Smaller:
                case CompareType.Bigger:
                default:
                    return false;
            }
        }
        protected override CompareType SetConditionType(CompareType compareType)
        {
            switch (compareType)
            {
                case CompareType.Equals:
                case CompareType.NotEquals:
                    return compareType;
                default:
                    return CompareType.None;
            }
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
