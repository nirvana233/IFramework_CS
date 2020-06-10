using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class Assemblies
    {
        public Assemblies()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < ass.Length; i++)
            {
                SubscribeAssembly(ass[i]);
            }
        }
        private Dictionary<string, Assembly> _amap = new Dictionary<string, Assembly>();
        private Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();
        private Dictionary<Type, string> _nameMap = new Dictionary<Type, string>();
        private object _lock = new object();



        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            SubscribeAssembly(args.LoadedAssembly);
        }
        private void SubscribeAssembly(Assembly assembly)
        {
            var name = assembly.GetName().Name;
            lock (_lock)
            {
                if (!_amap.ContainsKey(name))
                {
                    _amap.Add(name, assembly);
                }
            }
        }

        public bool ContainsType(string typeName)
        {
            lock (_lock)
            {
                return _typeMap.ContainsKey(typeName);
            }
        }
        public string GetTypeName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            string result;

            lock (_lock)
            {
                if (_nameMap.TryGetValue(type, out result) == false)
                {
                    if (type.IsGenericType)
                    {
                        // We track down all assemblies in the generic type definition
                        List<Type> toResolve = type.GetGenericArguments().ToList();
                        HashSet<Assembly> assemblies = new HashSet<Assembly>();

                        while (toResolve.Count > 0)
                        {
                            var t = toResolve[0];

                            if (t.IsGenericType)
                            {
                                toResolve.AddRange(t.GetGenericArguments());
                            }

                            assemblies.Add(t.Assembly);
                            toResolve.RemoveAt(0);
                        }

                        result = type.FullName + ", " + type.Assembly.GetName().Name;

                        foreach (var ass in assemblies)
                        {
                            result = result.Replace(ass.FullName, ass.GetName().Name);
                        }
                    }
                    else if (type.IsDefined(typeof(CompilerGeneratedAttribute), false))
                    {
                        result = type.FullName + ", " + type.Assembly.GetName().Name;
                    }
                    else
                    {
                        result = type.FullName + ", " + type.Assembly.GetName().Name;
                    }

                    _nameMap.Add(type, result);
                }
            }

            return result;
        }
        public Type GetType(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");


            Type result;

            lock (_lock)
            {
                if (_typeMap.TryGetValue(typeName, out result) == false)
                {
                    result = this.ParseTypeName(typeName);

                    if (result == null)
                    {
                        Log.W("Failed deserialization type lookup for type name '" + typeName + "'.");
                    }

                    // We allow null values on purpose so we don't have to keep re-performing invalid name lookups
                    _typeMap.Add(typeName, result);
                }
            }

            return result;
        }


        private Type ParseTypeName(string typeName)
        {
            Type type;
            type = Type.GetType(typeName);
            if (type != null) return type;

            string typeStr, assemblyStr;

            ParseName(typeName, out typeStr, out assemblyStr);

            if (!string.IsNullOrEmpty(typeStr))
            {

                Assembly assembly;
                if (assemblyStr != null)
                {
                    lock (_lock)
                    {
                        _amap.TryGetValue(assemblyStr, out assembly);
                    }

                    if (assembly == null)
                    {
                        try
                        {
                            assembly = Assembly.Load(assemblyStr);
                        }
                        catch { }
                    }

                    if (assembly != null)
                    {
                        try
                        {
                            type = assembly.GetType(typeStr);
                        }
                        catch { } // Assembly is invalid

                        if (type != null) return type;
                    }
                }

                // Try to check all assemblies for the type string
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                for (int i = 0; i < assemblies.Length; i++)
                {
                    assembly = assemblies[i];

                    try
                    {
                        type = assembly.GetType(typeStr, false);
                    }
                    catch { } // Assembly is invalid

                    if (type != null) return type;
                }
            }
            return null;
        }
        private static void ParseName(string fullName, out string typeName, out string assemblyName)
        {
            typeName = null;
            assemblyName = null;

            int firstComma = fullName.IndexOf(',');

            if (firstComma < 0 || (firstComma + 1) == fullName.Length)
            {
                typeName = fullName.Trim(',', ' ');
                return;
            }
            else
            {
                typeName = fullName.Substring(0, firstComma);
            }

            int secondComma = fullName.IndexOf(',', firstComma + 1);

            if (secondComma < 0)
            {
                assemblyName = fullName.Substring(firstComma).Trim(',', ' ');
            }
            else
            {
                assemblyName = fullName.Substring(firstComma, secondComma - firstComma).Trim(',', ' ');
            }
        }

    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
