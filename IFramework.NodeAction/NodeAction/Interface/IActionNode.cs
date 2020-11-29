using System;

namespace IFramework.NodeAction
{
    /// <summary>
    /// 节点
    /// </summary>
    public interface IActionNode
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        bool isDone { get; }
        /// <summary>
        /// 自动回收
        /// </summary>
        bool autoRecyle { get; }
        /// <summary>
        /// 唯一ID
        /// </summary>
        Guid guid { get; }
        /// <summary>
        /// 名字
        /// </summary>
        string name { get; }
        /// <summary>
        /// 所属环境
        /// </summary>
        FrameworkEnvironment env { get; }
        /// <summary>
        /// 是否回收了
        /// </summary>
        bool recyled { get; }
        /// <summary>
        /// 结束
        /// </summary>
        void Recyle();
    }
}