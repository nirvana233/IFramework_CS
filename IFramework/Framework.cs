using IFramework.Modules;
using IFramework.Modules.APP;
using IFramework.Modules.Coroutine;
using IFramework.Modules.ECS;
using IFramework.Modules.Fsm;
using IFramework.Modules.Loom;
using IFramework.Modules.Message;
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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [FrameworkVersion(2)]
    public class OnFrameworkInitClassAttribute : Attribute
    {
        public OnFrameworkInitClassAttribute() : this(0)
        {
        }
        public OnFrameworkInitClassAttribute(int type)
        {
            Type = type;
        }

        public int Type { get; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    [FrameworkVersion(2)]
    public class FrameworkVersionAttribute:Attribute
    {
        public FrameworkVersionAttribute(int version=1)
        {
            this.version = version;
        }
        public int version { get; }
    }
    [FrameworkVersion(4)]
    public class FrameworkModules : FrameworkModuleContainer, IFrameworkModules
    {
        public FsmModule Fsm { get; set; }
        public TimerModule Timer { get; set; }
        public LoomModule Loom { get; set; }
        public CoroutineModule Coroutine { get; set; }
        public MessageModule Message { get; set; }
        public FrameworkAppModule App { get; set; }
        public PoolModule Pool { get; set; }
        public ThreadModule ThreadPool { get; set; }
        public ECSModule ECS { get; set; }

        internal FrameworkModules(FrameworkEnvironment env) : base("Framework", env, true)
        {

        }

    }

    [FrameworkVersion(10)]
    public class FrameworkEnvironment : IDisposable
    {
        private bool _haveInit;
        private bool _disposed;
        private FrameworkModules _modules;
        private string _envName;
        private Stopwatch sw_init;
        private Stopwatch sw_delta;
        public event Action update;
        public event Action onInit;
        public event Action onDispose;
        public bool haveInit { get { return _haveInit; } }
        public bool disposed { get { return _disposed; } }
        public RecyclableObjectPool cyclePool { get; private set; }

        public IFrameworkContainer Container { get; set; }
        public FrameworkModules modules { get { return _modules; } }
        public string envName { get { return _envName; } }
        public TimeSpan deltaTime { get; private set; }
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

        public static FrameworkEnvironment CreateInstance(string envName)
        {
            return new FrameworkEnvironment(envName);
        }

        protected FrameworkEnvironment(string envName)
        {
            this._envName = envName;
        }
        public void init(IEnumerable<Type> types)
        {
            if (_haveInit) return;

            Container = new FrameworkContainer();
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
        public void Init(int[] includeTypes = null)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                              .SelectMany(item => item.GetTypes())
                              .Where(item => item.IsDefined(typeof(OnFrameworkInitClassAttribute), false))
                              .Select((type) =>
                              {
                                  var attr = type.GetCustomAttributes(typeof(OnFrameworkInitClassAttribute), false).First() as OnFrameworkInitClassAttribute;
                                  if (attr.Type == 0)
                                      return type;

                                  if (includeTypes != null && includeTypes.Length > 0)
                                  {
                                      for (int i = 0; i < includeTypes.Length; i++)
                                      {
                                          if (includeTypes[i] == attr.Type)
                                              return type;
                                      }
                                  }
                                  return null;
                              }).ToList();
            types.RemoveAll((type) => { return type == null; });
            init(types);
        }


        public void Dispose()
        {
            if (_disposed || !haveInit) return;
            if (onDispose != null) onDispose();
            sw_init.Stop();
            sw_delta.Stop();

            Container.Dispose();
            cyclePool.Dispose();

            _disposed = true;
            Container = null;
            sw_delta = null;
            sw_init = null;
            _modules = null;
            onInit = null;
            update = null;
            onDispose = null;
        }

        public void Update()
        {
            if (_disposed) return;
            sw_delta.Restart();
            if (update != null) update();
            sw_delta.Stop();
            deltaTime = sw_delta.Elapsed;
        }
    }
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

        public const string FrameworkName = "IFramework";
        public const string Author = "OnClick";
        public static string Version ;
        public const string Description = FrameworkName;


        public static FrameworkEnvironment env0;
        public static FrameworkEnvironment env1;
        public static FrameworkEnvironment env2;
        public static FrameworkEnvironment env3;

     
        public static FrameworkEnvironment CreateEnv(string envName)
        {
            return FrameworkEnvironment.CreateInstance(envName);
        }
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

        public static void BindEnvUpdate(this Action action, FrameworkEnvironment env)
        {
            env.update += action;
        }
        public static void UnBindEnvUpdate(this Action action, FrameworkEnvironment env)
        {
            env.update -= action;
        }
        public static void BindEnvDispose(this Action action, FrameworkEnvironment env)
        {
            env.onDispose += action;
        }
        public static void UnBindEnvDispose(this Action action, FrameworkEnvironment env)
        {
            env.onDispose -= action;
        }
        public static void BindEnvUpdate(this Action action, int envIndex)
        {
            action.BindEnvUpdate(GetEnv(envIndex));
        }
        public static void UnBindEnvUpdate(this Action action, int envIndex)
        {
            action.UnBindEnvUpdate(GetEnv(envIndex));
        }
        public static void BindEnvDispose(this Action action, int envIndex)
        {
            action.BindEnvDispose(GetEnv(envIndex));
        }
        public static void UnBindEnvDispose(this Action action, int envIndex)
        {
            action.UnBindEnvDispose(GetEnv(envIndex));
        }
    }

}
