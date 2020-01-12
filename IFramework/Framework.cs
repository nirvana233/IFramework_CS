using IFramework.Moudles;
using IFramework.Moudles.Coroutine;
using IFramework.Moudles.Message;
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
        private static List<FrameworkMoudle> moudles;

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
            stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            _disposed = false;
            haveInit = true;

            moudles = new List<FrameworkMoudle>();
            if (onInit != null) onInit();
        }
        public static void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            stopwatch2.Stop();
            stopwatch2 = null;
            if (onDispose != null) onDispose();

            for (int i = 0; i < moudles.Count; i++)
            {
                moudles[i].Dispose();
                moudles[i] = null;
            }
            moudles.Clear();
            moudles = null;

            Container.Dispose();
            SingletonPool.Dispose();
        }
        public static void Update()
        {
            if (_disposed) return;
            stopwatch.Restart();
            if (update != null) update();
            stopwatch.Stop();
            deltaTime = stopwatch.Elapsed;
        }


        public static CoroutineMoudle CoroutineMoudle { get; set; }
        public static MessageMoudle MessageMoudle { get; set; }

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
        public static void BindFramework(this FrameworkMoudle moude)
        {
            if (!moudles.Contains(moude))
            {
                moudles.Add(moude);
                update += moude.Update;
                onDispose += moude.Dispose;
            }
            else
            {
                Log.W(string.Format("Have Bind Moudle : {0}", moude.name));
            }
        }
        public static void UnBindFramework(this FrameworkMoudle moude)
        {
            if (!moudles.Contains(moude))
            {
                Log.W(string.Format("Have Not Bind Moudle : {0}", moude.name));
            }
            else
            {
                moudles.Remove(moude);
                update -= moude.Update;
                onDispose -= moude.Dispose;
            }
        }
    }
}
