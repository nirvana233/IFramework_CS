using System;
using System.Collections;

namespace IFramework.Moudles.Coroutine
{
    public class WaitForTimeSpan : CoroutineInstruction
    {
        private DateTime setTime;

        public WaitForTimeSpan(TimeSpan span) : base()
        {
            setTime = DateTime.Now + span;
        }
        protected override IEnumerator InnerLogoc()
        {
            while (DateTime.Now < setTime)
            {
                yield return false;
            }
            yield return true;
        }

    }
    public class WaitForSeconds : WaitForTimeSpan
    {
        public WaitForSeconds(double seconds) : base(TimeSpan.FromSeconds(seconds))
        {
        }
    }
    public class WaitForTicks : WaitForTimeSpan
    {
        public WaitForTicks(long ticks) : base(TimeSpan.FromTicks(ticks))
        {
        }
    }
    public class WaitForMinutes : WaitForTimeSpan
    {
        public WaitForMinutes(Double minutes) : base(TimeSpan.FromMinutes(minutes))
        {
        }
    }
    public class WaitForMilliseconds : WaitForTimeSpan
    {
        public WaitForMilliseconds(Double milliseconds) : base(TimeSpan.FromMilliseconds(milliseconds))
        {
        }
    }
    public class WaitForHours : WaitForTimeSpan
    {
        public WaitForHours(Double hours) : base(TimeSpan.FromHours(hours))
        {
        }
    }
    public class WaitForDays : WaitForTimeSpan
    {
        public WaitForDays(Double days) : base(TimeSpan.FromDays(days))
        {
        }
    }

    public class WaitForConditionsMet : CoroutineInstruction
    {
        public WaitForConditionsMet(Func<bool> condition)
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
}
