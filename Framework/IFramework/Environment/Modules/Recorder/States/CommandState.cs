namespace IFramework.Modules.Recorder
{
    /// <summary>
    /// 命令状态
    /// </summary>
    public class CommandState: BaseState
    {
        internal void SetValue(ICommand redo,ICommand undo)
        {
            this.redo = redo;
            this.undo = undo;
        }
        /// <summary>
        /// 执行
        /// </summary>
        protected override void OnRedo()
        {
            redo.Excute();
        }
        /// <summary>
        /// 撤回
        /// </summary>
        protected override void OnUndo()
        {
            undo.Excute();
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void OnReset()
        {
            redo = null;
            undo = null;
        }

        private ICommand redo;
        private ICommand undo;
    }
}
