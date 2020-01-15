using System;

namespace IFramework.Moudles
{
    internal interface IFrameworkMoudle : IDisposable
    {
        string moudeType { get; }
        string chunck { get; }
        string name { get; }
        bool binded { get; }
        bool disposed { get; }
        bool enable { get; set; }
        void Update();
        void Bind(FrameworkMoudleContainer container);
        void UnBind(bool dispose);
        FrameworkMoudleContainer container { get; }
    }
}
