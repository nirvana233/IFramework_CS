using System;

namespace IFramework.Modules.Fsm
{
    public interface IFsmConditionValue
    {
        string Name { get; set; }
        Type ValueType { get; }
        object Value { get; set; }
    }
    public class FsmConditionValue<T> : IFsmConditionValue
    {
        public string Name { get; set; }
        public Type ValueType { get { return typeof(T); } }
        public object Value { get; set; }

    }
}
