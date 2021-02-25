using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IFramework.Injection
{
     class FrameworkContainer : IFrameworkContainer, IDisposable
     {
        private const string emtyty = "______empty";
        private class Map<T1, T2, T3>
        {
            protected class InnerMap
            {
                public Dictionary<T2, T3> map = new Dictionary<T2, T3>();

                internal T3 GetValue(T2 key2)
                {
                    T3 inner;
                    if (map.TryGetValue(key2, out inner))
                    {
                        return inner;
                    }
                    return default(T3);
                }

                internal void SetVakue(T2 key2, T3 value)
                {
                    if (map.ContainsKey(key2))
                    {
                        map[key2] = value;
                    }
                    else
                    {
                        map.Add(key2, value);
                    }
                }

            }
            protected Dictionary<T1, InnerMap> map = new Dictionary<T1, InnerMap>();
            protected T3 Get(T1 key1, T2 key2)
            {
                InnerMap inner;
                if (map.TryGetValue(key1, out inner))
                {
                    return inner.GetValue(key2);
                }
                return default(T3);
            }
            protected void Set(T1 key1, T2 key2, T3 value)
            {
                InnerMap inner;
                if (!map.TryGetValue(key1, out inner))
                {
                    inner = new InnerMap();
                    map.Add(key1, inner);
                }
                inner.SetVakue(key2, value);
            }
            protected InnerMap Getmap(T1 key)
            {
                InnerMap inner;
                if (map.TryGetValue(key, out inner))
                {
                    return inner;
                }
                return null;
            }
            internal void Clear()
            {
                map.Clear();
            }
        }
        private class TypeMapping : Map<Type, string, Type>
        {
            public Type this[Type from, string name = null]
            {

                get
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        name = emtyty;
                    }
                    return base.Get(from, name);
                }
                set
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        name = emtyty;
                    }
                    base.Set(from, name, value);
                }
            }
            public IEnumerable<Type> GetValues(Type type)
            {
                foreach (var _type in map.Keys)
                {
                    if (type.IsAssignableFrom(_type))
                    {
                        foreach (var it in map[type].map.Keys)
                        {
                            if (string.IsNullOrEmpty(it))
                            {
                                yield return map[type].map[it];
                            }
                        }
                    }

                }

            }
        }
        private class TypeInstance : Map<Type, string, object>
        {
            public object this[Type from, string name = null]
            {
                get
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        name = emtyty;
                    }
                    return base.Get(from, name);
                }
                set
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        name = emtyty;
                    }
                    base.Set(from, name, value);
                }
            }
            public IEnumerable<object> GetValues(Type type)
            {
                InnerMap map = Getmap(type);
                if (map != null)
                {
                    foreach (var item in map.map)
                    {
                        if (!string.IsNullOrEmpty(item.Key))
                        {
                            yield return item.Value;
                        }
                    }
                }
            }
            public IEnumerable<object> Values
            {
                get
                {
                    foreach (var _map in map.Values)
                    {
                        foreach (var item in _map.map.Values)
                        {
                            yield return item;
                        }
                    }
                }
            }
        }


        private TypeMapping TypeMap = new TypeMapping();
        private TypeInstance Instances = new TypeInstance();

        public void Dispose()
        {
            this.Clear();
        }

        public void Clear()
        {
            this.Instances.Clear();
            this.TypeMap.Clear();
        }

        public void Inject(object obj)
        {
            if (obj == null)
            {
                return;
            }
            MemberInfo[] members = obj.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = members.Length - 1; i >= 0; i--)
            {
                MemberInfo member = members[i];
                if (member.IsDefined(typeof(InjectAttribute), true))
                {
                    InjectAttribute attr = member.GetCustomAttributes(typeof(InjectAttribute), true)[0] as InjectAttribute;
                    if (member is PropertyInfo)
                    {
                        PropertyInfo propertyInfo = member as PropertyInfo;
                        propertyInfo.SetValue(obj, this.GetValue(propertyInfo.PropertyType, attr.name, new object[0]), null);
                    }
                    else if (member is FieldInfo)
                    {
                        FieldInfo fieldInfo = member as FieldInfo;
                        fieldInfo.SetValue(obj, this.GetValue(fieldInfo.FieldType, attr.name, new object[0]));
                    }
                }
            }
        }

        public void InjectInstances()
        {
            foreach (object instance in this.Instances.Values)
            {
                this.Inject(instance);
            }
        }

        public void Subscribe<Type>(string name = null)
        {
            this.Subscribe(typeof(Type), typeof(Type), name);
        }

        public void Subscribe<BaseType, Type>(string name = null) where Type : BaseType
        {
            this.Subscribe(typeof(BaseType), typeof(Type), name);
        }

        public void Subscribe(Type source, Type target, string name = null)
        {
            this.TypeMap[source, name] = target;
        }

        public void SubscribeInstance<Type>(Type instance) where Type : class
        {
            this.SubscribeInstance<Type>(instance, true);
        }

        public void SubscribeInstance<Type>(Type instance, bool inject) where Type : class
        {
            this.SubscribeInstance<Type>(instance, null, inject);
        }

        public void SubscribeInstance<Type>(Type instance, string name = "empty", bool inject = true) where Type : class
        {
            this.SubscribeInstance(typeof(Type), instance, name, inject);
        }

        public void SubscribeInstance(Type baseType, object instance, bool inject = true)
        {
            this.SubscribeInstance(baseType, instance, null, inject);
        }

        public virtual void SubscribeInstance(Type baseType, object instance, string name = "empty", bool inject = true)
        {
            Type type = instance.GetType();
            if (type != baseType && !type.IsSubClassOfInterface(baseType) && !type.IsSubclassOf(baseType))
            {
                throw new Exception(string.Format("{0} is Not {1}", type, baseType));
            }
            this.Instances[baseType, name] = instance;
            if (inject)
            {
                this.Inject(instance);
            }
        }

        public T GetValue<T>(string name = null, params object[] args) where T : class
        {
            return (T)((object)this.GetValue(typeof(T), name, args));
        }

        public object GetValue(Type baseType, string name = null, params object[] constructorArgs)
        {
            object item = this.Instances[baseType, name];
            if (item != null)
            {
                return item;
            }

            Type map = this.TypeMap[baseType, name];
            if (map != null)
            {
                return this.CreateInstance(map, constructorArgs);
            }
            return null;
        }

        public IEnumerable<object> GetValues(Type type)
        {
            var ie = Instances.GetValues(type); ;
            foreach (var item in ie)
            {
                yield return item;
            }

            var ies = TypeMap.GetValues(type);
            foreach (var item in ies)
            {
                object obj = Activator.CreateInstance(item);
                this.Inject(obj);
                yield return obj;
            }
        }

        public IEnumerable<Type> GetValues<Type>()
        {
            foreach (object obj in this.GetValues(typeof(Type)))
            {
                yield return (Type)((object)obj);
            }
        }

        public object CreateInstance(Type type, params object[] ctrArgs)
        {
            if (ctrArgs != null && ctrArgs.Length != 0)
            {
                object obj2 = Activator.CreateInstance(type, ctrArgs);
                this.Inject(obj2);
                return obj2;
            }
            ConstructorInfo[] ctrs = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (ctrs.Length < 1)
            {
                object obj3 = Activator.CreateInstance(type);
                this.Inject(obj3);
                return obj3;
            }
            ParameterInfo[] maxParameters = ctrs[0].GetParameters();
            for (int i = ctrs.Length - 1; i >= 0; i--)
            {
                ParameterInfo[] parameters = ctrs[i].GetParameters();
                if (parameters.Length > maxParameters.Length)
                {
                    maxParameters = parameters;
                }
            }
            object[] args = maxParameters.Select(delegate (ParameterInfo p)
            {
                if (p.ParameterType.IsArray)
                {
                    return this.GetValues(p.ParameterType);
                }
                object tmp = this.GetValue(p.ParameterType, null, new object[0]);
                if (tmp != null)
                {
                    return tmp;
                }
                return this.GetValue(p.ParameterType, p.Name, new object[0]);
            }).ToArray<object>();
            object obj4 = Activator.CreateInstance(type, args);
            this.Inject(obj4);
            return obj4;
        }


      
    }
}
