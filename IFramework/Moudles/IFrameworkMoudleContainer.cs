using System;

namespace IFramework.Moudles
{
    internal interface IFrameworkMoudleContaner
    {
        string chunck { get; }
        event Action<Type, string> onMoudleNotExist;

        FrameworkMoudle this[Type type, string name] { get; }
        FrameworkMoudle CreateMoudle(Type type);
        T CreateMoudle<T>() where T : FrameworkMoudle;

        FrameworkMoudle FindMoudle(Type type, string name);
        T FindMoudle<T>(string name) where T : FrameworkMoudle;
    }

}
