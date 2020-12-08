namespace IFramework.Modules.Recorder
{
    /// <summary>
    /// 状态基类
    /// </summary>
    public abstract class BaseState
    {
        internal BaseState front;
        internal BaseState next;
        internal OperationRecorderModule recorder;
        /// <summary>
        /// 执行
        /// </summary>
        public abstract void Redo();
        /// <summary>
        /// 撤回
        /// </summary>
        public abstract void Undo();

        internal virtual void Reset()
        {
            front = null;
            next = null;
            recorder = null;
            OnReset();
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        protected abstract void OnReset();
    }
}
