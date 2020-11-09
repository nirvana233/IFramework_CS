using System;
using System.Linq;
using System.Reflection;

[assembly: AssemblyVersion("0.0.0.2")]
namespace IFramework { }
namespace IFramework
{
    /// <summary>
    /// 框架入口
    /// </summary>
    [Dependence(typeof(FrameworkEnvironment))]
    [Dependence(typeof(Assemblies))]
    [Version(8)]
    [Update(8,"增加环境数量")]
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
                                 if (type.IsDefined(typeof(VersionAttribute), false))
                                 {
                                     VersionAttribute attr = type.GetCustomAttributes(typeof(VersionAttribute), false).First() as VersionAttribute;
                                     sum += attr.version;
                                 }
                                 else
                                     sum += 1;
                             });
            int mul = 1000;
            do
            {
                float tval = sum % mul;
                Version = Version.AppendHead(string.Format(".{0}", tval));
                sum = sum / mul;
            } while (sum > 0);
            Version = Version.Substring(1);
            int tmp = 4 - Version.Split('.').Length;
            for (int i = 0; i < tmp; i++)
                Version = Version.AppendHead("0.");
            return Version;
        }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public const string FrameworkName = "IFramework";
        public const string Author = "OnClick";
        public static string Version;
        public const string Description = FrameworkName;

        public static Assemblies Assembly = new Assemblies();

        public static FrameworkEnvironment env0;
        public static FrameworkEnvironment env1;
        public static FrameworkEnvironment env2;
        public static FrameworkEnvironment env3;
        public static FrameworkEnvironment env4;
        public static FrameworkEnvironment env5;
        public static FrameworkEnvironment env6;
        public static FrameworkEnvironment env7;
        public static FrameworkEnvironment env8;
        public static FrameworkEnvironment env9;


        public static FrameworkEnvironment extra0;
        public static FrameworkEnvironment extra1;
        public static FrameworkEnvironment extra2;
        public static FrameworkEnvironment extra3;
        public static FrameworkEnvironment extra4;

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        private static FrameworkEnvironment CreateInstance(string envName, EnvironmentType envType)
        {
            return FrameworkEnvironment.CreateInstance(envName, envType);
        }
        /// <summary>
        /// 实例化环境
        /// </summary>
        /// <param name="envName">环境名</param>
        /// <param name=" envType">环境类型</param>
        /// <returns>环境</returns>
        public static FrameworkEnvironment CreateEnv(string envName, EnvironmentType envType)
        {
            if (GetEnv(envType)!=null)
            {
                Log.E(string.Format("The EnvironmentType {0} is not null ", envType));
                return GetEnv(envType);
            }

            switch (envType)
            {
                case EnvironmentType.Ev0: env0 = CreateInstance(envName, envType); return env0;
                case EnvironmentType.Ev1: env1 = CreateInstance(envName, envType); return env1;
                case EnvironmentType.Ev2: env2 = CreateInstance(envName, envType); return env2;
                case EnvironmentType.Ev3: env3 = CreateInstance(envName, envType); return env3;
                case EnvironmentType.Ev4: env4 = CreateInstance(envName, envType); return env4;
                case EnvironmentType.Ev5: env5 = CreateInstance(envName, envType); return env5;
                case EnvironmentType.Ev6: env6 = CreateInstance(envName, envType); return env6;
                case EnvironmentType.Ev7: env7 = CreateInstance(envName, envType); return env7;
                case EnvironmentType.Ev8: env8 = CreateInstance(envName, envType); return env8;
                case EnvironmentType.Ev9: env9 = CreateInstance(envName, envType); return env9;
                case EnvironmentType.Extra0: extra0 = CreateInstance(envName, envType); return extra0;
                case EnvironmentType.Extra1: extra1 = CreateInstance(envName, envType); return extra1;
                case EnvironmentType.Extra2: extra2 = CreateInstance(envName, envType); return extra2;
                case EnvironmentType.Extra3: extra3 = CreateInstance(envName, envType); return extra3;
                case EnvironmentType.Extra4: extra4 = CreateInstance(envName, envType); return extra4;
                default:
                    throw new Exception(string.Format("The EnvironmentType {0} Error,Please Check ", envType));
            }
        }
        /// <summary>
        /// 根据序号获取环境
        /// </summary>
        /// <param name=" envType">环境类型</param>
        /// <returns></returns>
        public static FrameworkEnvironment GetEnv(EnvironmentType envType)
        {
            switch (envType)
            {
                case EnvironmentType.Ev0: return env0;
                case EnvironmentType.Ev1: return env1;
                case EnvironmentType.Ev2: return env3;
                case EnvironmentType.Ev3: return env3;
                case EnvironmentType.Ev4: return env4;
                case EnvironmentType.Ev5: return env5;
                case EnvironmentType.Ev6: return env6;
                case EnvironmentType.Ev7: return env7;
                case EnvironmentType.Ev8: return env8;
                case EnvironmentType.Ev9: return env9;
                case EnvironmentType.Extra0: return extra0;
                case EnvironmentType.Extra1: return extra1;
                case EnvironmentType.Extra2: return extra2;
                case EnvironmentType.Extra3: return extra3;
                case EnvironmentType.Extra4: return extra4;
                default:
                    throw new Exception(string.Format("The EnvironmentType {0} Error,Please Check ", envType));
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
            action.BindEnvUpdate(GetEnv(envType));
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name=" envType"></param>
        public static void UnBindEnvUpdate(this Action action, EnvironmentType envType)
        {
            action.UnBindEnvUpdate(GetEnv(envType));
        }
        /// <summary>
        /// 绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name=" envType"></param>
        public static void BindEnvDispose(this Action action, EnvironmentType envType)
        {
            action.BindEnvDispose(GetEnv(envType));
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name=" envType"></param>
        public static void UnBindEnvDispose(this Action action, EnvironmentType envType)
        {
            action.UnBindEnvDispose(GetEnv(envType));
        }
    }
}
