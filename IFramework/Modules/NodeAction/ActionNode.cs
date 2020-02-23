using System;
using System.Collections;

namespace IFramework.Modules.NodeAction
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public abstract class ActionNode :RecyclableObject, IDisposable
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
        public void NodeReset()
        {
            mOnBeginCalled = false;
            _isDone = false;
            OnNodeReset();
        }

        public void Config(bool autoRecyle)
        {
            this._autoRecyle = autoRecyle;
            SetDataDirty();
        }

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



        protected abstract void OnBegin();
        protected abstract void OnCompelete();
        protected abstract bool OnMoveNext();
        protected abstract void OnDispose();
        protected abstract void OnNodeReset();
    }
    public class ConditionNode : ActionNode
    {
        private Func<bool> _condition;
        private Action _callback;
        public Func<bool> condition { get { return _condition; } }
        public Action callback { get { return _callback; } }
        public void Config(Func<bool> condition, Action callback, bool autoRecyle)
        {
            this._condition = condition;
            this._callback = callback;
            base.Config(autoRecyle);
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            _callback = null;
            _condition = null;
        }

        protected override void OnDispose()
        {
            _callback = null;
            _condition = null;
        }

        protected override bool OnMoveNext()
        {
            if (_condition.Invoke())
                _callback();
            return false;
        }

        protected override void OnNodeReset() { }
        protected override void OnBegin() { }
        protected override void OnCompelete() { }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
