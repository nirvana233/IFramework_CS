using System.Collections;

namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 所有等待类的基类
    /// </summary>
    public abstract class CoroutineInstruction
    {
        private bool _isDone = false;
        readonly IEnumerator _routine;
        internal bool isDone
        {
            get { Update(); return _isDone; }
        }
        /// <summary>
        /// ctor
        /// </summary>
        protected CoroutineInstruction() { _routine = InnerLogoc(); }
        private void Update()
        {
            if (_routine != null)
            {
                if (_routine.MoveNext())
                {
                    if (_routine.Current != null)
                        _isDone = (bool)_routine.Current;
                }
            }
        }
        /// <summary>
        /// 等待逻辑，返回 True 结束
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator InnerLogoc();
    }
}
