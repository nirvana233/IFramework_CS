using System;
using System.Collections;

namespace IFramework.Modules.Coroutine
{
    public class Coroutine
    {
        internal CoroutineModule module;
        internal IEnumerator routine;
        private Coroutine innerAction;
        private bool isDone;
        public bool IsDone
        {
            get { return isDone; }
            internal set
            {
                if (value != isDone)
                {
                    isDone = value;
                    if (value)
                    {
                        if (onCompelete != null)
                            onCompelete();
                    }
                }
            }
        }
        public event Action onCompelete;

        internal Coroutine(IEnumerator routine) { isDone = false; this.routine = routine; }

        internal void Start()
        {
            module.update += Update;
        }

        public void Stop()
        {
            isDone = true;
            innerAction = null;
            routine = null;
            module.Set(this);
            module.update -= Update;
        }

        private void Update()
        {
            if (innerAction == null)
            {
                if (!routine.MoveNext())
                {
                    Stop();
                }
                if (isDone) return;
                if (routine.Current != null)
                {
                    if (routine.Current is CoroutineInstruction)
                        innerAction = module.Get(IsFinish(routine.Current as CoroutineInstruction));
                    else if (routine.Current is IEnumerator)
                        innerAction = module.Get(routine.Current as IEnumerator);
                    else if (routine.Current is Coroutine)
                        innerAction = module.Get(IsFinish(routine.Current as Coroutine));
                }
            }
            if (innerAction != null)
            {
                if (!innerAction.IsDone)
                {
                    innerAction.Update();
                    if (innerAction.IsDone)
                        innerAction = null;
                }
            }
        }
        private IEnumerator IsFinish(Coroutine coroutine)
        {
            while (!coroutine.IsDone)
            {
                yield return false;
            }
            yield return true;
        }

        private IEnumerator IsFinish(CoroutineInstruction CoroutineActionInstruction)
        {
            while (!CoroutineActionInstruction.IsDone)
            {
                yield return false;
            }
            yield return true;
        }
    }

}
