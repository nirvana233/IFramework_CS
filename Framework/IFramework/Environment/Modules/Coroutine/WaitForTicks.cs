using System;

namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 等待trick
    /// </summary>
    public class WaitForTicks : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="ticks">等待帧数</param>
        public WaitForTicks(long ticks) : base(TimeSpan.FromTicks(ticks))
        {
        }
    }
}
