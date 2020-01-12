namespace IFramework.Utility.Astar
{
    /// <summary>
    /// 二维 A* 地图节点
    /// </summary>
    public class AStarNode2X : IAStarNode
    {
        private AStarNodeType nodeType;
        private IAStarNode parentNode;
        private float g;
        private float h;
        /// <summary>
        /// 节点类型（是否可以行走）
        /// </summary>
        public AStarNodeType NodeType { get { return nodeType; } }
        /// <summary>
        /// 父节点，用于返回路径
        /// </summary>
        public IAStarNode ParentNode { get { return parentNode; } }
        public float G { get { return g; } }
        public float H { get { return h; } }
        public float F { get { return g + h; ; } }
        private int m_x;
        private int m_y;
        /// <summary>
        /// 地图位置 坐标
        /// </summary>
        public int Y { get { return m_y; } }
        /// <summary>
        /// 地图位置 坐标
        /// </summary>
        public int X { get { return m_x; } }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="mapPosX"></param>
        /// <param name="mapPosY"></param>
        /// <param name="nodeType"></param>
        public AStarNode2X(int mapPosX, int mapPosY, AStarNodeType nodeType)
        {
            this.m_x = mapPosX;
            this.m_y = mapPosY;
            this.nodeType = nodeType;
            g = h = float.MaxValue / 2;
        }
        /// <summary>
        /// 设置默认父节点
        /// </summary>
        /// <param name="node"></param>
        public void SetDefaultParent(IAStarNode node)
        {
            this.parentNode = node;
        }
        /// <summary>
        /// 尝试更新最短距离
        /// </summary>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="trySetNode"></param>
        /// <returns></returns>
        public float TryUpdateFCost(float g, float h, IAStarNode trySetNode)
        {
            if (g + h < F)
            {
                this.g = g;
                this.h = h;
                this.parentNode = trySetNode;
            }
            return F;
        }
        /// <summary>
        /// 重置节点数据
        /// </summary>
        public void Reset()
        {
            g = h = float.MaxValue / 2;
            parentNode = null;
        }


    }

}
