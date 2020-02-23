using System;
using System.Linq;

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
    }

}
