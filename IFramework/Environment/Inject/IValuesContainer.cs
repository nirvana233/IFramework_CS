using System;
using System.Collections.Generic;

namespace IFramework.Injection
{
    /// <summary>
    /// 对象注入容器
    /// </summary>
    public interface IValuesContainer :IContainer, IDisposable
    {
        /// <summary>
        /// 注入类型
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="name"></param>
        void Subscribe<Type>(string name = "");
        /// <summary>
        /// 注入类型
        /// </summary>
        /// <typeparam name="BaseType"></typeparam>
        /// <typeparam name="Type"></typeparam>
        /// <param name="name"></param>
        void Subscribe<BaseType, Type>(string name = "") where Type : BaseType;
        /// <summary>
        /// 注入类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="name"></param>
        void Subscribe(Type source, Type target, string name = "");

        /// <summary>
        /// 注入实例
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="inject"></param>
        void SubscribeInstance<Type>(Type instance, string name="", bool inject = true) where Type : class;

        /// <summary>
        /// 注入实例
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="inject"></param>
        void SubscribeInstance(Type baseType, object instance, string name = "", bool inject = true);

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        T GetValue<T>(string name = "", params object[] args) where T : class;
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        object GetValue(Type baseType, string name = "", params object[] param);
        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<object> GetValues(Type type);
        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <returns></returns>
        IEnumerable<Type> GetValues<Type>();


        /// <summary>
        /// 注入一个对象
        /// </summary>
        /// <param name="obj"></param>
        void Inject(object obj);
        /// <summary>
        /// 为所有容器内实例注入
        /// </summary>
        void InjectInstances();

        /// <summary>
        /// 清除所有
        /// </summary>
        void Clear();

    }

}
