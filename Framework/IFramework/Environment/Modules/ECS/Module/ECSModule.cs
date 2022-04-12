using System;
using System.Collections.Generic;

namespace IFramework.Modules.ECS
{
    /// <summary>
    /// 模仿Ecs结构
    /// </summary>
    public partial class ECSModule : UpdateModule, IECSModule
    {
        private Systems _systems;
        private Entitys _entitys;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override ModulePriority OnGetDefaulyPriority()
        {
            return ModulePriority.ECS;
        }
        protected override void Awake()
        {
            _systems = new Systems();
            _entitys = new Entitys(this);
        }
        protected override void OnDispose()
        {
            _systems.Dispose();
            _entitys.Dispose();
            _systems = null;
            _entitys = null;
        }
        protected override void OnUpdate()
        {
            _systems.Update();
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释


        private IComponent CreateComponent(Type type)
        {
            return Activator.CreateInstance(type) as IComponent;
        }
        /// <summary>
        /// 创建实体，创建完，注册
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity CreateEntity<TEntity>() where TEntity : IEntity
        {
            TEntity entity = Activator.CreateInstance<TEntity>();
            SubscribeEntity(entity);
            return entity;
        }
        /// <summary>
        /// 注册实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public void SubscribeEntity<TEntity>(TEntity entity) where TEntity : IEntity
        {
            _entitys.SubscribeEntity(entity);
        }
        /// <summary>
        /// 解除注册实体
        /// </summary>
        /// <param name="entity"></param>
        public void UnSubscribeEntity(IEntity entity)
        {
            _entitys.UnSubscribeEntity(entity);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEntity> GetEntitys()
        {
            return _entitys.GetEntitys();
        }


        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IComponent AddComponent(IEntity entity, Type type)
        {
            return _entitys.AddComponent(entity, type);
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <param name="useSame"></param>
        /// <returns></returns>
        public IComponent AddComponent(IEntity entity, IComponent component, bool useSame)
        {
            return _entitys.AddComponent(entity, component, useSame);
        }
        /// <summary>
        /// 刷新组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <param name="component"></param>
        public void ReFreshComponent(IEntity entity, Type type, IComponent component)
        {
            _entitys.ReFreshComponent(entity, type, component);
        }
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IComponent GetComponent(IEntity entity, Type type)
        {
            return _entitys.GetComponent(entity, type);
        }
        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        public void RemoveComponent(IEntity entity, Type type)
        {
            _entitys.RemoveComponent(entity, type);
        }


        /// <summary>
        /// 注册系统
        /// </summary>
        /// <param name="system"></param>
        public void SubscribeSystem(IExcuteSystem system)
        {
            _systems.AddSystem(system);
        }
        /// <summary>
        /// 解除注册系统
        /// </summary>
        /// <param name="system"></param>
        public void UnSubscribeSystem(IExcuteSystem system)
        {
            _systems.RemoveSystem(system);
        }

    }
}
