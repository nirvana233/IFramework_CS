using System;

namespace IFramework.Modules
{
    internal interface IFrameworkModuleContaner : IDisposable
    {
        string chunck { get; }
        bool binded { get; }
        event Action<Type, string> onModuleNotExist;

        void Update();
        FrameworkModule this[Type type, string name] { get; }
        FrameworkModule CreateModule(Type type, string name = "");
        T CreateModule<T>(string name = "") where T : FrameworkModule;

        FrameworkModule FindModule(Type type, string name);
        T FindModule<T>(string name) where T : FrameworkModule;
    }

}
