using System;
using System.Linq;
using System.Linq.Expressions;

namespace IFramework
{
    /// <summary>
    /// object 静态扩展
    /// </summary>
    public static partial class ObjectExtension
    {
        /// <summary>
        /// 是否继承接口
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ContainsInterface(this object obj, Type type)
        {
            var objType = obj.GetType();
            return !objType.IsInterface && objType.GetInterfaces().Contains(type);
        }
        /// <summary>
        /// 是否继承接口
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ContainsInterface(this object obj, string name)
        {
            var type = obj.GetType();
            return !type.IsInterface && type.GetInterface(name) != null;
        }
        ///// <summary>
        ///// 获取属性名称
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="obj"></param>
        ///// <param name="property">属性表达式</param>
        ///// <returns></returns>
        //public static string GetPropertyName<T>(this object obj, Expression<Func<T>> property)
        //{
        //    MemberExpression memberExpression = property.Body as MemberExpression;
        //    if (memberExpression == null) return null;
        //    return memberExpression.Member.Name;
        //}
        /// <summary>
        /// 强转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T As<T>(this object obj)
        {
            if (obj==null)
            {
                return default(T);
            }
            return (T)obj;
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
