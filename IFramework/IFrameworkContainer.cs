using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IFramework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; set; }

        public InjectAttribute()
        {
        }
    }
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


    class FrameworkContainer : IFrameworkContainer
    {
        /// http://stackoverflow.com/questions/1171812/multi-key-dictionary-in-c
        public class Tuple<T1, T2>
        {
            public readonly T1 Item1;
            public readonly T2 Item2;

            public Tuple(T1 item1, T2 item2)
            {
                Item1 = item1;
                Item2 = item2;
            }

            public override bool Equals(Object obj)
            {
                Tuple<T1, T2> p = obj as Tuple<T1, T2>;
                if (obj == null) return false;

                if (Item1 == null)
                {
                    if (p.Item1 != null) return false;
                }
                else
                {
                    if (p.Item1 == null || !Item1.Equals(p.Item1)) return false;
                }

                if (Item2 == null)
                {
                    if (p.Item2 != null) return false;
                }
                else
                {
                    if (p.Item2 == null || !Item2.Equals(p.Item2)) return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                int hash = 0;
                if (Item1 != null)
                    hash ^= Item1.GetHashCode();
                if (Item2 != null)
                    hash ^= Item2.GetHashCode();
                return hash;
            }
        }
        public class TypeMappingCollection : Dictionary<Tuple<Type, string>, Type>
        {
            public Type this[Type from, string name = null]
            {
                get
                {
                    Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                    Type mapping = null;
                    if (this.TryGetValue(key, out mapping))
                        return mapping;
                    return null;
                }
                set
                {
                    Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                    this[key] = value;
                }
            }
        }
        public class TypeInstanceCollection : Dictionary<Tuple<Type, string>, object>
        {
            public object this[Type from, string name = null]
            {
                get
                {
                    Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                    object mapping = null;
                    if (this.TryGetValue(key, out mapping))
                        return mapping;
                    return null;
                }
                set
                {
                    Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                    this[key] = value;
                }
            }
        }

        private TypeInstanceCollection _instances;
        private TypeMappingCollection typeMap;
        public TypeMappingCollection TypeMap
        {
            get
            {
                if (typeMap == null)
                    typeMap = new TypeMappingCollection();
                return typeMap;
            }
            set { typeMap = value; }
        }
        public TypeInstanceCollection Instances
        {
            get
            {
                if (_instances == null)
                    _instances = new TypeInstanceCollection();
                return _instances;
            }
            set { _instances = value; }
        }

        private List<OnContainerNotExistType> dels = new List<OnContainerNotExistType>();
        public event OnContainerNotExistType onNotExistType
        {
            add
            {
                if (!dels.Contains(value))
                    dels.Add(value);
            }
            remove
            {
                if (dels.Contains(value))
                    dels.Remove(value);
            }
        }

        public void Dispose()
        {
            Clear();
            dels.Clear();
        }
        public void Clear()
        {
            Instances.Clear();
            TypeMap.Clear();
        }

        public void Inject(object obj)
        {
            if (obj == null) return;
#if !NETFX_CORE
            var members = obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
#else
            var members = obj.GetType().GetTypeInfo().DeclaredMembers;
#endif
            for (int i = members.Length - 1; i >= 0; i--)
            {
                var member = members[i];
                if (!member.IsDefined(typeof(InjectAttribute), true)) continue;
                var attr = member.GetCustomAttributes(typeof(InjectAttribute), true)[0] as InjectAttribute;
                if (member is PropertyInfo)
                {
                    var propertyInfo = member as PropertyInfo;
                    propertyInfo.SetValue(obj, Resolve(propertyInfo.PropertyType, attr.Name), null);
                }
                else if (member is FieldInfo)
                {
                    var fieldInfo = member as FieldInfo;
                    fieldInfo.SetValue(obj, Resolve(fieldInfo.FieldType, attr.Name));
                }
            }
        }
        public void InjectInstances()
        {
            foreach (object instance in Instances.Values)
            {
                Inject(instance);
            }
        }

        public void Register<Type>(string name = null)
        {
            Register(typeof(Type), typeof(Type), name);
        }
        public void Register<BaseType, Type>(string name = null) where Type : BaseType
        {
            Register(typeof(BaseType), typeof(Type), name);
        }
        public void Register(Type source, Type target, string name = null)
        {
            TypeMap[source, name] = target;
        }

        public void RegisterInstance<Type>(Type instance) where Type : class
        {
            RegisterInstance<Type>(instance, true);
        }
        public void RegisterInstance<Type>(Type instance, bool inject) where Type : class
        {
            RegisterInstance<Type>(instance, null, inject);
        }
        public void RegisterInstance<Type>(Type instance, string name, bool inject = true) where Type : class
        {
            RegisterInstance(typeof(Type), instance, name, inject);
        }
        public void RegisterInstance(Type baseType, object instance, bool inject = true)
        {
            RegisterInstance(baseType, instance, null, inject);
        }
        public virtual void RegisterInstance(Type baseType, object instance, string name = null, bool inject = true)
        {
            Type type = instance.GetType();
            if (type != baseType && !type.IsSubClassOfInterface(baseType) && !type.IsSubclassOf(baseType))
                throw new Exception(string.Format("{0} is Not {1}", type, baseType));
            Instances[baseType, name] = instance;
            if (inject)
            {
                Inject(instance);
            }
        }


        public T Resolve<T>(string name = null, params object[] args) where T : class
        {
            return (T)Resolve(typeof(T), name, args);
        }
        public object Resolve(Type baseType, string name = null, params object[] constructorArgs)
        {
            var item = Instances[baseType, name];
            if (item != null)
                return item;
            var map = TypeMap[baseType, name];
            if (map != null)
                return CreateInstance(map, constructorArgs);
            else
            {
                for (int i = dels.Count - 1; i >= 0; i--)
                {
                    var del = dels[i];
                    var instance = del.Invoke(baseType, name);
                    if (instance == null) continue;
                    Type objType = instance.GetType();
                    if (objType != baseType && !objType.IsSubclassOf(baseType) && !objType.IsSubClassOfInterface(baseType)) continue;
                    RegisterInstance(baseType, instance, name, true);
                    return instance;
                }
            }
            return null;
        }

        //type=>type 的子类
        public IEnumerable<object> ResolveAll(Type type)
        {
            foreach (KeyValuePair<Tuple<Type, string>, object> instance in Instances)
            {
                if (instance.Key.Item1 == type && !string.IsNullOrEmpty(instance.Key.Item2))
                    yield return instance.Value;
            }

            foreach (KeyValuePair<Tuple<Type, string>, Type> map in TypeMap)
            {
                if (!string.IsNullOrEmpty(map.Key.Item2))
                {
#if NETFX_CORE
                    var condition = type.GetTypeInfo().IsSubclassOf(map.From);
#else
                    var condition = type.IsAssignableFrom(map.Key.Item1);
#endif
                    if (condition)
                    {
                        var item = Activator.CreateInstance(map.Value);
                        Inject(item);
                        yield return item;
                    }
                }
            }
        }
        public IEnumerable<Type> ResolveAll<Type>()
        {
            foreach (var obj in ResolveAll(typeof(Type)))
            {
                yield return (Type)obj;
            }
        }

        public object CreateInstance(Type type, params object[] ctrArgs)
        {
            if (ctrArgs != null && ctrArgs.Length > 0)
            {
                var obj2 = Activator.CreateInstance(type, ctrArgs);
                Inject(obj2);
                return obj2;
            }
#if !NETFX_CORE
            ConstructorInfo[] ctrs = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
#else
            ConstructorInfo[] constructor = type.GetTypeInfo().DeclaredConstructors.ToArray();
#endif
            if (ctrs.Length < 1)
            {
                var obj2 = Activator.CreateInstance(type);
                Inject(obj2);
                return obj2;
            }
            var maxParameters = ctrs[0].GetParameters();
            for (int i = ctrs.Length - 1; i >= 0; i--)
            {
                var parameters = ctrs[i].GetParameters();
                if (parameters.Length > maxParameters.Length)
                {
                    maxParameters = parameters;
                }
            }


            var args = maxParameters.Select(p =>
            {
                if (p.ParameterType.IsArray)
                    return ResolveAll(p.ParameterType);
                else
                {
                    var tmp = Resolve(p.ParameterType);
                    if (tmp != null)
                        return tmp;
                    return Resolve(p.ParameterType, p.Name);
                }
            }).ToArray();

            var obj = Activator.CreateInstance(type, args);
            Inject(obj);
            return obj;
        }


    }
}
