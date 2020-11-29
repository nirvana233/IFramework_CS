using System;
using System.Collections.Generic;

namespace IFramework.Modules.ECS
{
    /// <summary>
    /// Ecs模块
    /// </summary>
    public interface IECSModule
    {
        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        TEntity CreateEntity<TEntity>() where TEntity : IEntity;
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEntity> GetEntitys();
        /// <summary>
        /// 注册实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        void SubscribeEntity<TEntity>(TEntity entity) where TEntity : IEntity;
        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entity"></param>
        void UnSubscribeEntity(IEntity entity);




        /// <summary>
    /// 添加组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="component"></param>
    /// <param name="useSame"></param>
    /// <returns></returns>
        IComponent AddComponent(IEntity entity, IComponent component, bool useSame);
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IComponent AddComponent(IEntity entity, Type type);
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IComponent GetComponent(IEntity entity, Type type);
        /// <summary>
        /// 刷新组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <param name="component"></param>
        void ReFreshComponent(IEntity entity, Type type, IComponent component);
        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        void RemoveComponent(IEntity entity, Type type);


        /// <summary>
        /// 注册系统
        /// </summary>
        /// <param name="system"></param>
        void SubscribeSystem(IExcuteSystem system);
        /// <summary>
        /// 移除系统
        /// </summary>
        /// <param name="system"></param>
        void UnSubscribeSystem(IExcuteSystem system);
    }
}