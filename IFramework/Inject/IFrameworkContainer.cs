using System;
using System.Collections.Generic;

namespace IFramework.Inject
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public delegate object OnContainerNotExistType(Type baseType, string name);


    public interface IFrameworkContainer : IDisposable
    {
        event OnContainerNotExistType onNotExistType;
        void Clear();
        void Inject(object obj);
        void InjectInstances();

        void Register<Type>(string name = null);
        void Register<BaseType, Type>(string name = null) where Type : BaseType;
        void Register(Type source, Type target, string name = null);
        void RegisterInstance<Type>(Type instance) where Type : class;
        void RegisterInstance<Type>(Type instance, bool inject) where Type : class;
        void RegisterInstance<Type>(Type instance, string name, bool inject = true) where Type : class;
        void RegisterInstance(Type baseType, object instance, bool inject = true);
        void RegisterInstance(Type baseType, object instance, string name = null, bool inject = true);

        T Resolve<T>(string name = null, params object[] args) where T : class;
        object Resolve(Type baseType, string name = null, params object[] constructorArgs);
        IEnumerable<object> ResolveAll(Type type);
        IEnumerable<Type> ResolveAll<Type>();

        object CreateInstance(Type type, params object[] ctrArgs);
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
