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

        public WaitForTimeSpan(TimeSpan span) : base()
        {
            _setTime = DateTime.Now + span;
        }
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
        public WaitForSeconds(double seconds) : base(TimeSpan.FromSeconds(seconds))
        {
        }
    }
    /// <summary>
    /// 等待trick
    /// </summary>
    public class WaitForTicks : WaitForTimeSpan
    {
        public WaitForTicks(long ticks) : base(TimeSpan.FromTicks(ticks))
        {
        }
    }
    /// <summary>
    /// 等待分钟
    /// </summary>
    public class WaitForMinutes : WaitForTimeSpan
    {
        public WaitForMinutes(double minutes) : base(TimeSpan.FromMinutes(minutes))
        {
        }
    }
    /// <summary>
    /// 等待毫秒
    /// </summary>
    public class WaitForMilliseconds : WaitForTimeSpan
    {
        public WaitForMilliseconds(double milliseconds) : base(TimeSpan.FromMilliseconds(milliseconds))
        {
        }
    }
    /// <summary>
    /// 等待小时
    /// </summary>
    public class WaitForHours : WaitForTimeSpan
    {
        public WaitForHours(double hours) : base(TimeSpan.FromHours(hours))
        {
        }
    }
    /// <summary>
    /// 等待日子
    /// </summary>
    public class WaitForDays : WaitForTimeSpan
    {
        public WaitForDays(double days) : base(TimeSpan.FromDays(days))
        {
        }
    }
    /// <summary>
    /// 等待条件成立
    /// </summary>
    public class WaitUtil : CoroutineInstruction
    {
        public WaitUtil(Func<bool> condition)
        {
            Condition = condition;
        }

        private Func<bool> Condition { get; }

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
        public WaitWhile(Func<bool> condition)
        {
            Condition = condition;
        }

        private Func<bool> Condition { get; }

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
        public WaitForFrames(int count)
        {
            curCount = 0;
            Count = count;
        }
        private int curCount;
        public int Count { get; }
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
        public WaitForFrame() : base(1)
        {
        }
    }

}
