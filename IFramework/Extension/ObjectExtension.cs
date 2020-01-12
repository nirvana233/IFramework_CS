using System;
using System.Linq;

namespace IFramework
{
    public static partial class ObjectExtension
    {
        public static bool ContainsInterface(this object obj, Type type)
        {
            var objType = obj.GetType();
            return !objType.IsInterface && objType.GetInterfaces().Contains(type);
        }
        public static bool ContainsInterface(this object obj, string name)
        {
            var type = obj.GetType();
            return !type.IsInterface && type.GetInterface(name) != null;
        }
    }

}
