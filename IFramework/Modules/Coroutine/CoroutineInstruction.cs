using System.Collections;

namespace IFramework.Modules.Coroutine
{
    public abstract class CoroutineInstruction
    {
        private bool isDone = false;
        readonly IEnumerator routine;
        internal bool IsDone
        {
            get { Update(); return isDone; }
        }
        protected CoroutineInstruction() { routine = InnerLogoc(); }
        protected void Update()
        {
            if (routine != null)
            {
                if (routine.MoveNext())
                {
                    if (routine.Current != null)
                        isDone = (bool)routine.Current;
                }
            }
        }
        protected abstract IEnumerator InnerLogoc();
    }
}
