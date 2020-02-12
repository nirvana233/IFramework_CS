using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework.Modules.Coroutine
{
    internal interface ICoroutineModule
    {
        Coroutine StartCoroutine(IEnumerator routine);
    }
    public class CoroutineModule : FrameworkModule, ICoroutineModule
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

        protected override bool needUpdate { get { return true; } }

        internal Coroutine Get(IEnumerator routine)
        {
            var coroutine = pool.Get(routine);
            coroutine.routine = routine;
            coroutine.IsDone = false;
            coroutine.module = this;
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
    [FrameworkVersion(3)]
    public static class CoroutineModuleExtension
    {
        public static Coroutine StartCoroutine(this object obj,int envIndex,IEnumerator routine)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envIndex);
            return _env.modules.Coroutine.StartCoroutine(routine);
        }
    }
}
