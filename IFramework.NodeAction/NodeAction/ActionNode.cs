using System;

namespace IFramework.NodeAction
{
    abstract class ActionNode : RecyclableObject, IActionNode
    {
        private bool mOnBeginCalled;

        private bool _isDone;
        private bool _autoRecyle;

        public bool isDone { get { return _isDone; } }
        public bool autoRecyle { get { return _autoRecyle; } set { _autoRecyle = value; } }

        internal event Action onBegin;
        internal event Action onCompelete;
        internal event Action onRecyle;
        //internal event Action onDispose;
        internal event Action onFrame;

        protected override void OnDispose()
        {
            base.OnDispose();
            if (!recyled)
                Recyle();
            //OnNodeDispose();
            //if (onDispose != null)
            //    onDispose();
            //onDispose = null;
        }
        internal bool MoveNext()
        {
            if (recyled || disposed) return false;
            if (!mOnBeginCalled)
            {
                mOnBeginCalled = true;
                OnBegin();
                if (onBegin != null)
                    onBegin();
            }
            if (!_isDone)
            {
                if (onFrame != null)
                    onFrame();
                _isDone = !OnMoveNext();
            }
            if (_isDone)
            {
                OnCompelete();
                if (onCompelete != null)
                    onCompelete();
                if (_autoRecyle)
                    Recyle();
            }
            return !_isDone && !recyled && !disposed;
        }
        internal void NodeReset()
        {
            mOnBeginCalled = false;
            _isDone = false;
            OnNodeReset();
        }

        internal void Config(bool autoRecyle)
        {
            this._autoRecyle = autoRecyle;
            SetDataDirty();
        }

        protected override void OnDataReset()
        {
            mOnBeginCalled = false;
            _isDone = false;
            onFrame = null;
            onBegin = null;
            onCompelete = null;
            onRecyle = null;
        }
        protected override void OnRecyle()
        {
            if (onRecyle != null)
                onRecyle();
            ResetData();
            _isDone = false;
        }



        protected abstract void OnBegin();
        protected abstract void OnCompelete();
        protected abstract bool OnMoveNext();
        //protected abstract void OnNodeDispose();
        protected abstract void OnNodeReset();
    }

}
