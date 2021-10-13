namespace IFramework.Utility.Astar
{/// <summary>
/// 地图
/// </summary>
/// <typeparam name="T"></typeparam>
    public interface IAstarMap<T> where T : IAStarNode
    {
        /// <summary>
        /// 是否可以斜着走
        /// </summary>
        bool walkSideways { get; set; }
        /// <summary>
        /// 获取邻居节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        T[] GetNeighborNodes(T node);
        /// <summary>
        /// 获取两点之间距离
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        float GetHCost(T start, T end);
        /// <summary>
        /// 重置数据
        /// </summary>
        void Reset();
    }

}
