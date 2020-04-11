using System.Collections.Generic;
using IFramework.Modules.Message;
using IFramework.Modules.ECS;
using System;
using System.Linq;

namespace IFramework.Modules.MVP
{
    /// <summary>
    /// MVP
    /// </summary>
    [FrameworkVersion(46)]
    [Dependence(typeof(MessageModule))]
    [Dependence(typeof(ECSModule))]
    public class MVPModule : UpdateFrameworkModule
    {
        private MessageModule _message;
        private ECSModule _ecs;

        private Dictionary<string, MVPGroup> _groupmap;
        /// <summary>
        /// 添加组
        /// </summary>
        /// <param name="group"></param>
        public void AddGroup(MVPGroup group)
        {
            MVPGroup _group = FindGroup(name);
            if (_group != null)
                throw new Exception("Have Add Group " + group.name);
            else
            {
                group.module = this;
                _groupmap.Add(group.name, group);
                group.sensor.message = _message;
                group.policy.message = _message;
                group.executor.message = _message;
                group.view.message = _message;

                _ecs.SubscribeEntity(group.entity);
                _ecs.SubscribeSystem(group.sensor);
                _ecs.SubscribeSystem(group.policy);
                _ecs.SubscribeSystem(group.executor);
                _ecs.SubscribeSystem(group.view);
            }
        }
        /// <summary>
        /// 移除组
        /// </summary>
        /// <param name="name"></param>
        public void RemoveGroup(string name)
        {
            MVPGroup _group=FindGroup(name);
            if (_group == null)
                throw new Exception("Have not Add Group " + name);
            else
            {
                _group.module = null;
                _ecs.UnSubscribeEntity(_group.entity);

                _ecs.UnSubscribeSystem(_group.sensor);
                _ecs.UnSubscribeSystem(_group.policy);
                _ecs.UnSubscribeSystem(_group.executor);
                _ecs.UnSubscribeSystem(_group.view);

                _group.sensor.message = null;
                _group.policy.message = null;
                _group.executor.message = null;
                _group.view.message = null;

                _groupmap.Remove(name);
            }
        }
        /// <summary>
        /// 移除组
        /// </summary>
        /// <param name="group"></param>
        public void RemoveGroup(MVPGroup group)
        {
            RemoveGroup(group.name);
        }
        /// <summary>
        /// 查找组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MVPGroup FindGroup(string name)
        {
            MVPGroup _group;
            _groupmap.TryGetValue(name, out _group);
            return _group;
        }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override void Awake()
        {
            _message = CreatInstance<MessageModule>(this.chunck);
            _ecs = CreatInstance<ECSModule>(this.chunck);
            _groupmap = new Dictionary<string, MVPGroup>();
        }
        protected override void OnDispose()
        {
            var em = _groupmap.Values.ToList();
            em.ForEach((e) =>
            {
                e.Dispose();
            });
            _groupmap = null;
            _ecs.Dispose();
            _message.Dispose();
        }
        protected override void OnUpdate()
        {
            _ecs.Update();
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
    
}
