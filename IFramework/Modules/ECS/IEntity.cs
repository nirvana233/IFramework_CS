using System;

namespace IFramework.Modules.ECS
{
    /// <summary>
    /// 实体
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 注册的模块
        /// </summary>
         ECSModule _mou { get; set; }
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IComponent GetComponent(Type type);
        /// <summary>
        /// 湖区组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetComponent<T>() where T : IComponent;
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
         IComponent AddComponent(Type type);
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
         T AddComponent<T>() where T : IComponent;
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="component"></param>
        /// <param name="useSame"></param>
        /// <returns></returns>
        IComponent AddComponent(IComponent component, bool useSame);
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <param name="useSame"></param>
        /// <returns></returns>
        T AddComponent<T>(T component, bool useSame) where T : IComponent;
        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="type"></param>
         void RemoveComponent(Type type);

        /// <summary>
        /// 移除组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
         void RemoveComponent<T>() where T : IComponent;
        /// <summary>
        /// 是否包含组件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
         bool ContainsComponent(Type type);
        /// <summary>
        /// 是否包含组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool ContainsComponent<T>() where T : IComponent;
        /// <summary>
        /// 直接替换原组件，结构体必须使用这个方法刷新数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="component"></param>
         void ReFreshComponent(Type type, IComponent component);
        /// <summary>
        /// 直接替换原组件，结构体必须使用这个方法刷新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
         void ReFreshComponent<T>(T component) where T : IComponent;
        /// <summary>
        /// 解除模块注册
        /// </summary>
         void Destory();
    }

}
