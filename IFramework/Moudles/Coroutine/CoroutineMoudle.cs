using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework.Moudles.Coroutine
{
    public class CoroutineMoudle : FrameworkMoudle
    {
        protected CoroutineMoudle(string chunck):base(chunck)
        {
            pool = new CoroutinePool();
        }
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
        protected override void OnDispose()
        {
            pool.Dispose();
        }


        public Coroutine StartCoroutine(IEnumerator routine)
        {
            var coroutine = Get(routine);
            coroutine.Start();
            return coroutine;
        }
        internal event Action update;
        public override void Update()
        {
            if (update != null)
                update();
        }
    }

}
