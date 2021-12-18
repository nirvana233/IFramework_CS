﻿using System;
using System.Collections;

namespace IFramework.Modules.Coroutine
{

    /// <summary>
    /// 携程模块
    /// </summary>
    [ScriptVersionAttribute(8)]
    public class CoroutineModule : UpdateModule, ICoroutineModule
    {
        class CoroutinePool : ObjectPool<Coroutine>
        {
            protected override Coroutine CreatNew(IEventArgs arg)
            {
                return new Coroutine();
            }
            protected override void OnGet(Coroutine t, IEventArgs arg)
            {
                t.OnGet();
            }
        }

      
        private CoroutinePool _pool;

        /// <summary>
        /// 开启一个携程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public ICoroutine StartCoroutine(IEnumerator routine)
        {
            var coroutine = Get(routine);
            coroutine.Start();
            return coroutine;
        }


        internal Coroutine Get(IEnumerator routine)
        {
            var coroutine = _pool.Get();
            coroutine._routine = routine;
            coroutine._module = this;
            return coroutine;
        }
        internal void Set(Coroutine routine)
        {
            _pool.Set(routine);
        }

        internal event Action update;
        /// <summary>
        /// 优先级
        /// </summary>
        protected override ModulePriority OnGetDefaulyPriority()
        {
            return ModulePriority.Coroutine;
        }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
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

        public void StopCoroutine(ICoroutine routine)
        {
            routine.Compelete();
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

    }
}
