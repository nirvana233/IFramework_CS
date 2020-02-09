using System;

namespace IFramework.Modules
{
    internal interface IFrameworkModule : IDisposable
    {
        string moudeType { get; }
        string chunck { get; }
        string name { get; }
        bool binded { get; }
        bool disposed { get; }
        bool enable { get; set; }
        void Update();
        void Bind(FrameworkModuleContainer container);
        void UnBind(bool dispose);
        FrameworkModuleContainer container { get; }
    }
}
