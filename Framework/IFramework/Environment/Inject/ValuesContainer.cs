using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IFramework
{
    class ValuesContainer : IValuesContainer
    {
        private class Map<T>
        {
            public class Value : IValueContainer<T>
            {
                public string key;
                public T value { get; set; }
            }

            protected List<Value> containers;
            protected Dictionary<Type, List<int>> map;
            public Map()
            {
                containers = new List<Value>();
                map = new Dictionary<Type, List<int>>();
            }
            public void Set(Type super, string key, T t)
            {
                List<int> list;
                if (!map.TryGetValue(super, out list))
                {
                    list = new List<int>();
                    map.Add(super, list);
                }

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    int index = list[i];
                    var value = containers[index];
                    if (value.key == key)
                    {
                        value.value = t;
                        return;
                    }
                }
                list.Add(containers.Count);
                containers.Add(new Value() { key = key, value = t });
            }
            public T Get(Type super, string key)
            {
                List<int> list;
                if (map.TryGetValue(super, out list))
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        int index = list[i];
                        var value = containers[index];
                        if (value.key == key)
                        {
                            return value.value;
                        }
                    }
                }
                return default(T);
            }

            protected T GetByindex(int index)
            {
                return containers[index].value;
            }
            public void Clear()
            {
                map.Clear();
                containers.Clear();
            }

            public IEnumerable<T> Values
            {
                get
                {
                    for (int i = 0; i < containers.Count; i++)
                    {
                        yield return GetByindex(i);
                    }
                }
            }
        }
        private class Types : Map<Type>
        {
            public IEnumerable<Type> GetTypes(Type type)
            {
                foreach (var _type in map.Keys)
                {
                    if (type.IsAssignableFrom(_type))
                    {
                        foreach (var it in map[_type])
                        {
                            yield return GetByindex(it);
                        }
                    }

                }

            }
        }
        private class Instances : Map<object>
        {
            public IEnumerable<object> GetInstances(Type type)
            {
                List<int> list;
                if (map.TryGetValue(type, out list))
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        int index = list[i];
                        var value = containers[index];
                        yield return value.value;
                    }
                }
            }
        }
        private Types _type = new Types();
        private Instances _instance = new Instances();

        public void Dispose()
        {
            this.Clear();
        }

        public void Clear()
        {
            this._instance.Clear();
            this._type.Clear();
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
            foreach (object instance in this._instance.Values)
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
            this._type.Set(source, name, target);
        }

        public void SubscribeInstance<Type>(Type instance, string name, bool inject = true) where Type : class
        {
            this.SubscribeInstance(typeof(Type), instance, name, inject);
        }
        public void SubscribeInstance(Type baseType, object instance, string name, bool inject = true)
        {
            Type type = instance.GetType();
            if (type != baseType && !type.IsExtendInterface(baseType) && !type.IsSubclassOf(baseType))
            {
                throw new Exception(string.Format("{0} is Not {1}", type, baseType));
            }
            this._instance.Set(baseType, name, instance);
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
            object item = _instance.Get(baseType, name);
            if (item != null) return item;
            Type map = this._type.Get(baseType, name);
            if (map != null)
            {
                return this.CreateInstance(map, constructorArgs);
            }
            return null;
        }

        public IEnumerable<object> GetValues(Type type)
        {
            var ie = _instance.GetInstances(type); ;
            foreach (var item in ie)
            {
                yield return item;
            }

            var ies = _type.GetTypes(type);
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

        private object CreateInstance(Type type, params object[] ctrArgs)
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
