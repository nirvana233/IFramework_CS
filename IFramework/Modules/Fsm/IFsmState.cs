namespace IFramework.Modules.Fsm
{
    /// <summary>
    /// 状态机状态
    /// </summary>
    public interface IFsmState
    {
        /// <summary>
        /// 切入状态执行一次
        /// </summary>
        void OnEnter();
        /// <summary>
        /// 切出状态执行一次
        /// </summary>
        void OnExit();
        /// <summary>
        /// 处于该状态不停刷新
        /// </summary>
        void Update();
    }
}
