using System;
using System.Collections;

namespace IFramework.Moudles.NodeAction
{
    public abstract class ActionNode : IActionNode
    {
        private bool mOnBeginCalled;

        private bool disposed;
        private bool isDone;
        private bool autoDispose;

        object IEnumerator.Current { get { return null; } }

        public bool Disposed { get { return disposed; } }
        public bool IsDone { get { return isDone; } }
        public bool AutoDispose { get { return autoDispose; } set { autoDispose = value; } }

        public event Action onBegin;
        public event Action onCompelete;
        public event Action onDispose;

        public void Dispose()
        {
            if (disposed) return;
            OnDispose();
            if (onDispose != null)
                onDispose();
            onDispose = null;
            onBegin = null;
            onCompelete = null;
            disposed = true;
        }
        public void Reset()
        {
            mOnBeginCalled = false;
            isDone = false;
            OnReset();
        }
        public bool MoveNext()
        {
            if (disposed) return false;
            if (!mOnBeginCalled)
            {
                mOnBeginCalled = true;
                OnBegin();
                if (onBegin != null)
                    onBegin();
            }
            if (!isDone)
                isDone = !OnMoveNext();
            if (isDone)
            {
                if (onCompelete != null)
                    onCompelete();
                if (autoDispose)
                    Dispose();
            }
            return !isDone && !disposed;
        }

        protected abstract void OnBegin();
        protected abstract void OnCompelete();

        protected abstract bool OnMoveNext();
        protected abstract void OnDispose();
        protected abstract void OnReset();
    }

}
