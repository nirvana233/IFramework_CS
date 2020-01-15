using System;

namespace IFramework.Moudles
{
    internal interface IFrameworkMoudleContaner:IDisposable
    {
        string chunck { get; }
        bool binded { get; }
        event Action<Type, string> onMoudleNotExist;

        void Update();
        FrameworkMoudle this[Type type, string name] { get; }
        FrameworkMoudle CreateMoudle(Type type);
        T CreateMoudle<T>() where T : FrameworkMoudle;

        FrameworkMoudle FindMoudle(Type type, string name);
        T FindMoudle<T>(string name) where T : FrameworkMoudle;
    }

}
