namespace IFramework.Modules.Fsm
{
    [System.Runtime.InteropServices.ComVisible(false)]
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
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
