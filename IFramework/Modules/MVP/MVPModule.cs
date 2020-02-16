using System;
using IFramework.Modules.Message;
using IFramework.Modules.ECS;
namespace IFramework.Modules.MVP
{
    /// <summary>
    /// MVP
    /// </summary>
    [FrameworkVersion(16)]
    public class MVPModule : FrameworkModule
    {
        private MessageModule _message;
        private ECSModule _ecs;
        private MVPEnity _enity;

        private SensorSystem _sensor;
        private PolicySystem _policy;
        private PolicyExecutorSystem _policyExecutor;
        private ViewSystem _view;
        /// <summary>
        /// 实体
        /// </summary>
        public MVPEnity enity
        {
            get { return _enity; }
            set
            {
                if (_enity != null)
                    _ecs.UnSubscribeEnity(_enity);
                _enity = value;
                _ecs.SubscribeEnity(_enity);
                if (_policy != null) _policy.enity = _enity;
                if (_policyExecutor != null) _policyExecutor.enity = _enity;
                if (_sensor != null) _sensor.enity = _enity;
                if (_view != null) _view.enity = _enity;
            }
        }
        /// <summary>
        /// 消息监听系统
        /// </summary>
        public SensorSystem sensor
        {
            get { return _sensor; }
            set
            {
                if (_sensor != null)
                    _ecs.UnSubscribeSystem(_sensor);
                _sensor = value;
                if (_enity != null) _sensor.enity = _enity;
                _sensor.message = _message;
                _ecs.SubscribeSystem(_sensor);
            }
        }
        /// <summary>
        /// 消息决策系统
        /// </summary>
        public PolicySystem policy
        {
            get { return _policy; }
            set
            {
                if (_policy != null)
                    _ecs.UnSubscribeSystem(_policy);
                _policy = value;
                if (_enity != null) _policy.enity = _enity;
                _policy.message = _message;
                _ecs.SubscribeSystem(_policy);
            }
        }
        /// <summary>
        /// 决策执行系统
        /// </summary>
        public PolicyExecutorSystem policyExecutor
        {
            get { return _policyExecutor; }
            set
            {
                if (_policyExecutor != null)
                    _ecs.UnSubscribeSystem(_policyExecutor);
                _policyExecutor = value;
                if (_enity != null) _policyExecutor.enity = _enity;
                _policyExecutor.message = _message;
                _ecs.SubscribeSystem(_policyExecutor);
            }
        }
        /// <summary>
        /// 试图系统
        /// </summary>
        public ViewSystem view
        {
            get { return _view; }
            set
            {
                if (_view != null)
                    _ecs.UnSubscribeSystem(_view);
                _view = value;
                if (_enity != null) _view.enity = _enity;
                _view.message = _message;
                _ecs.SubscribeSystem(_view);
            }
        }


#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override void Awake()
        {
            _message = CreatInstance<MessageModule>(this.chunck);
            _ecs = CreatInstance<ECSModule>(this.chunck);

        }
        protected override void OnDispose()
        {
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
