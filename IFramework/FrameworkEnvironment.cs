using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IFramework
{
    /// <summary>
    /// 框架初始化时候调用被标记的静态类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [FrameworkVersion(2)]
    public class OnFrameworkInitClassAttribute : Attribute
    {
        /// <summary>
        /// 配合初始化的版本 0，
        /// 默认初始化，其他自行规定，用于区分环境，
        /// 一般某个环境特有的静态类和环境编号一致
        /// </summary>
        public EnvironmentType type { get; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="type"></param>
        public OnFrameworkInitClassAttribute(EnvironmentType type = EnvironmentType.None)
        {
            this.type = type;
        }
    }
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
    [FrameworkVersion(20)]
    public class FrameworkEnvironment : IDisposable
    {
        private bool _haveInit;
        private bool _disposed;
        private FrameworkModules _modules;
        private string _envName;
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
        /// 环境是否被释放
        /// </summary>
        public bool disposed { get { return _disposed; } }
        /// <summary>
        /// IRecyclable 实例的环境容器
        /// </summary>
        public RecyclableObjectPool cyclePool { get; private set; }
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
        public string envName { get { return _envName; } }
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
            this._envName = envName;
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
            cyclePool = new RecyclableObjectPool();
            if (types != null)
                types.ForEach((type) =>
                {
                    TypeAttributes ta = type.Attributes;
                    if (ta.HasFlag(TypeAttributes.Abstract) && ta.HasFlag(TypeAttributes.Sealed))
                        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                });

            if (onInit != null) onInit();
            deltaTime = TimeSpan.Zero;
            _disposed = false;
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
                              .Where(item => item.IsDefined(typeof(OnFrameworkInitClassAttribute), false))
                              .Select((type) =>
                              {
                                  var attr = type.GetCustomAttributes(typeof(OnFrameworkInitClassAttribute), false).First() as OnFrameworkInitClassAttribute;
                                  if (attr.type.HasFlag(this.envType) || attr.type.HasFlag(EnvironmentType.None))
                                      return type;
                                  return null;
                              }).ToList();
            types.RemoveAll((type) => { return type == null; });
            Init(types);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (_disposed || !haveInit) return;
            if (onDispose != null) onDispose();
            sw_init.Stop();
            sw_delta.Stop();

            container.Dispose();
            cyclePool.Dispose();

            _disposed = true;
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
            if (_disposed) return;
            sw_delta.Restart();
            if (update != null) update();
            sw_delta.Stop();
            deltaTime = sw_delta.Elapsed;
        }
    }
}
