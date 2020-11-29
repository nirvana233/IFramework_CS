using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IFramework.Injection
{
    class FrameworkContainer : IFrameworkContainer
    {
        /// http://stackoverflow.com/questions/1171812/multi-key-dictionary-in-c
        private class Tuple<T1, T2>
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
        private class TypeMappingCollection : Dictionary<Tuple<Type, string>, Type>
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
        private class TypeInstanceCollection : Dictionary<Tuple<Type, string>, object>
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

        private TypeMappingCollection _typeMap;
        private TypeInstanceCollection _instance;
        public FrameworkContainer()
        {
            _typeMap = new TypeMappingCollection();
            _instance = new TypeInstanceCollection();
        }

        public void Subscribe<Type>(string name = null)
        {
            Subscribe(typeof(Type), typeof(Type), name);
        }
        public void Subscribe<BaseType, Type>(string name = null) where Type : BaseType
        {
            Subscribe(typeof(BaseType), typeof(Type), name);
        }
        public void Subscribe(Type source, Type target, string name = null)
        {
            _typeMap[source, name] = target;
        }
        public void SubscribeInstance<Type>(Type instance, string name="", bool inject = true) where Type : class
        {
            SubscribeInstance(typeof(Type), instance, name, inject);
        }
        public virtual void SubscribeInstance(Type baseType, object instance, string name = null, bool inject = true)
        {
            Type type = instance.GetType();
            if (type != baseType && !type.IsSubClassOfInterface(baseType) && !type.IsSubclassOf(baseType))
                throw new Exception(string.Format("{0} is Not {1}", type, baseType));
            _instance[baseType, name] = instance;
            if (inject)
            {
                Inject(instance);
            }
        }


        public T GetValue<T>(string name = null, params object[] args) where T : class
        {
            return (T)GetValue(typeof(T), name, args);
        }
        public object GetValue(Type baseType, string name = null, params object[] constructorArgs)
        {
            var item = _instance[baseType, name];
            if (item != null)
                return item;
            var type = _typeMap[baseType, name];
            if (type != null)
                return CreateInstance(type, constructorArgs);
          
            return null;
        }
        //type=>type 的子类
        public IEnumerable<object> GetValues(Type type)
        {
            foreach (KeyValuePair<Tuple<Type, string>, object> instance in _instance)
            {
                if (instance.Key.Item1 == type && !string.IsNullOrEmpty(instance.Key.Item2))
                    yield return instance.Value;
            }

            foreach (KeyValuePair<Tuple<Type, string>, Type> map in _typeMap)
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
        public IEnumerable<Type> GetValues<Type>()
        {
            foreach (var obj in GetValues(typeof(Type)))
            {
                yield return (Type)obj;
            }
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
                    propertyInfo.SetValue(obj, GetValue(propertyInfo.PropertyType, attr.name), null);
                }
                else if (member is FieldInfo)
                {
                    var fieldInfo = member as FieldInfo;
                    fieldInfo.SetValue(obj, GetValue(fieldInfo.FieldType, attr.name));
                }
            }
        }
        public void InjectInstances()
        {
            foreach (object instance in _instance.Values)
            {
                Inject(instance);
            }
        }

        public void Clear()
        {
            _instance.Clear();
            _typeMap.Clear();
        }
        public void Dispose()
        {
            Clear();
        }

        private object CreateInstance(Type type, params object[] ctrArgs)
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
                    return GetValues(p.ParameterType);
                else
                {
                    var tmp = GetValue(p.ParameterType);
                    if (tmp != null)
                        return tmp;
                    return GetValue(p.ParameterType, p.Name);
                }
            }).ToArray();

            var obj = Activator.CreateInstance(type, args);
            Inject(obj);
            return obj;
        }

    }
}
