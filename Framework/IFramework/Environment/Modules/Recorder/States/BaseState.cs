﻿using System;

namespace IFramework.Modules.Recorder
{
    /// <summary>
    /// 状态基类
    /// </summary>
    public abstract class BaseState : ICloneable, IUniqueIDObject
    {
        internal BaseState front;
        internal BaseState next;
        internal OperationRecorderModule recorder;
        /// <summary>
        /// id
        /// </summary>
        protected Guid _id = Guid.NewGuid();

        /// <summary>
        /// id
        /// </summary>
        public Guid guid { get { return _id; } }

        internal void Redo() { OnRedo(); }
        internal void Undo() { OnUndo(); }
        internal virtual void Reset()
        {

            front = null;
            next = null;
            recorder = null;
            OnReset();
        }


        /// <summary>
        /// 执行
        /// </summary>
        protected abstract void OnRedo();
        /// <summary>
        /// 撤回
        /// </summary>
        protected abstract void OnUndo();
        /// <summary>
        /// 重置数据
        /// </summary>
        protected abstract void OnReset();
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

    }
}
