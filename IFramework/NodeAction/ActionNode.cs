using System;

namespace IFramework.NodeAction
{
    /// <summary>
    /// 节点
    /// </summary>
     abstract class ActionNode : RecyclableObject, IActionNode
    {
        private bool mOnBeginCalled;
        public ActionNode parent;
        public int depth;
        private bool _isDone;
        private bool _autoRecyle;
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool isDone { get { return _isDone; } }
        /// <summary>
        /// 自动回收
        /// </summary>
        public bool autoRecyle { get { return _autoRecyle; } set { _autoRecyle = value; } }

        internal event Action onBegin;
        internal event Action onCompelete;
        internal event Action onRecyle;
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
            if (!_isDone) _isDone = !OnMoveNext();
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


        /// <summary>
        /// 下一帧
        /// </summary>
        /// <returns></returns>
        protected abstract bool OnMoveNext();
        /// <summary>
        /// 调用时机：repeat 节点完成一次
        /// </summary>
        protected abstract void OnNodeReset();


        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            if (!recyled)
                Recyle();
        }
        /// <summary>
        /// 设置自动回收
        /// </summary>
        /// <param name="autoRecyle"></param>
        public void Config(bool autoRecyle)
        {
            this._autoRecyle = autoRecyle;
            SetDataDirty();
        }
        /// <summary>
        /// 数据重置
        /// </summary>
        protected override void OnDataReset()
        {
            parent = null;
            depth = 0;
            mOnBeginCalled = false;
            _isDone = false;
            onBegin = null;
            onCompelete = null;
            onRecyle = null;
        }
        /// <summary>
        /// 被回收时
        /// </summary>
        protected override void OnRecyle()
        {
            if (onRecyle != null)
                onRecyle();
            ResetData();
            _isDone = false;
        }


        /// <summary>
        /// 开始时
        /// </summary>
        protected virtual void OnBegin() { }
        /// <summary>
        /// 结束时
        /// </summary>
        protected virtual void OnCompelete() { }


    }

}
