using System;
using System.Collections;

namespace IFramework.Modules.NodeAction
{
    internal interface IActionNode 
    {
        bool disposed { get; }
        bool isDone { get; }
        bool autoRecyle { get; set; }
        bool MoveNext();
        void NodeReset();
        //event Action onBegin;
        //event Action onCompelete;
        //event Action onDispose;
        //event Action onRecyle;
    }

}
