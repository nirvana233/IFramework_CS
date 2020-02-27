using System.Collections;

namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 等待帧数
    /// </summary>
    public class WaitForFrames : CoroutineInstruction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="count">帧数 </param>
        public WaitForFrames(int count)
        {
            curCount = 0;
            Count = count;
        }
        private int curCount;
        private int Count { get; }
        /// <summary>
        /// override
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator InnerLogoc()
        {
            while (curCount++ < Count)
            {
                yield return false;
            }
            yield return true;
        }
    }
}
