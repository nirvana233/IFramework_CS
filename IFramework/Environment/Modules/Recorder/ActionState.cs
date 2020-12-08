using System;

namespace IFramework.Modules.Recorder
{
    /// <summary>
    /// 回调状态
    /// </summary>
    public class ActionState : BaseState
    {
        internal void SetValue(Action redo, Action undo)
        {
            this.redo = redo;
            this.undo = undo;
        }
        /// <summary>
        /// 执行
        /// </summary>
        public override void Redo()
        {
            redo();
        }
        /// <summary>
        /// 撤回
        /// </summary>
        public override void Undo()
        {
            undo();
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void OnReset()
        {
            redo = null;
            undo = null;
        }

        private Action redo;
        private Action undo;
    }
}
