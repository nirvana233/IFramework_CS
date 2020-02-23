namespace IFramework.Utility.Astar
{
    /// <summary>
    /// A*节点
    /// </summary>
    public interface IAStarNode
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        AStarNodeType NodeType { get; }
        /// <summary>
        /// 父节点，用于寻路递归
        /// </summary>
        IAStarNode ParentNode { get; }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        float G { get; }
        float H { get; }
        float F { get; }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /// <summary>
        /// 设置默认父节点
        /// </summary>
        /// <param name="node"></param>
        void SetDefaultParent(IAStarNode node);
        /// <summary>
        /// 尝试刷新数据
        /// </summary>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="trySetNode"></param>
        /// <returns></returns>
        float TryUpdateFCost(float g, float h, IAStarNode trySetNode);

        /// <summary>
        /// 重置数据
        /// </summary>
        void Reset();
    }

}
