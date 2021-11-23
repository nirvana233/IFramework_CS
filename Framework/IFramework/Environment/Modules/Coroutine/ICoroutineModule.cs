using System.Collections;

namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 协程模块
    /// </summary>
    public interface ICoroutineModule
    {
        /// <summary>
        /// 开启一个协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        ICoroutine StartCoroutine(IEnumerator routine);
        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="routine"></param>
        void StopCoroutine(ICoroutine routine);

    }
}