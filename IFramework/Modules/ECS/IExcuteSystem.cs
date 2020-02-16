namespace IFramework.Modules.ECS
{
    /// <summary>
    /// 处理系统
    /// </summary>
    public interface IExcuteSystem
    {
        /// <summary>
        /// 处理
        /// </summary>
        void Excute();
        /// <summary>
        /// 模块释放时
        /// </summary>
        void OnModuleDispose();
    }

}
