using System;
using System.Collections;
using System.Collections.Generic;

namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 携程模块
    /// </summary>
    [FrameworkVersion(8)]
    public class CoroutineModule : UpdateFrameworkModule
    {
        class CoroutinePool : IDisposable
        {
            public CoroutinePool() { _coroutines = new Queue<Coroutine>(); }
            private Queue<Coroutine> _coroutines;
            public Coroutine Get(IEnumerator routine)
            {
                Coroutine coroutine;
                if (_coroutines.Count <= 0)
                    coroutine = new Coroutine(routine);
                else
                    coroutine = _coroutines.Dequeue();
                return coroutine;
            }
            public void Set(Coroutine action)
            {
                _coroutines.Enqueue(action);
            }

            public void Dispose()
            {
                _coroutines.Clear();
            }
        }

      
        private CoroutinePool _pool;

        /// <summary>
        /// 开启一个携程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            var coroutine = Get(routine);
            coroutine.Start();
            return coroutine;
        }


        internal Coroutine Get(IEnumerator routine)
        {
            var coroutine = _pool.Get(routine);
            coroutine._routine = routine;
            coroutine.isDone = false;
            coroutine._module = this;
            return coroutine;
        }
        internal void Set(Coroutine routine)
        {
            _pool.Set(routine);
        }

        internal event Action update;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

        public override int priority { get { return 40; } }

        protected override void OnUpdate()
        {
            if (update != null)
                update();
        }
        protected override void OnDispose()
        {
            _pool.Dispose();
        }

        protected override void Awake()
        {
            _pool = new CoroutinePool();
        }

    }

    [FrameworkVersion(3)]
    public static class CoroutineModuleExtension
    {
        /// <summary>
        /// 开启一个携程
        /// </summary>
        /// <param name="obj"></param>
        /// <param name=" envType"></param>
        /// <param name="routine">迭代器</param>
        /// <returns></returns>
        public static Coroutine StartCoroutine(this object obj, EnvironmentType envType, IEnumerator routine)
        {
            FrameworkEnvironment _env = Framework.GetEnv( envType);
            return _env.modules.Coroutine.StartCoroutine(routine);
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
