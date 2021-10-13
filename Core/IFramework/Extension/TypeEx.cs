using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IFramework
{
    /// <summary>
    /// Type静态扩展
    /// </summary>
    public static partial class TypeEx
    {
        /// <summary>
        /// 获取当前程序集中的类型的子类，3.5有问题
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetSubTypesInAssembly(this Type self)
        {
            if (self.IsInterface)
                return Assembly.GetExecutingAssembly()
                               .GetTypes()
                               .Where(item => item.GetInterfaces().Contains(self));
            return Assembly.GetExecutingAssembly()
                           .GetTypes()
                           .Where(item => item.IsSubclassOf(self));
        }
        /// <summary>
        /// 获取所有程序集中的类型的子类，3.5有问题
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetSubTypesInAssemblys(this Type self)
        {
            if (self.IsInterface)
                return AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(item => item.GetTypes())
                                .Where(item => item.GetInterfaces().Contains(self));
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(item => item.GetTypes())
                            .Where(item => item.IsSubclassOf(self));
        }
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static object CreatInstance(this Type self)
        {
            return self.IsValueType ? Activator.CreateInstance(self) : null;
        }
        /// <summary>
        /// 是否继承接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Interface"></param>
        /// <returns></returns>
        public static bool IsExtendInterface(this Type self, Type Interface)
        {
            return self.GetInterfaces().Contains(Interface);
        }
        
        /// <summary>
        /// 是否继承自泛型类
        /// </summary>
        /// <param name="self"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsSubclassOfGeneric(this Type self, Type genericType)
        {
#if NETFX_CORE
                if (!genericTypeDefinition.GetTypeInfo().IsGenericTypeDefinition)
#else
            if (!genericType.IsGenericTypeDefinition)
#endif
                return false;

#if NETFX_CORE
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(genericTypeDefinition))
#else
            if (self.IsGenericType && self.GetGenericTypeDefinition().Equals(genericType))
#endif
                return true;

#if NETFX_CORE
                Type baseType = type.GetTypeInfo().BaseType;
#else
            Type baseType = self.BaseType;
#endif
            if (baseType != null && baseType != typeof(object))
            {
                if (IsSubclassOfGeneric(baseType, genericType))
                    return true;
            }

            foreach (Type t in self.GetInterfaces())
            {
                if (IsSubclassOfGeneric(t, genericType))
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 获取类型树
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IList<Type> GetTypeTree(this Type t)
        {
            var tmp = t;
            var types = new List<Type>();
            do
            {
                types.Add(t);
                t = t.BaseType;
            } while (t!=null);
            types.AddRange(tmp.GetInterfaces());
            return types;
        }
        /// <summary>
        /// 获取程序集下的静态扩展
        /// </summary>
        /// <param name="self"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetExtensionMethods(this Type self,Assembly assembly)
        {
            var query = from type in assembly.GetTypes()
                        where !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == self
                        select method;
            return query;
        }

    }

}
