namespace IFramework.Modules.MVVM
{
    /// <summary>
    /// MVVM 模块
    /// </summary>
    public interface IMVVMModule
    {
        /// <summary>
        /// 添加组
        /// </summary>
        /// <param name="group"></param>
        void AddGroup(MVVMGroup group);
        /// <summary>
        /// 找到组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        MVVMGroup FindGroup(string name);
        /// <summary>
        /// 移除组
        /// </summary>
        /// <param name="group"></param>
        void RemoveGroup(MVVMGroup group);
        /// <summary>
        /// 移除组
        /// </summary>
        /// <param name="name"></param>
        void RemoveGroup(string name);
    }
}