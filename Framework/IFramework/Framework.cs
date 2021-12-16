﻿using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
[assembly: AssemblyVersion("0.0.0.2")]
namespace IFramework { }
namespace IFramework
{
    /// <summary>
    /// 框架入口
    /// </summary>
    [RequireAttribute(typeof(FrameworkEnvironment))]
    [ScriptVersionAttribute(10)]
    [VersionUpdateAttribute(8, "增加环境数量")]
    [VersionUpdateAttribute(10, "改变环境为属性")]
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
                             .ToList()
                             .ForEach((type) =>
                             {
                                 if (!type.FullName.Contains(FrameworkName)) return;
                                 if (type.IsDefined(typeof(ScriptVersionAttribute), false))
                                 {
                                     ScriptVersionAttribute attr = type.GetCustomAttributes(typeof(ScriptVersionAttribute), false).First() as ScriptVersionAttribute;
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
        private static Dictionary<int, IEnvironment> envs = new Dictionary<int, IEnvironment>();
        private static LockParam _lock = new LockParam();

        public static IEnvironment current { get { return FrameworkEnvironment.current; } }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /// <summary>
        /// 实例化环境
        /// </summary>
        /// <param name=" envType">环境类型</param>
        /// <returns>环境</returns>
        public static IEnvironment CreateEnv(EnvironmentType envType)
        {
            using (new LockWait(ref _lock))
            {
                IEnvironment env;
                if (envs.TryGetValue((int)envType, out env))
                {
                    throw new Exception(string.Format("The EnvironmentType {0} is not null ", envType));
                }
                else
                {
                    env = new FrameworkEnvironment(envType);
                    envs.Add((int)envType, env);
                    return env;
                }
            }
        }
        /// <summary>
        /// 根据序号获取环境
        /// </summary>
        /// <param name=" envType">环境类型</param>
        /// <returns></returns>
        public static IEnvironment GetEnv(EnvironmentType envType)
        {
            using(new LockWait(ref _lock))
            {
                IEnvironment env;
                if (envs.TryGetValue((int)envType, out env))
                {
                    return env;
                }
                else
                {
                    throw new Exception(string.Format("The EnvironmentType {0} Error Not Find ,Please Check ", envType));
                }
            }
        }
        /// <summary>
        /// 释放环境
        /// </summary>
        /// <param name="envType"></param>
        public static void DisposeEnv(EnvironmentType envType)
        {
            var env = GetEnv(envType);
            if (env!=null)
            {
                env.Dispose();
                using (new LockWait(ref _lock))
                {
                    envs.Remove((int)envType);
                }
            }
           
        }


        /// <summary>
        /// 绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void BindEnvUpdate(this Action action, IEnvironment env)
        {
            env.BindUpdate(action);
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void UnBindEnvUpdate(this Action action, IEnvironment env)
        {
            env.UnBindUpdate(action);
        }
        /// <summary>
        /// 绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void BindEnvDispose(this Action action, IEnvironment env)
        {
            env.BindDispose(action);
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void UnBindEnvDispose(this Action action, IEnvironment env)
        {
            env.UnBindDispose(action);
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
