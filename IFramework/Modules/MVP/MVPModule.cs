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
    public class MVPModule : FrameworkModule
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

                _ecs.SubscribeEnity(group.enity);
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
                _ecs.UnSubscribeEnity(_group.enity);

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
    /// <summary>
    /// MVp组
    /// </summary>
    public class MVPGroup:IDisposable
    {

        private MVPEnity _enity;

        private SensorSystem _sensor;
        private PolicySystem _policy;
        private PolicyExecutorSystem _executor;
        private ViewSystem _view;
        private string _name;
        internal MVPModule module { get; set; }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="enity"></param>
        /// <param name="sensor"></param>
        /// <param name="policy"></param>
        /// <param name="executor"></param>
        /// <param name="view"></param>
        /// <param name="name"></param>
        public MVPGroup(MVPEnity enity, SensorSystem sensor, PolicySystem policy, PolicyExecutorSystem executor, ViewSystem view, string name)
        {
            this._enity = enity;
            this._sensor = sensor;
            this._policy = policy;
            this._executor = executor;
            this._view = view;
            this._name = name;

            if (_enity != null)
            {
                _sensor.enity = _enity;
                _policy.enity = _enity;
                _policy.enity = _enity;
                _view.enity = _enity;
            } 
        }
        /// <summary>
        /// 名字
        /// </summary>
        public string name { get { return _name; }  }

        /// <summary>
        /// 实体
        /// </summary>
        public MVPEnity enity{ get { return _enity; }}
        /// <summary>
        /// 消息监听系统
        /// </summary>
        public SensorSystem sensor{get { return _sensor; }}
        /// <summary>
        /// 消息决策系统
        /// </summary>
        public PolicySystem policy{ get { return _policy; }}
        /// <summary>
        /// 决策执行系统
        /// </summary>
        public PolicyExecutorSystem executor{ get { return _executor; }}
        /// <summary>
        /// 试图系统
        /// </summary>
        public ViewSystem view{ get { return _view; }}
        /// <summary>
        /// 释放,并且从模块中移除
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            if (module != null)

                module.RemoveGroup(name);
            if (_sensor != null) _sensor.GroupDispose();
            if (_policy != null) _policy.GroupDispose();
            if (_executor != null) _executor.GroupDispose();
            if (_view != null) _view.GroupDispose();
            if (_enity != null) _enity.Destory();
        }
        /// <summary>
        /// 释放时
        /// </summary>
        protected void OnDispose() { }
    }
}
