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
        public override void Redo()
        {
            redo.Excute();
        }
        /// <summary>
        /// 撤回
        /// </summary>
        public override void Undo()
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
