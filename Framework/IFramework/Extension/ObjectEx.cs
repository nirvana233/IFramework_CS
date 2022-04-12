using System;
using System.Linq;
using System.Linq.Expressions;

namespace IFramework
{
    /// <summary>
    /// object 静态扩展
    /// </summary>
    public static partial class ObjectEx
    {
        /// <summary>
        /// 强转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T As<T>(this object obj)where T : class
        {
            if (obj==null)
            {
                return default(T);
            }
            return obj as T;
        }
        /// <summary>
        /// 对象是否是该类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Is<T>(this object obj)
        {
            return obj is T;
        }
    }

}
