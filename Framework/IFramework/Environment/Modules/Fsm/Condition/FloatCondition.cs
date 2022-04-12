namespace IFramework.Modules.Fsm
{
    [System.Runtime.InteropServices.ComVisible(false)]
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
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
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
