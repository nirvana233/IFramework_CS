using System;

namespace IFramework
{
    public interface ISingleton : IDisposable
    {
        void OnSingletonInit();
    }

}
