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
        private Queue<DelayedTask> _delay;
        /// <summary>
        /// 在主线程跑一个方法
        /// </summary>
        /// <param name="action"></param>
        public void RunDelay(Action action)
        {
            if (action == null) return;
            lock (_delay)
            {
                _delay.Enqueue(Framework.GlobalAllocate<DelayedTask>().Config(action));
            }
        }

        protected override ModulePriority OnGetDefaulyPriority()
        {
            return ModulePriority.Loom;
        }
        protected override void OnUpdate()
        {
            int count = 0;
            Queue<DelayedTask> _tasks = Framework.GlobalAllocate<Queue<DelayedTask>>();
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
                _task.GlobalRecyle();
            }
            _tasks.GlobalRecyle();
        }
        protected override void OnDispose()
        {
            while (_delay.Count != 0) _delay.Dequeue().GlobalRecyle();
            _delay.Clear();
            _delay.GlobalRecyle();
        }
        protected override void Awake()
        {
            _delay = Framework.GlobalAllocate<Queue<DelayedTask>>();
        }
    }
}
