using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
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

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class Assemblies
    {
        public Assemblies()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < ass.Length; i++)
            {
                SubscribeAssembly(ass[i]);
            }
        }
        private Dictionary<string, Assembly> _amap = new Dictionary<string, Assembly>();
        private Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();
        private Dictionary<Type, string> _nameMap = new Dictionary<Type, string>();
        private object _lock = new object();



        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            SubscribeAssembly(args.LoadedAssembly);
        }
        private void SubscribeAssembly(Assembly assembly)
        {
            var name = assembly.GetName().Name;
            lock (_lock)
            {
                if (!_amap.ContainsKey(name))
                {
                    _amap.Add(name, assembly);
                }
            }
        }

        public bool ContainsType(string typeName)
        {
            lock (_lock)
            {
                return _typeMap.ContainsKey(typeName);
            }
        }
        public string GetTypeName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            string result;

            lock (_lock)
            {
                if (_nameMap.TryGetValue(type, out result) == false)
                {
                    if (type.IsGenericType)
                    {
                        // We track down all assemblies in the generic type definition
                        List<Type> toResolve = type.GetGenericArguments().ToList();
                        HashSet<Assembly> assemblies = new HashSet<Assembly>();

                        while (toResolve.Count > 0)
                        {
                            var t = toResolve[0];

                            if (t.IsGenericType)
                            {
                                toResolve.AddRange(t.GetGenericArguments());
                            }

                            assemblies.Add(t.Assembly);
                            toResolve.RemoveAt(0);
                        }

                        result = type.FullName + ", " + type.Assembly.GetName().Name;

                        foreach (var ass in assemblies)
                        {
                            result = result.Replace(ass.FullName, ass.GetName().Name);
                        }
                    }
                    else if (type.IsDefined(typeof(CompilerGeneratedAttribute), false))
                    {
                        result = type.FullName + ", " + type.Assembly.GetName().Name;
                    }
                    else
                    {
                        result = type.FullName + ", " + type.Assembly.GetName().Name;
                    }

                    _nameMap.Add(type, result);
                }
            }

            return result;
        }
        public Type GetType(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");


            Type result;

            lock (_lock)
            {
                if (_typeMap.TryGetValue(typeName, out result) == false)
                {
                    result = this.ParseTypeName(typeName);

                    if (result == null)
                    {
                        Log.W("Failed deserialization type lookup for type name '" + typeName + "'.");
                    }

                    // We allow null values on purpose so we don't have to keep re-performing invalid name lookups
                    _typeMap.Add(typeName, result);
                }
            }

            return result;
        }


        private Type ParseTypeName(string typeName)
        {
            Type type;
            type = Type.GetType(typeName);
            if (type != null) return type;

            string typeStr, assemblyStr;

            ParseName(typeName, out typeStr, out assemblyStr);

            if (!string.IsNullOrEmpty(typeStr))
            {

                Assembly assembly;
                if (assemblyStr != null)
                {
                    lock (_lock)
                    {
                        _amap.TryGetValue(assemblyStr, out assembly);
                    }

                    if (assembly == null)
                    {
                        try
                        {
                            assembly = Assembly.Load(assemblyStr);
                        }
                        catch { }
                    }

                    if (assembly != null)
                    {
                        try
                        {
                            type = assembly.GetType(typeStr);
                        }
                        catch { } // Assembly is invalid

                        if (type != null) return type;
                    }
                }

                // Try to check all assemblies for the type string
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                for (int i = 0; i < assemblies.Length; i++)
                {
                    assembly = assemblies[i];

                    try
                    {
                        type = assembly.GetType(typeStr, false);
                    }
                    catch { } // Assembly is invalid

                    if (type != null) return type;
                }
            }
            return null;
        }
        private static void ParseName(string fullName, out string typeName, out string assemblyName)
        {
            typeName = null;
            assemblyName = null;

            int firstComma = fullName.IndexOf(',');

            if (firstComma < 0 || (firstComma + 1) == fullName.Length)
            {
                typeName = fullName.Trim(',', ' ');
                return;
            }
            else
            {
                typeName = fullName.Substring(0, firstComma);
            }

            int secondComma = fullName.IndexOf(',', firstComma + 1);

            if (secondComma < 0)
            {
                assemblyName = fullName.Substring(firstComma).Trim(',', ' ');
            }
            else
            {
                assemblyName = fullName.Substring(firstComma, secondComma - firstComma).Trim(',', ' ');
            }
        }

    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

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

        public static Assemblies Assembly=new Assemblies();

        public static FrameworkEnvironment env0;
        public static FrameworkEnvironment env1;
        public static FrameworkEnvironment env2;
        public static FrameworkEnvironment env3;
        public static FrameworkEnvironment env4;

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释


        /// <summary>
        /// 创建一个环境
        /// </summary>
        /// <param name="envName">环境名称</param>
        /// <param name=" envType">环境类型</param>
        /// <returns>环境</returns>
        public static FrameworkEnvironment CreateEnv(string envName, EnvironmentType envType)
        {
            return FrameworkEnvironment.CreateInstance(envName, envType);
        }
        /// <summary>
        /// 实例化环境
        /// </summary>
        /// <param name="envName">环境名</param>
        /// <param name=" envType">环境类型</param>
        /// <returns>环境</returns>
        public static FrameworkEnvironment InitEnv(string envName, EnvironmentType envType)
        {
            switch ( envType)
            {
                case EnvironmentType.Ev0: env0 = CreateEnv(envName, envType); return env0;
                case EnvironmentType.Ev1: env1 = CreateEnv(envName, envType); return env1;
                case EnvironmentType.Ev2: env2 = CreateEnv(envName, envType); return env2;
                case EnvironmentType.Ev3: env3 = CreateEnv(envName, envType); return env3;
                case EnvironmentType.Ev4: env4 = CreateEnv(envName, envType); return env4;
                default:
                    throw new Exception(string.Format("The EnvironmentType {0} Error,Please Check ",  envType));
            }
        }
        /// <summary>
        /// 根据序号获取环境
        /// </summary>
        /// <param name=" envType">环境类型</param>
        /// <returns></returns>
        public static FrameworkEnvironment GetEnv(EnvironmentType envType)
        {
            switch ( envType)
            {
                case EnvironmentType.Ev0: return env0;
                case EnvironmentType.Ev1: return env1;
                case EnvironmentType.Ev2: return env3;
                case EnvironmentType.Ev3: return env3;
                case EnvironmentType.Ev4: return env4;
                default:
                    throw new Exception(string.Format("The EnvironmentType {0} Error,Please Check ",  envType));
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
        /// <param name=" envType"></param>
        public static void BindEnvUpdate(this Action action, EnvironmentType envType)
        {
            action.BindEnvUpdate(GetEnv( envType));
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name=" envType"></param>
        public static void UnBindEnvUpdate(this Action action, EnvironmentType envType)
        {
            action.UnBindEnvUpdate(GetEnv( envType));
        }
        /// <summary>
        /// 绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name=" envType"></param>
        public static void BindEnvDispose(this Action action, EnvironmentType envType)
        {
            action.BindEnvDispose(GetEnv( envType));
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name=" envType"></param>
        public static void UnBindEnvDispose(this Action action, EnvironmentType envType)
        {
            action.UnBindEnvDispose(GetEnv( envType));
        }
    }



#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class FrameworkObject : IDisposable
    {
        private string _name ;
        private bool _disposed;
        private Guid _guid=Guid.NewGuid();
        public Guid guid { get { return _guid; } }
        public bool disposed { get { return _disposed; } }
        public string name { get { return _name; }set { _name = value; } }

        protected virtual void OnDispose() { }
        public virtual void Dispose()
        {
            Dispose(null,null);
        }
        protected void Dispose(Action frontofonDispose, Action frontof_disposed)
        {
            if (_disposed) return;
            if (frontofonDispose != null)
                frontofonDispose();
            OnDispose();
            if (frontof_disposed != null)
                frontof_disposed();
            _name = string.Empty;
            _guid = Guid.Empty;
            _disposed = true;
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
