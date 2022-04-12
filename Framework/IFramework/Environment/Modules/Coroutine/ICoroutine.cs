namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 协程实体
    /// </summary>
    public interface ICoroutine:IAwaitable<CoroutineAwaiter>
    {

        /// <summary>
        /// 手动结束
        /// </summary>
        void Compelete();
    }
}