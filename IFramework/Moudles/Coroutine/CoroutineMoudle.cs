using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework.Moudles.Coroutine
{
    public interface ICoroutineMoudle
    {
        Coroutine StartCoroutine(IEnumerator routine);
    }
    public class CoroutineMoudle : FrameworkMoudle, ICoroutineMoudle
    {
        class CoroutinePool : IDisposable
        {
            public CoroutinePool() { coroutines = new Queue<Coroutine>(); }
            private Queue<Coroutine> coroutines;
            public Coroutine Get(IEnumerator routine)
            {
                Coroutine coroutine;
                if (coroutines.Count <= 0)
                    coroutine = new Coroutine(routine);
                else
                    coroutine = coroutines.Dequeue();
                return coroutine;
            }
            public void Set(Coroutine action)
            {
                coroutines.Enqueue(action);
            }

            public void Dispose()
            {
                coroutines.Clear();
            }
        }

      
        private CoroutinePool pool;
        internal Coroutine Get(IEnumerator routine)
        {
            var coroutine = pool.Get(routine);
            coroutine.routine = routine;
            coroutine.IsDone = false;
            coroutine.moudle = this;
            return coroutine;
        }
        internal void Set(Coroutine routine)
        {
            pool.Set(routine);
        }


        public Coroutine StartCoroutine(IEnumerator routine)
        {
            var coroutine = Get(routine);
            coroutine.Start();
            return coroutine;
        }
        internal event Action update;
        protected override void OnUpdate()
        {
            if (update != null)
                update();
        }
        protected override void OnDispose()
        {
            pool.Dispose();
        }

        protected override void Awake()
        {
            pool = new CoroutinePool();
        }
    }

}
