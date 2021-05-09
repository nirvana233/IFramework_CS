using IFramework.Injection;
using IFramework.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IFramework
{
    /// <summary>
    /// 框架运行环境
    /// </summary>
    class FrameworkEnvironment : DisposableObject, IEnvironment
    {
        private bool _inited;
        private FrameworkModules _modules;
        private Stopwatch sw_init;
        private Stopwatch sw_delta;
        private EnvironmentType _envType;
        private LoomModule _loom;
        private event Action update;
        private event Action onDispose;
        private static LockParam _envsetlock = new LockParam();
        private static FrameworkEnvironment _current;
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
        public IFrameworkModules modules { get { return _modules; } }
        /// <summary>
        /// 环境类型
        /// </summary>
        public EnvironmentType envType { get { return _envType; } }

        /// <summary>
        /// 最近一次 Update 方法用时
        /// </summary>
        public TimeSpan deltaTime { get; private set; }
        /// <summary>
        /// 初始化之后的时间
        /// </summary>
        public TimeSpan timeSinceInit
        {
            get
            {
                if (sw_init == null) return TimeSpan.Zero;
                sw_init.Stop();
                var span = sw_init.Elapsed;
                sw_init.Start();
                return span;
            }
        }


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
            _modules = new FrameworkModules(this);
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
            _loom = modules.CreateModule<LoomModule>("");
            deltaTime = TimeSpan.Zero;
            _inited = true;
            sw_delta = new Stopwatch();
            sw_init = new Stopwatch();
            sw_init.Start();
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
            (cycleCollection as RecyclableObjectCollection).Dispose();
            container.Dispose();
            sw_init.Stop();
            sw_delta.Stop();
            container = null;
            sw_delta = null;
            sw_init = null;
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
            sw_delta.Reset();
            sw_delta.Start();
            if (update != null) update();
            sw_delta.Stop();
            deltaTime = sw_delta.Elapsed;
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
