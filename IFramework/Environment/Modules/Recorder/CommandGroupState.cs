using System.Collections.Generic;

namespace IFramework.Modules.Recorder
{
    /// <summary>
    /// 命令组
    /// </summary>
    public class CommandGroupState : BaseState
    {
        internal void SetValue(ICommand redo, ICommand undo)
        {
            this.redo.Add(redo);
            this.undo.Add(undo);
        }
        /// <summary>
        /// 执行
        /// </summary>
        protected override void OnRedo()
        {
            for (int i = 0; i < redo.Count; i++)
            {
                redo[i].Excute();
            }
        }
        /// <summary>
        /// 撤回
        /// </summary>
        protected override void OnUndo()
        {
            for (int i = 0; i < redo.Count; i++)
            {
                undo[i].Excute();
            }
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void OnReset()
        {
            redo.Clear();
            undo.Clear();
        }

        private List<ICommand> redo = new List<ICommand>();
        private List<ICommand> undo = new List<ICommand>();
    }
}
