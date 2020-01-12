using System;
using System.Collections;

namespace IFramework.Moudles.NodeAction
{
    public interface IActionNode : IEnumerator, IDisposable
    {
        bool Disposed { get; }
        bool IsDone { get; }
        bool AutoDispose { get; set; }
        event Action onBegin;
        event Action onCompelete;
        event Action onDispose;
    }

}
