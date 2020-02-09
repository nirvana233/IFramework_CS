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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyVersion("0.0.0.2")]
namespace IFramework { }
namespace IFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OnFrameworkInitClassAttribute : Attribute { }

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

        internal FrameworkModules() : base("Framework",true)
        {

        }

    }
   

    public static class Framework
    {
        private static bool _haveInit;
        private static bool _disposed;
        private static FrameworkModules _modules;
        private static Stopwatch sw_init;
        private static Stopwatch sw_delta;

        public static RecyclableObjectPool cyclePool { get; private set; }

        public static event Action update;
        public static event Action onInit;
        public static event Action onDispose;

        public const string FrameworkName = "IFramework";
        public const string Author = "OnClick";
        public const string Version = "0.0.1";
        public const string Description = FrameworkName;

        public static bool haveInit { get { return _haveInit; } }
        public static bool disposed { get { return _disposed; } }
        public static IFrameworkContainer Container { get; set; }
        public static FrameworkModules modules { get { return _modules; } }


        public static TimeSpan deltaTime { get; private set; }
        public static TimeSpan timeSinceInit
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

        public static void Init()
        {
            if (_haveInit) return;
            Container = new FrameworkContainer();
            _modules = new FrameworkModules();
            cyclePool = new RecyclableObjectPool();

            AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(item => item.GetTypes())
                            .Where(item => item.IsDefined(typeof(OnFrameworkInitClassAttribute), false))
                            .ForEach((type) => {
                                TypeAttributes ta = type.Attributes;
                                if (ta.HasFlag(TypeAttributes.Abstract) && ta.HasFlag(TypeAttributes.Sealed))
                                {
                                    // type.TypeInitializer.Invoke(null, null);
                                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                                }
                            });
            if (onInit != null) onInit();

            deltaTime = TimeSpan.Zero;
            _disposed = false;
            _haveInit = true;
            sw_delta = new Stopwatch();
            sw_init = new Stopwatch();
            sw_init.Start();
        }
        public static void Dispose()
        {
            if (_disposed) return;
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

        public static void Update()
        {
            if (_disposed) return;
            sw_delta.Restart();
            if (update != null) update();
            sw_delta.Stop();
            deltaTime = sw_delta.Elapsed;
        }






        public static void BindFrameworkUpdate(this Action action)
        {
            update += action;
        }
        public static void UnBindFrameworkUpdate(this Action action)
        {
            update -= action;
        }
        public static void BindFrameworkDispose(this Action action)
        {
            onDispose += action;
        }
        public static void UnBindFrameworkDispose(this Action action)
        {
            onDispose -= action;
        }

    }

}
