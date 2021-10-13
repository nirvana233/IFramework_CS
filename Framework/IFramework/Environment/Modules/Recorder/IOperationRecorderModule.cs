namespace IFramework.Modules.Recorder
{
    /// <summary>
    /// 操作记录
    /// </summary>
    public interface IOperationRecorderModule
    {
        /// <summary>
        /// 分配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Allocate<T>() where T : BaseState, new();
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        bool Redo();
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="state"></param>
        /// <param name="redo"></param>
        void Subscribe(BaseState state, bool redo = true);

        /// <summary>
        /// 撤回
        /// </summary>
        /// <returns></returns>
        bool Undo();
    }
}