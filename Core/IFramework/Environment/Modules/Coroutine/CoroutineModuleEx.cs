using System;
using System.Collections;

namespace IFramework.Modules.Coroutine
{
    /// <summary>
    /// 协程模块扩展
    /// </summary>
    [ScriptVersionAttribute(5)]
    [VersionUpdateAttribute(5,"面向接口")]
    public static class CoroutineModuleEx
    {
        /// <summary>
        /// 开启一个携程
        /// </summary>
        /// <param name="obj"></param>
        /// <param name=" envType"></param>
        /// <param name="routine">迭代器</param>
        /// <returns></returns>
        public static ICoroutine StartCoroutine(this object obj, IEnumerator routine, EnvironmentType envType)
        {
            var _env = Framework.GetEnv( envType);
            return _env.modules.Coroutine.StartCoroutine(routine);
        }
        /// <summary>
        /// 开启一个携程
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="env"></param>
        /// <param name="routine">迭代器</param>
        /// <returns></returns>
        public static ICoroutine StartCoroutine(this object obj, IEnumerator routine, IEnvironment env)
        {
            return env.modules.Coroutine.StartCoroutine(routine);
        }
        /// <summary>
        /// 结束回调
        /// </summary>
        /// <param name="self"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ICoroutine OnCompelete(this ICoroutine self, Action action)
        {
            (self as Coroutine).onCompelete += action;
            return self;
        }
    }
}
