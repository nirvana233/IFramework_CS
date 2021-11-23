namespace IFramework.Modules
{
    /// <summary>
    /// 默认的模块的优先级
    /// </summary>
    public static class ModulePriorities
    {
        /// <summary>
        /// 配置表
        /// </summary>
        public const int Config = 0;
        /// <summary>
        /// 环境等待
        /// </summary>
        public const int Loom = 10;
        /// <summary>
        /// undo
        /// </summary>
        public const int Recorder = 50;

        /// <summary>
        /// 消息转发
        /// </summary>
        public const int Message = 100;
        /// <summary>
        /// 协程
        /// </summary>
        public const int Coroutine = 200;
        /// <summary>
        /// ecs
        /// </summary>
        public const int ECS = 400;
        /// <summary>
        /// fsm
        /// </summary>
        public const int FSM = 500;
        /// <summary>
        /// 其他
        /// </summary>
        public const int Custom = 1000;

    }
}
