namespace IFramework.Modules.ECS
{
    /// <summary>
    /// 处理系统
    /// </summary>
    public interface IExcuteSystem:ICommand
    {
        /// <summary>
        /// 模块释放时
        /// </summary>
        void OnModuleDispose();
    }

}
