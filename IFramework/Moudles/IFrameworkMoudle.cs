using System;

namespace IFramework.Moudles
{
    internal interface IFrameworkMoudle : IDisposable
    {
        string name { get; }
        bool disposed { get; }
        bool enable { get; set; }
        void Update();
    }
}
