using IFramework.Inject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IFramework
{
    /// <summary>
    /// 环境类型
    /// </summary>
    [Flags]
    public enum EnvironmentType : int
    {
        /// <summary>
        /// 所有，配合环境初始化
        /// </summary>
        None = 1,
        /// <summary>
        /// 环境0
        /// </summary>
        Ev0 = 2,
        /// <summary>
        /// 环境1
        /// </summary>
        Ev1 = 4,
        /// <summary>
        /// 环境2
        /// </summary>
        Ev2 = 8,
        /// <summary>
        /// 环境3
        /// </summary>
        Ev3 = 16,
        /// <summary>
        /// 环境4
        /// </summary>
        Ev4 = 32
    }
    /// <summary>
    /// 框架运行环境
    /// </summary>
    [FrameworkVersion(22)]
    [ScriptVersionUpdate(21, "替换可回收对象池子类型")]
    [ScriptVersionUpdate(22, "调整释放时候的成员顺序")]
    public class FrameworkEnvironment : FrameworkObject
    {
        private bool _haveInit;
        private FrameworkModules _modules;
        private Stopwatch sw_init;
        private Stopwatch sw_delta;
        private EnvironmentType _envType;

        /// <summary>
        /// Update 方法的回调
        /// </summary>
        public event Action update;
        /// <summary>
        /// 环境初始化回调
        /// </summary>
        public event Action onInit;
        /// <summary>
        /// 环境释放的回调
        /// </summary>
        public event Action onDispose;
        /// <summary>
        /// 环境是否已经初始化
        /// </summary>
        public bool haveInit { get { return _haveInit; } }

        /// <summary>
        /// IRecyclable 实例的环境容器
        /// </summary>
        public RecyclableObjectCollection cycleCollection { get; private set; }
        /// <summary>
        /// IOC容器
        /// </summary>
        public IFrameworkContainer container { get; set; }
        /// <summary>
        /// 环境下自带的模块容器
        /// </summary>
        public FrameworkModules modules { get { return _modules; } }
        /// <summary>
        /// 环境类型
        /// </summary>
        public EnvironmentType envType { get { return _envType; } }
        /// <summary>
        /// 环境名称
        /// </summary>
        public string envName { get { return name; } set { name = value; } }
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
        /// 创建一个 环境
        /// </summary>
        /// <param name="envName">环境名称</param>
        /// <param name="envType">环境类型</param>
        /// <returns></returns>
        public static FrameworkEnvironment CreateInstance(string envName, EnvironmentType envType)
        {
            return new FrameworkEnvironment(envName, envType);
        }

        private FrameworkEnvironment(string envName, EnvironmentType envType)
        {
            this.envName = envName;
            this._envType = envType;
        }
        /// <summary>
        /// 初始化环境，3.5 使用
        /// </summary>
        /// <param name="types">需要初始化调用的静态类</param>
        public void Init(IEnumerable<Type> types)
        {
            if (_haveInit) return;

            container = new FrameworkContainer();
            _modules = new FrameworkModules(this);
            cycleCollection = new RecyclableObjectCollection();
            if (types != null)
                types.ForEach((type) =>
                {
                    TypeAttributes ta = type.Attributes;
                    if ((ta & TypeAttributes.Abstract) != 0 && ((ta & TypeAttributes.Sealed) != 0))
                        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                });

            if (onInit != null) onInit();
            deltaTime = TimeSpan.Zero;
            _haveInit = true;
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
            base.OnDispose();
            if (disposed || !haveInit) return;

            cycleCollection.Dispose();
            container.Dispose();
            if (onDispose != null) onDispose();
            sw_init.Stop();
            sw_delta.Stop();

            container = null;
            sw_delta = null;
            sw_init = null;
            _modules = null;
            onInit = null;
            update = null;
            onDispose = null;
        }

        /// <summary>
        /// 刷新环境
        /// </summary>
        public void Update()
        {
            if (disposed) return;
            sw_delta.Reset();
            sw_delta.Start();
            if (update != null) update();
            sw_delta.Stop();
            deltaTime = sw_delta.Elapsed;
        }
    }
}
