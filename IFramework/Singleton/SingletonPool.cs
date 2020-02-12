using System;
using System.Collections.Generic;

namespace IFramework
{
    //[OnFrameworkInitClass]
    public static class SingletonPool
    {
        static Dictionary<Type, ISingleton> pairs;
        static SingletonPool()
        {
            pairs = new Dictionary<Type, ISingleton>();
           // Framework.onDispose += Dispose;
        }
        public static void Set<T>(T singleton) where T : ISingleton
        {
            Type type = typeof(T);
            if (!pairs.ContainsKey(type))
                pairs.Add(type, singleton);
            else
                throw new Exception("Singleton Err");
        }
        public static void Dispose<T>() where T : ISingleton
        {
            Type type = typeof(T);
            if (pairs.ContainsKey(type))
            {
                pairs[type].Dispose();
                pairs.Remove(type);
            }
            else
                throw new Exception("SingletonPool dispose Err");
        }
        public static void Dispose()
        {
            foreach (var item in pairs.Values)
            {
                item.Dispose();
            }
            pairs.Clear();
        }
    }

}
