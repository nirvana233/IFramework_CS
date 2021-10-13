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
            _curCount = 0;
            this._count = count;
        }
        private int _curCount;
        private int _count { get; }
        /// <summary>
        /// override
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator InnerLogoc()
        {
            while (_curCount++ < _count)
            {
                yield return false;
            }
            yield return true;
        }
    }
}
