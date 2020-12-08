using System;
using System.Collections.Generic;

namespace IFramework.Modules.Recorder
{
    /// <summary>
    /// 回调组
    /// </summary>
    public class ActionGroupState : BaseState
    {
        internal void SetValue(Action redo, Action undo)
        {
            this.redo.Add(redo);
            this.undo.Add(undo);
        }
        /// <summary>
        /// 执行
        /// </summary>
        public override void Redo()
        {
            for (int i = 0; i < redo.Count; i++)
            {
                redo[i]();
            }
        }
        /// <summary>
        /// 撤回
        /// </summary>
        public override void Undo()
        {
            for (int i = 0; i < redo.Count; i++)
            {
                undo[i]();
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

        private List<Action> redo = new List<Action>();
        private List<Action> undo = new List<Action>();
    }
}
