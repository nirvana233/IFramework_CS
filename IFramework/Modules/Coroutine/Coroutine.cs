using System;
using System.Collections;

namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 携程 模拟
    /// </summary>
    public class Coroutine
    {
        internal CoroutineModule _module;
        internal IEnumerator _routine;
        private Coroutine _innerAction;
        private bool _isDone;
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool isDone
        {
            get { return _isDone; }
            internal set
            {
                if (value != _isDone)
                {
                    _isDone = value;
                    if (value)
                    {
                        if (onCompelete != null)
                            onCompelete();
                    }
                }
            }
        }
        /// <summary>
        /// 携程完成时候回调
        /// </summary>
        public event Action onCompelete;
        /// <summary>
        /// 手动结束携程
        /// </summary>
        public void Stop()
        {
            _isDone = true;
            _innerAction = null;
            _routine = null;
            _module.Set(this);
            _module.update -= Update;
        }


        internal Coroutine(IEnumerator routine) { _isDone = false; this._routine = routine; }

        internal void Start()
        {
            _module.update += Update;
        }


        private void Update()
        {
            if (_innerAction == null)
            {
                if (!_routine.MoveNext())
                {
                    Stop();
                }
                if (_isDone) return;
                if (_routine.Current != null)
                {
                    if (_routine.Current is CoroutineInstruction)
                        _innerAction = _module.Get(IsFinish(_routine.Current as CoroutineInstruction));
                    else if (_routine.Current is IEnumerator)
                        _innerAction = _module.Get(_routine.Current as IEnumerator);
                    else if (_routine.Current is Coroutine)
                        _innerAction = _module.Get(IsFinish(_routine.Current as Coroutine));
                }
            }
            if (_innerAction != null)
            {
                if (!_innerAction.isDone)
                {
                    _innerAction.Update();
                    if (_innerAction.isDone)
                        _innerAction = null;
                }
            }
        }
        private IEnumerator IsFinish(Coroutine coroutine)
        {
            while (!coroutine.isDone)
            {
                yield return false;
            }
            yield return true;
        }

        private IEnumerator IsFinish(CoroutineInstruction CoroutineActionInstruction)
        {
            while (!CoroutineActionInstruction.isDone)
            {
                yield return false;
            }
            yield return true;
        }
    }

}
