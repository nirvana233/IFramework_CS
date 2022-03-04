using System;
using System.Collections;

namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 携程 模拟
    /// </summary>
    internal class Coroutine : YieldInstruction,ICoroutine
    {
        internal CoroutineModule _module;
        internal IEnumerator _routine;
        private Coroutine _innerAction;
        private bool _isDone;
        /// <summary>
        /// 是否完成
        /// </summary>
        internal override bool isDone
        {
            get { return _isDone; }
        }

        internal void OnGet()
        {
            _isDone = false;
        }

        /// <summary>
        /// 携程完成时候回调
        /// </summary>
        internal event Action onCompelete;
        /// <summary>
        /// 手动结束携程
        /// </summary>
        public void Compelete()
        {

            _isDone = true;
            if (_innerAction!=null)
            {
                _innerAction.Compelete();
            }
            if (onCompelete != null)
                onCompelete();

            onCompelete = null;
            _innerAction = null;
            _routine = null;
            _module.update -= Update;
            _module.Set(this);
        }


        public Coroutine() { _isDone = false;}

        internal void Start()
        {
            _module.update += Update;
        }


        private void Update()
        {
            if (IsCompelete())
            {
                Compelete();
            } 
        }
        private IEnumerator IsCompete(Coroutine coroutine)
        {
            while (!coroutine.isDone)
            {
                yield return false;
            }
            yield return true;
        }

        private IEnumerator IsCompete(YieldInstruction instruction)
        {
            while (!instruction.isDone)
            {
                yield return false;
            }
            yield return true;
        }

        public CoroutineAwaiter GetAwaiter()
        {
            return new CoroutineAwaiter(this);
        }

        protected override bool IsCompelete()
        {
            if (_innerAction == null)
            {
                if (!_routine.MoveNext())
                {
                    return true;
                }
                if (_routine.Current != null)
                {
                    if (_routine.Current is YieldInstruction)
                        _innerAction = _module.Get(IsCompete(_routine.Current as YieldInstruction));
                    else if (_routine.Current is IEnumerator)
                        _innerAction = _module.Get(_routine.Current as IEnumerator);
                    else if (_routine.Current is Coroutine)
                        _innerAction = _module.Get(IsCompete(_routine.Current as Coroutine));
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

            return false;
        }
    }
}
