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

[assembly: AssemblyVersion("0.0.0.1")]
namespace IFramework { }
namespace IFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OnFrameworkInitClassAttribute : Attribute { }

    public static class Framework
    {
        private static bool _haveInit;
        private static bool _disposed;
        private static FrameworkMoudles _moudles;
        private static Stopwatch sw_init;
        private static Stopwatch sw_delta;

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
        public static IFrameworkMoudles moudles { get { return _moudles; } }
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





        static Framework()
        {
            Container = new FrameworkContainer();
        }

        public static void Init()
        {
            if (_haveInit) return;
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
            _moudles.Bind(moudle);
        }
        public static void UnBindFramework(this FrameworkMoudle moudle,bool dispose=true)
        {
            _moudles.UnBind(moudle,dispose);
        }

        class FrameworkMoudles : IFrameworkMoudles, IDisposable
        {
            public FsmMoudle Fsm { get; set; }
            public TimerMoudle Timer { get; set; }
            public LoomMoudle Loom { get; set; }
            public CoroutineMoudle Coroutine { get; set; }
            public MessageMoudle Message { get; set; }
            public event Action<Type, string> onMoudleNotExist;

            public FrameworkMoudle CreateMoudle(Type type, string chunck = "Framework", bool bind = true)
            {
                return FrameworkMoudle.CreatInstance(type, chunck, bind);
            }
            public T CreateMoudle<T>(string chunck = "Framework", bool bind = true) where T : FrameworkMoudle
            {
                return FrameworkMoudle.CreatInstance<T>(chunck, bind);
            }


            public FrameworkMoudle this[Type type, string name]
            {
                get { return FindMoudle(type, name); }
            }
            public FrameworkMoudle FindMoudle(Type type, string name)
            {
                FrameworkMoudle mou = default(FrameworkMoudle);
                if (moudles.ContainsKey(type))
                    mou = moudles[type].Find((m) => { return m.name == name; });
                if (mou == null)
                    if (onMoudleNotExist != null)
                    {
                        onMoudleNotExist(type, name);
                        if (moudles.ContainsKey(type))
                            mou = moudles[type].Find((m) => { return m.name == name; });
                    }
                return mou;
            }
            public T FindMoudle<T>(string name) where T : FrameworkMoudle
            {
                return FindMoudle(typeof(T), name) as T;
            }




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
            public void UnBind(FrameworkMoudle moudle,bool dispose=true)
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
                        if (dispose)
                            moudle.Dispose();
                    }
                }
            }
        }
    }
}
