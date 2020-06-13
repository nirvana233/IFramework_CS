namespace IFramework.Utility.Astar
{
    /// <summary>
    /// A* 路径搜寻者
    /// </summary>
    /// <typeparam name="Node">A* 节点类型</typeparam>
    /// <typeparam name="Map">A* 地图leix</typeparam>
    public interface IAStarSearcher<Node, Map> where Node : IAStarNode where Map : IAstarMap<Node>
    {
        /// <summary>
        /// 寻路
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <returns>节点路径</returns>
        Node[] Search(Node start, Node end);
        /// <summary>
        /// 加载 A* 地图
        /// </summary>
        /// <param name="map"></param>
        void LoadMap(Map map);
    }
}
