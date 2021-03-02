using System;

namespace IFramework.Modules
{
    /// <summary>
    /// 模块容器
    /// </summary>
    public interface IFrameworkModuleContainer
    {
        /// <summary>
        /// 是否绑定
        /// </summary>
        bool binded { get; }
        /// <summary>
        /// 组
        /// </summary>
        string chunck { get; }
        /// <summary>
        /// 环境
        /// </summary>
        FrameworkEnvironment env { get; }
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        FrameworkModule CreateModule(Type type, string name = "");
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T CreateModule<T>(string name = "") where T : FrameworkModule;
        /// <summary>
        /// 获取模块
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        FrameworkModule GetModule(Type type, string name = "");
        /// <summary>
        /// 获取模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T GetModule<T>(string name = "") where T : FrameworkModule;
        /// <summary>
        /// 查找模块
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        FrameworkModule FindModule(Type type, string name = "");
        /// <summary>
        /// 查找模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T FindModule<T>(string name = "") where T : FrameworkModule;
    }
}