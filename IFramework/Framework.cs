using IFramework.Moudles;
using IFramework.Moudles.APP;
using IFramework.Moudles.Coroutine;
using IFramework.Moudles.Fsm;
using IFramework.Moudles.Loom;
using IFramework.Moudles.Message;
using IFramework.Moudles.Pool;
using IFramework.Moudles.Threads;
using IFramework.Moudles.Timer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyVersion("0.0.0.1")]
namespace IFramework { }
namespace IFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OnFrameworkInitClassAttribute : Attribute { }

    public class FrameworkMoudles : FrameworkMoudleContainer, IFrameworkMoudles
    {
        public FsmMoudle Fsm { get; set; }
        public TimerMoudle Timer { get; set; }
        public LoomMoudle Loom { get; set; }
        public CoroutineMoudle Coroutine { get; set; }
        public MessageMoudle Message { get; set; }
        public FrameworkAppMoudle App { get; set; }
        public PoolMoudle Pool { get; set; }
        public ThreadMoudle ThreadPool { get; set; }

        internal FrameworkMoudles() : base("Framework",true)
        {

        }

    }
    internal class FrameworkArgsPool : IDisposable
        {
            interface IFrameworkArgsInnerPool { }
            class FrameworkArgsInnerPool<Args> : ObjectPool<Args>, IFrameworkArgsInnerPool where Args : FrameworkArgs
            {
                protected override Args CreatNew(IEventArgs arg, params object[] param)
                {
                    return Activator.CreateInstance<Args>();
                }
            }

            private Dictionary<Type, IFrameworkArgsInnerPool> ArgMap;
            public FrameworkArgsPool()
            {
                ArgMap = new Dictionary<Type, IFrameworkArgsInnerPool>();
            }
            public void Dispose()
            {
                foreach (var item in ArgMap.Values)
                    (item as FrameworkArgsInnerPool<FrameworkArgs>).Dispose();
                ArgMap.Clear();
            }

            private FrameworkArgsInnerPool<FrameworkArgs> GetPool(Type type)
            {
                if (!ArgMap.ContainsKey(type))
                    ArgMap.Add(type, new FrameworkArgsInnerPool<FrameworkArgs>());
                return ArgMap[type] as FrameworkArgsInnerPool<FrameworkArgs>;
            }
            public T Allocate<T>() where T : FrameworkArgs
            {
                return GetPool(typeof(T)).Get() as T;
            }
            public void Set<T>(T t) where T : FrameworkArgs
            {
                GetPool(typeof(T)).Set(t);
            }
        }

    public static class Framework
    {
        private static bool _haveInit;
        private static bool _disposed;
        private static FrameworkMoudles _moudles;
        private static Stopwatch sw_init;
        private static Stopwatch sw_delta;

        internal static FrameworkArgsPool argPool { get; private set; }

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
        public static FrameworkMoudles moudles { get { return _moudles; } }


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
            _moudles = new FrameworkMoudles();
            argPool = new FrameworkArgsPool();

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
            _disposed = true;
            if (onDispose != null) onDispose();
            sw_init.Stop();
            sw_delta.Stop();
            Container.Dispose();
            argPool.Dispose();

            Container = null;
            sw_delta = null;
            sw_init = null;
            _moudles = null;
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


        public static void BindFramework(this FrameworkMoudle moudle)
        {
            moudle.Bind(moudles);
        }
        public static void UnBindFramework(this FrameworkMoudle moudle,bool dispose=true)
        {
            moudle.UnBind(dispose);
        }
    }

}
