using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFramework.Modules.MVP
{
    /// <summary>
    /// MVp组
    /// </summary>
    public class MVPGroup : IDisposable
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
        public string name { get { return _name; } }

        /// <summary>
        /// 实体
        /// </summary>
        public MVPEnity enity { get { return _enity; } }
        /// <summary>
        /// 消息监听系统
        /// </summary>
        public SensorSystem sensor { get { return _sensor; } }
        /// <summary>
        /// 消息决策系统
        /// </summary>
        public PolicySystem policy { get { return _policy; } }
        /// <summary>
        /// 决策执行系统
        /// </summary>
        public PolicyExecutorSystem executor { get { return _executor; } }
        /// <summary>
        /// 试图系统
        /// </summary>
        public ViewSystem view { get { return _view; } }
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
