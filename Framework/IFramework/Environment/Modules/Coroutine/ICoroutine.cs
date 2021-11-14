namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 协程实体
    /// </summary>
    public interface ICoroutine:IAwaitable<CoroutineAwaiter>
    {
        /// <summary>
        /// 是否结束
        /// </summary>
        bool isDone { get; }
        /// <summary>
        /// 手动结束
        /// </summary>
        void Compelete();
    }
}