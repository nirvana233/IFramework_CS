using System;

namespace IFramework.NodeAction
{
    /// <summary>
    /// 节点
    /// </summary>
    public interface IActionNode:IRecyclable
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        bool isDone { get; }
        /// <summary>
        /// 自动回收
        /// </summary>
        bool autoRecyle { get; }


    }
}