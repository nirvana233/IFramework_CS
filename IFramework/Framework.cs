using IFramework.Moudles;
using IFramework.Moudles.Coroutine;
using IFramework.Moudles.Fsm;
using IFramework.Moudles.Loom;
using IFramework.Moudles.Message;
using IFramework.Moudles.Timer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OnFrameworkInitClassAttribute : Attribute { }

    public static class Framework
    {
        public const string FrameworkName = "IFramework";
        public const string Author = "OnClick";
        public const string Version = "0.0.1";
        public const string Description = FrameworkName;
        private static bool haveInit;
        public static bool HaveInit { get { return haveInit; } }
        private static bool _disposed;
        public static bool disposed { get { return _disposed; } }
        public static TimeSpan deltaTime { get; private set; }

        private static Stopwatch stopwatch2;

        public static TimeSpan timeSinceInit
        {
            get
            {
                if (stopwatch2 == null) return TimeSpan.Zero;
                stopwatch2.Stop();
                var span = stopwatch2.Elapsed;
                stopwatch2.Start();
                return span;
            }
        }
        private static Stopwatch stopwatch = new Stopwatch();

        public static event Action update;
        public static event Action onInit;
        public static event Action onDispose;

        public static IFrameworkContainer Container { get; set; }

        private static FrameworkMoudles _moudles;
        public static IFrameworkMoudles moudles { get { return _moudles; } }



        static Framework()
        {
            Container = new FrameworkContainer();
        }

        public static void Init()
        {
            if (haveInit) return;
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
            deltaTime = TimeSpan.Zero;
            _moudles = new FrameworkMoudles();
            if (onInit != null) onInit();
            _disposed = false;
            haveInit = true;
            stopwatch2 = new Stopwatch();
            stopwatch2.Start();
        }
        public static void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            stopwatch2.Stop();
            if (onDispose != null) onDispose();
            Container.Dispose();

            stopwatch2 = null;
            _moudles = null;
            onInit = null;
            update = null;
            onDispose = null;
        }
        public static void Update()
        {
            if (_disposed) return;
            stopwatch.Restart();
            if (update != null) update();
            stopwatch.Stop();
            deltaTime = stopwatch.Elapsed;
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
            _moudles.Bind(moudle);
        }
        public static void UnBindFramework(this FrameworkMoudle moudle)
        {
            _moudles.UnBind(moudle);
        }

        class FrameworkMoudles : IFrameworkMoudles, IDisposable
        {
            public FsmMoudle Fsm { get; set; }
            public TimerMoudle Timer { get; set; }
            public LoomMoudle Loom { get; set; }
            public CoroutineMoudle Coroutine { get; set; }
            public MessageMoudle Message { get; set; }

            private Dictionary<Type, List<FrameworkMoudle>> moudles;
            private List<FrameworkMoudle> mou;
            public FrameworkMoudles()
            {
                mou = new List<FrameworkMoudle>();
                moudles = new Dictionary<Type, List<FrameworkMoudle>>();
                Framework.update += Update;
                Framework.onDispose += Dispose;
            }
            public void Dispose()
            {
                Framework.update -= Update;
                Framework.onDispose -= Dispose;
                for (int i = 0; i < mou.Count; i++)
                {
                    var m = mou[i];
                    m.Dispose();
                }
                mou.Clear();
                moudles.Clear();
                mou = null;
                moudles = null;
            }
            protected void Update()
            {
                mou.ForEach((m) => { m.Update(); });
            }
            public void Bind(FrameworkMoudle moudle)
            {
                Type type = moudle.GetType();
                if (!moudles.ContainsKey(type))
                    moudles.Add(type, new List<FrameworkMoudle>());
                var list = moudles[type];
                var tmpMoudle = list.Find((m) => { return moudle.name == m.name; });
                if (tmpMoudle == null)
                {
                    list.Add(moudle);
                    mou.Add(moudle);
                }
                else
                    Log.E(string.Format("Have Bind Moudle | Type {0}  Name {1}", type, moudle.name));
            }
            public void UnBind(FrameworkMoudle moudle)
            {
                Type type = moudle.GetType();
                if (moudles.ContainsKey(type))
                    Log.E(string.Format("Have Not Bind Moudle | Type {0}  Name {1}", type, moudle.name));
                else
                {
                    var list = moudles[type];
                    var tmpMoudle = list.Find((m) => { return moudle == m; });
                    if (tmpMoudle == null)
                        Log.E(string.Format("Have Not Bind Moudle | Type {0}  Name {1}", type, moudle.name));
                    else
                    {
                        mou.Remove(moudle);
                        list.Remove(moudle);
                    }
                }
            }
            public FrameworkMoudle this[Type type, string name]
            {
                get { return FindMoudle(type, name); }
            }
            public FrameworkMoudle FindMoudle(Type type, string name)
            {
                if (!moudles.ContainsKey(type))
                    return default(FrameworkMoudle);
                return moudles[type].Find((m) => { return m.name == name; });
            }
            public T FindMoudle<T>(string name) where T : FrameworkMoudle
            {
                return FindMoudle(typeof(T), name) as T;
            }
        }
    }
}
