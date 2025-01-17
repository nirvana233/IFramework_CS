﻿using IFramework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IFramework
{
    /// <summary>
    /// 框架运行环境
    /// </summary>
    class FrameworkEnvironment : Unit, IEnvironment
    {
        private bool _inited;
        private Modules.Modules _modules;
        private EnvironmentType _envType;
        private LoomModule _loom;
        private event Action update;
        private event Action onDispose;
        private static LockParam _envsetlock = new LockParam();
        private static FrameworkEnvironment _current;
        private TimeCalculator _time;
        /// <summary>
        /// 环境是否已经初始化
        /// </summary>
        public bool inited { get { return _inited; } }
        /// <summary>
        /// IRecyclable 实例的环境容器
        /// </summary>
        internal RecyclableObjectCollection cycleCollection { get; private set; }
        /// <summary>
        /// IOC容器
        /// </summary>
        public IValuesContainer container { get; set; }
        /// <summary>
        /// 环境下自带的模块容器
        /// </summary>
        public IModules modules { get { return _modules; } }
        /// <summary>
        /// 环境类型
        /// </summary>
        public EnvironmentType envType { get { return _envType; } }



        /// <summary>
        /// 当前环境
        /// </summary>
        public static FrameworkEnvironment current
        {
            get { return _current; }
            private set
            {
                using (new LockWait(ref _envsetlock))
                {
                    _current = value;
                }
            }
        }

        public ITimeCalculator time { get { return _time; } }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="envType"></param>
        public FrameworkEnvironment(EnvironmentType envType)
        {
            this._envType = envType;
        }

        /// <summary>
        /// 初始化环境，3.5 使用
        /// </summary>
        /// <param name="types">需要初始化调用的静态类</param>
        public void Init(IEnumerable<Type> types)
        {
            if (_inited) return;
            current = this;
            container = new ValuesContainer();
            _modules = new Modules.Modules(this);
            cycleCollection = new RecyclableObjectCollection();
            if (types != null)
            {
                foreach (var type in types)
                {
                    TypeAttributes ta = type.Attributes;
                    if ((ta & TypeAttributes.Abstract) != 0 && ((ta & TypeAttributes.Sealed) != 0))
                        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
            }
            _loom = LoomModule.CreatInstance<LoomModule>("");
            _time = new TimeCalculator();
            _inited = true;
        }
        /// <summary>
        /// 初始化环境，4.X 使用
        /// </summary>
        public void InitWithAttribute()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                              .SelectMany(item => item.GetTypes())
                              .Where(item => item.IsDefined(typeof(OnEnvironmentInitAttribute), false))
                              .Select((type) =>
                              {
                                  var attr = type.GetCustomAttributes(typeof(OnEnvironmentInitAttribute), false).First() as OnEnvironmentInitAttribute;
                                  var _type = attr.type;
                                  if ((_type & this.envType) != 0 || (_type & EnvironmentType.None) != 0)
                                      return type;
                                  return null;
                              }).ToList();
            types.RemoveAll((type) => { return type == null; });
            Init(types);
        }
        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            if (disposed || !inited) return;

            if (onDispose != null) onDispose();
            _modules.Dispose();
            (cycleCollection as RecyclableObjectCollection).Dispose();
            container.Dispose();
            _loom.Dispose();
            _time.Dispose();
            container = null;
            _modules = null;
            update = null;
            onDispose = null;
        }
        /// <summary>
        /// 刷新环境
        /// </summary>
        public void Update()
        {
            if (disposed) return;
            current = this;
            _time.BeginDelta();
            _loom.Update();
            _modules.Update();
            if (update != null) update();
            _time.EndDelta();
        }

        /// <summary>
        /// 等待 环境的 update，即等到该环境的线程来临
        /// </summary>
        /// <param name="action"></param>
        public void WaitEnvironmentFrame(Action action)
        {
            if (disposed) return;
            _loom.RunDelay(action);
        }

        /// <summary>
        /// 绑定帧
        /// </summary>
        /// <param name="action"></param>
        public void BindUpdate(Action action)
        {
            if (disposed) return;
            update += action;
        }
        /// <summary>
        /// 移除绑定帧
        /// </summary>
        /// <param name="action"></param>
        public void UnBindUpdate(Action action)
        {
            if (disposed) return;
            update -= action;
        }
        /// <summary>
        /// 绑定dispose
        /// </summary>
        /// <param name="action"></param>
        public void BindDispose(Action action)
        {
            if (disposed) return;
            onDispose += action;
        }
        /// <summary>
        /// 移除绑定dispose
        /// </summary>
        /// <param name="action"></param>
        public void UnBindDispose(Action action)
        {
            if (disposed) return;
            onDispose -= action;
        }
    }
}
