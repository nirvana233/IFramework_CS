using System;
using System.Collections;

namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 等待时间
    /// </summary>
    public class WaitForTimeSpan : CoroutineInstruction
    {
        private DateTime _setTime;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="span"> 等待时间</param>
        public WaitForTimeSpan(TimeSpan span) : base()
        {
            _setTime = DateTime.Now + span;
        }
        /// <summary>
        /// override
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator InnerLogoc()
        {
            while (DateTime.Now < _setTime)
            {
                yield return false;
            }
            yield return true;
        }

    }
}
