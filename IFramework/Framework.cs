using IFramework.Modules;
using IFramework.Modules.APP;
using IFramework.Modules.Coroutine;
using IFramework.Modules.ECS;
using IFramework.Modules.Fsm;
using IFramework.Modules.Loom;
using IFramework.Modules.Message;
using IFramework.Modules.MVP;
using IFramework.Modules.Pool;
using IFramework.Modules.Threads;
using IFramework.Modules.Timer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyVersion("0.0.0.2")]
namespace IFramework { }
namespace IFramework
{
    /// <summary>
    /// 框架代码版本默认有 1
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    [FrameworkVersion(2)]
    public class FrameworkVersionAttribute:Attribute
    {
        /// <summary>
        /// 版本
        /// </summary>
        public int version { get; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="version"></param>
        public FrameworkVersionAttribute(int version=1)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        {
            this.version = version;
        }
    }
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
        public int type { get; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="type"></param>
        public OnFrameworkInitClassAttribute(int type=0)
        {
            this.type = type;
        }
    }
    /// <summary>
    /// 框架提供的模块
    /// </summary>
    [FrameworkVersion(6)]
    public class FrameworkModules : FrameworkModuleContainer
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public FsmModule Fsm { get; set; }
        public TimerModule Timer { get; set; }
        public LoomModule Loom { get; set; }
        public CoroutineModule Coroutine { get; set; }
        public MessageModule Message { get; set; }
        public FrameworkAppModule App { get; set; }
        public PoolModule Pool { get; set; }
        public ThreadModule ThreadPool { get; set; }
        public ECSModule ECS { get; set; }
        public MVPModule MVP { get; set; }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        internal FrameworkModules(FrameworkEnvironment env) : base("Framework", env, true)
        {

        }

    }

    /// <summary>
    /// 框架运行环境
    /// </summary>
    [FrameworkVersion(10)]
    public class FrameworkEnvironment : IDisposable
    {
        private bool _haveInit;
        private bool _disposed;
        private FrameworkModules _modules;
        private string _envName;
        private Stopwatch sw_init;
        private Stopwatch sw_delta;
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
        /// <returns></returns>
        public static FrameworkEnvironment CreateInstance(string envName)
        {
            return new FrameworkEnvironment(envName);
        }

        private FrameworkEnvironment(string envName)
        {
            this._envName = envName;
        }
        /// <summary>
        /// 初始化环境，3.5 使用
        /// </summary>
        /// <param name="types">需要初始化调用的静态类</param>
        public void init(IEnumerable<Type> types)
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
        /// <param name="includeTypes">被标记 OnFrameworkInitClassAttribute  静态类 的版本，默认有 0 </param>
        public void Init(int[] includeTypes = null)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                              .SelectMany(item => item.GetTypes())
                              .Where(item => item.IsDefined(typeof(OnFrameworkInitClassAttribute), false))
                              .Select((type) =>
                              {
                                  var attr = type.GetCustomAttributes(typeof(OnFrameworkInitClassAttribute), false).First() as OnFrameworkInitClassAttribute;
                                  if (attr.type == 0)
                                      return type;

                                  if (includeTypes != null && includeTypes.Length > 0)
                                  {
                                      for (int i = 0; i < includeTypes.Length; i++)
                                      {
                                          if (includeTypes[i] == attr.type)
                                              return type;
                                      }
                                  }
                                  return null;
                              }).ToList();
            types.RemoveAll((type) => { return type == null; });
            init(types);
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
    /// <summary>
    /// 框架入口
    /// </summary>
    public static class Framework
    {
        static Framework()
        {
            CalcVersion();
        }
            
        private static string CalcVersion()
        {
            int sum = 0;
            Version = "";
            AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(item => item.GetTypes())
                             .ForEach((type) =>
                             {
                                 if (!type.FullName.Contains(FrameworkName)) return;
                                 if (type.IsDefined(typeof(FrameworkVersionAttribute), false))
                                 {
                                     FrameworkVersionAttribute attr = type.GetCustomAttributes(typeof(FrameworkVersionAttribute), false).First() as FrameworkVersionAttribute;
                                     sum += attr.version;
                                 }
                                 else
                                     sum += 1;
                             });
            Log.E(sum);
            int mul = 1000;
            do
            {
                float tval = sum % mul;
                Version = Version.AppendHead(string.Format(".{0}", tval));
                sum = sum / mul;
            } while (sum > 0);
            Version = Version.Substring(1);
            int tmp=4-  Version.Split('.').Length;
            for (int i = 0; i < tmp; i++)
                Version = Version.AppendHead("0.");
            return Version;
        }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public const string FrameworkName = "IFramework";
        public const string Author = "OnClick";
        public static string Version;
        public const string Description = FrameworkName;


        public static FrameworkEnvironment env0;
        public static FrameworkEnvironment env1;
        public static FrameworkEnvironment env2;
        public static FrameworkEnvironment env3;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释


        /// <summary>
        /// 创建一个环境
        /// </summary>
        /// <param name="envName">环境名称</param>
        /// <returns>环境</returns>
        public static FrameworkEnvironment CreateEnv(string envName)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        {
            return FrameworkEnvironment.CreateInstance(envName);
        }
        /// <summary>
        /// 实例化环境
        /// </summary>
        /// <param name="envName">环境名</param>
        /// <param name="envIndex">环境序号</param>
        /// <returns>环境</returns>
        public static FrameworkEnvironment InitEnv(string envName, int envIndex)
        {
            switch (envIndex)
            {
                case 0: env0 = CreateEnv(envName); return env0;
                case 1: env1 = CreateEnv(envName); return env1;
                case 2: env2 = CreateEnv(envName); return env3;
                case 3: env3 = CreateEnv(envName); return env3;
                default: throw new Exception(string.Format("The Index {0} Error,Please Between [ 0 , 3 ]", envIndex));
            }
           
        }
        /// <summary>
        /// 根据序号获取环境
        /// </summary>
        /// <param name="envIndex"></param>
        /// <returns></returns>
        public static FrameworkEnvironment GetEnv(int envIndex)
        {
            switch (envIndex)
            {
                case 0: return env0;
                case 1: return env1;
                case 2: return env3;
                case 3: return env3;
                default: throw new Exception(string.Format("The Index {0} Error,Please Between [ 0 , 3 ]", envIndex));
            }
        }
        /// <summary>
        /// 绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void BindEnvUpdate(this Action action, FrameworkEnvironment env)
        {
            env.update += action;
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void UnBindEnvUpdate(this Action action, FrameworkEnvironment env)
        {
            env.update -= action;
        }
        /// <summary>
        /// 绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void BindEnvDispose(this Action action, FrameworkEnvironment env)
        {
            env.onDispose += action;
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void UnBindEnvDispose(this Action action, FrameworkEnvironment env)
        {
            env.onDispose -= action;
        }

        /// <summary>
        /// 绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="envIndex">环境序号</param>
        public static void BindEnvUpdate(this Action action, int envIndex)
        {
            action.BindEnvUpdate(GetEnv(envIndex));
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="envIndex">环境序号</param>
        public static void UnBindEnvUpdate(this Action action, int envIndex)
        {
            action.UnBindEnvUpdate(GetEnv(envIndex));
        }
        /// <summary>
        /// 绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="envIndex">环境序号</param>
        public static void BindEnvDispose(this Action action, int envIndex)
        {
            action.BindEnvDispose(GetEnv(envIndex));
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="envIndex">环境序号</param>
        public static void UnBindEnvDispose(this Action action, int envIndex)
        {
            action.UnBindEnvDispose(GetEnv(envIndex));
        }
    }



   

}
