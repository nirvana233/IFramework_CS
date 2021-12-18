using System;
using System.Collections.Generic;

namespace IFramework.Modules
{
    /// <summary>
    /// 线程反馈模块
    /// </summary>
    class LoomModule : UpdateModule
    {
        private class DelayedTask
        {
            public Action action;
            public DelayedTask Config(Action action)
            {
                this.action = action;
                return this;
            }

        }
        private class Pool : ObjectPool<DelayedTask>
        {
            protected override bool OnSet(DelayedTask t, IEventArgs arg)
            {
                t.action = null;
                return base.OnSet(t, arg);
            }
            protected override DelayedTask CreatNew(IEventArgs arg)
            {
                return new DelayedTask();
            }
        }
        private Queue<DelayedTask> _tasks;
        private Queue<DelayedTask> _delay;
        private Pool _pool;
        /// <summary>
        /// 在主线程跑一个方法
        /// </summary>
        /// <param name="action"></param>
        public void RunDelay(Action action)
        {
            if (action == null) return;
            lock (_delay)
            {
                _delay.Enqueue(_pool.Get().Config(action));
            }
        }

        protected override ModulePriority OnGetDefaulyPriority()
        {
            return ModulePriority.Loom;
        }
        protected override void OnUpdate()
        {
            int count = 0;
            lock (_delay)
            {
                count = _delay.Count;
                for (int i = 0; i < count; i++)
                {
                    _tasks.Enqueue(_delay.Dequeue());
                }
            }
            for (int i = 0; i < count; i++)
            {
                var _task = _tasks.Dequeue();
                _task.action();
                _pool.Set(_task);
            }
        }
        protected override void OnDispose()
        {
            _tasks.Clear();
            _delay.Clear();
            _pool.Dispose();
        }
        protected override void Awake()
        {
            _tasks = new Queue<DelayedTask>();
            _delay = new Queue<DelayedTask>();
            _pool = new Pool();
        }
    }
}
