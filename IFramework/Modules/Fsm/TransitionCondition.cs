
using System;

namespace IFramework.Modules.Fsm
{
    /// <summary>
    /// 比较方式
    /// </summary>
    public enum ConditionCompareType
    {
        SmallerThanCompare, BiggerThanCompare, EqualsWithCompare, NotEqualsWithCompare
    }
    public interface ITransitionCondition
    {
        Type type { get; }
        string name { get; }
        ConditionCompareType compareType { get;  }
        bool IsMetCondition();
    }
    /// <summary>
    /// 状态机过度条件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TransitionCondition<T> : ITransitionCondition
    {
        private T _compareValue;
        private ConditionCompareType _compareType;
        private FsmConditionValue<T> _conditionValue { get; set; }

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
        public ConditionCompareType compareType { get { return _compareType; }protected set { _compareType = value; } }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cconditionValue">改变的数值</param>
        /// <param name="compareValue">比较的默认值</param>
        /// <param name="compareType">比较方式</param>
        protected TransitionCondition(FsmConditionValue<T> cconditionValue, object compareValue, ConditionCompareType compareType)
        {
            this._conditionValue = cconditionValue;
            this._compareValue = (T)compareValue;
            SetConditionType(compareType);
        }
        /// <summary>
        /// 设置比较值
        /// </summary>
        /// <param name="compareType"></param>
        protected abstract void SetConditionType(ConditionCompareType compareType);
        /// <summary>
        /// 是否条件成立
        /// </summary>
        /// <returns></returns>
        public abstract bool IsMetCondition();
    }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class StringTransitionCondition : TransitionCondition<string>
    {
        public StringTransitionCondition(FsmConditionValue<string> cconditionValue, object CompareValue, ConditionCompareType compareType) : base(cconditionValue, CompareValue, compareType) { }
        public override bool IsMetCondition()
        {
            switch (compareType)
            {
                case ConditionCompareType.EqualsWithCompare:
                    return compareValue == value;
                case ConditionCompareType.NotEqualsWithCompare:
                    return compareValue != value;
                case ConditionCompareType.SmallerThanCompare:
                case ConditionCompareType.BiggerThanCompare:
                default:
                    return false;
            }
        }
        protected override void SetConditionType(ConditionCompareType compareType)
        {
            switch (compareType)
            {
                case ConditionCompareType.SmallerThanCompare:
                case ConditionCompareType.BiggerThanCompare:
                    throw new Exception("The Type is Illeagal whith " + type);
                case ConditionCompareType.EqualsWithCompare:
                case ConditionCompareType.NotEqualsWithCompare:
                    this.compareType = compareType;
                    break;
            }
        }
    }
    public class IntTransitionCondition : TransitionCondition<int>
    {
        public IntTransitionCondition(FsmConditionValue<int> conditionValue, object compareValue, ConditionCompareType compareType) : base(conditionValue, compareValue, compareType) { }
        protected override void SetConditionType(ConditionCompareType compareType)
        {
            this.compareType = compareType;
        }
        public override bool IsMetCondition()
        {
            switch (compareType)
            {
                case ConditionCompareType.SmallerThanCompare:
                    return value < compareValue;
                case ConditionCompareType.BiggerThanCompare:
                    return value > compareValue;
                case ConditionCompareType.EqualsWithCompare:
                    return compareValue == value;
                case ConditionCompareType.NotEqualsWithCompare:
                    return compareValue != value;
                default:
                    return false;
            }
        }
    }
    public class FloatTransitionCondition : TransitionCondition<float>
    {
        public FloatTransitionCondition(FsmConditionValue<float> conditionValue, object compareValue, ConditionCompareType compareType) : base(conditionValue, compareValue, compareType) { }
        protected override void SetConditionType(ConditionCompareType compareType)
        {
            this.compareType = compareType;
        }
        public override bool IsMetCondition()
        {
            switch (compareType)
            {
                case ConditionCompareType.SmallerThanCompare:
                    return value < compareValue;
                case ConditionCompareType.BiggerThanCompare:
                    return value > compareValue;
                case ConditionCompareType.EqualsWithCompare:
                    return compareValue == value;
                case ConditionCompareType.NotEqualsWithCompare:
                    return compareValue != value;
                default:
                    return false;
            }
        }
    }
    public class BoolTransitionCondition : TransitionCondition<bool>
    {
        public BoolTransitionCondition(FsmConditionValue<bool> conditionValue, object compareValue, ConditionCompareType compareType) : base(conditionValue, compareValue, compareType) { }
        public override bool IsMetCondition()
        {
            switch (compareType)
            {
                case ConditionCompareType.EqualsWithCompare:
                    return compareValue == value;
                case ConditionCompareType.NotEqualsWithCompare:
                    return compareValue != value;
                case ConditionCompareType.SmallerThanCompare:
                case ConditionCompareType.BiggerThanCompare:
                default:
                    return false;
            }
        }
        protected override void SetConditionType(ConditionCompareType compareType)
        {
            switch (compareType)
            {
                case ConditionCompareType.SmallerThanCompare:
                case ConditionCompareType.BiggerThanCompare:
                    throw new Exception("The Type is Illeagal whith " + type);
                case ConditionCompareType.EqualsWithCompare:
                case ConditionCompareType.NotEqualsWithCompare:
                    this.compareType = compareType;
                    break;
            }
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
