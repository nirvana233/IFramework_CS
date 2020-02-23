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
    /// <summary>
    /// 等待秒
    /// </summary>
    public class WaitForSeconds : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="seconds">等待秒数</param>
        public WaitForSeconds(double seconds) : base(TimeSpan.FromSeconds(seconds))
        {
        }
    }
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
    /// <summary>
    /// 等待分钟
    /// </summary>
    public class WaitForMinutes : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="minutes">等待分钟数</param>
        public WaitForMinutes(double minutes) : base(TimeSpan.FromMinutes(minutes))
        {
        }
    }
    /// <summary>
    /// 等待毫秒
    /// </summary>
    public class WaitForMilliseconds : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="milliseconds">毫秒</param>
        public WaitForMilliseconds(double milliseconds) : base(TimeSpan.FromMilliseconds(milliseconds))
        {
        }
    }
    /// <summary>
    /// 等待小时
    /// </summary>
    public class WaitForHours : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="hours">小时</param>
        public WaitForHours(double hours) : base(TimeSpan.FromHours(hours))
        {
        }
    }
    /// <summary>
    /// 等待日子
    /// </summary>
    public class WaitForDays : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="days">天数</param>
        public WaitForDays(double days) : base(TimeSpan.FromDays(days))
        {
        }
    }
    /// <summary>
    /// 等待条件成立
    /// </summary>
    public class WaitUtil : CoroutineInstruction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="condition">等待成立条件</param>
        public WaitUtil(Func<bool> condition)
        {
            Condition = condition;
        }

        private Func<bool> Condition { get; }
        /// <summary>
        /// override
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator InnerLogoc()
        {
            while (!Condition.Invoke())
            {
                yield return false;
            }
            yield return true;
        }
    }
    /// <summary>
    /// 等待条件不成立
    /// </summary>
    public class WaitWhile : CoroutineInstruction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="condition">等待不成立条件</param>
        public WaitWhile(Func<bool> condition)
        {
            Condition = condition;
        }

        private Func<bool> Condition { get; }
        /// <summary>
        /// override
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator InnerLogoc()
        {
            while (Condition.Invoke())
            {
                yield return false;
            }
            yield return true;
        }
    }
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
            while (curCount++<Count)
            {
                yield return false;
            }
            yield return true;
        }
    }
    /// <summary>
    /// 等一帧
    /// </summary>
    public class WaitForFrame : WaitForFrames
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public WaitForFrame() : base(1)
        {
        }
    }

}
