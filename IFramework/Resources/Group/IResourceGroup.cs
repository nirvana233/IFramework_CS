using System;

namespace IFramework.Resources
{
    interface IResourceGroup : IDisposable
    {
        string name { get; set; }

        void ClearUnuseResources();
    }
}
