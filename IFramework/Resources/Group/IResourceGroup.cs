using System;

namespace IFramework.Resource
{
    interface IResourceGroup : IDisposable
    {
        string name { get; set; }

        void ClearUnuseResources();
    }
}
