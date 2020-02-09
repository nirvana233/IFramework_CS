using System;
using System.Collections;

namespace IFramework.Modules.NodeAction
{
    public abstract class ActionNode :RecyclableObject, IActionNode,IDisposable
    {
        private bool mOnBeginCalled;

        private bool _disposed;
        private bool _isDone;
        private bool _autoRecyle;

        public bool disposed { get { return _disposed; } }
        public bool isDone { get { return _isDone; } }
        public bool autoRecyle { get { return _autoRecyle; } set { _autoRecyle = value; } }

        public event Action onBegin;
        public event Action onCompelete;
        public event Action onRecyle;
        public event Action onDispose;

        public void Dispose()
        {
            if (!recyled)
                Recyle();
            if (_disposed) return;
            OnDispose();
            if (onDispose != null)
                onDispose();
            onDispose = null;
            _disposed = true;
        }
       
        public bool MoveNext()
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
                _isDone = !OnMoveNext();
            if (_isDone)
            {
                if (onCompelete != null)
                    onCompelete();
                if (_autoRecyle)
                    Recyle();
            }
            return !_isDone && !recyled && !disposed;
        }

        public void Config(bool autoRecyle)
        {
            this._autoRecyle = autoRecyle;
            SetDataDirty();
        }


        protected abstract void OnBegin();
        protected abstract void OnCompelete();

        protected abstract bool OnMoveNext();
        protected abstract void OnDispose();

        protected override void OnDataReset()
        {
            mOnBeginCalled = false;
            _isDone = false;

            onBegin = null;
            onCompelete = null;
            onRecyle = null;
           
        }
        
        protected override void OnRecyle()
        {
            if (onRecyle != null)
                onRecyle();
            ResetData();
            _isDone = true;
        }

        public abstract void OnNodeReset();

        public void NodeReset()
        {
            mOnBeginCalled = false;
            _isDone = false;
            OnNodeReset();
        }
    }

}
