using System;

namespace IFramework.Modules.Config
{
    /// <summary>
    /// config 模块
    /// </summary>
    public interface IConfigModule
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetConfig(Type type, string name);
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T GetConfig<T>(string name);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IConfigModule SetConfig(Type type, object value, string name);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IConfigModule SetConfig<T>(T value, string name);
        /// <summary>
        /// 绑定 config 绑定对象需要继承BindableObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setter"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        IConfigModule BindConfig<T>(Action<T> setter, Func<T> getter);
    }
}