namespace IFramework.Modules.Recorder
{
    /// <summary>
    /// 包含数值的状态
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ValueState<T> : BaseState, IValueContainer<T>
    {
        /// <summary>
        /// 数值
        /// </summary>
        public T value { get; set; }
    }
}
