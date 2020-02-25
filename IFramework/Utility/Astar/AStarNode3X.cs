namespace IFramework.Utility.Astar
{
    /// <summary>
    /// 三维 A* 地图节点
    /// </summary>
    public class AStarNode3X : IAStarNode
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
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public float G { get { return g; } }
        public float H { get { return h; } }
        public float F { get { return g + h; ; } }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        private int m_X;
        private int m_Y;
        private int m_Z;
        /// <summary>
        /// 地图位置 坐标
        /// </summary>
        public int Z { get { return m_Z; } }
        /// <summary>
        /// 地图位置 坐标
        /// </summary>
        public int Y { get { return m_Y; } }
        /// <summary>
        /// 地图位置 坐标
        /// </summary>
        public int X { get { return m_X; } }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="mapPosX"></param>
        /// <param name="mapPosY"></param>
        /// <param name="mapPosZ"></param>
        /// <param name="nodeType"></param>
        public AStarNode3X(int mapPosX, int mapPosY, int mapPosZ, AStarNodeType nodeType)
        {
            this.m_X = mapPosX;
            this.m_Y = mapPosY;
            this.m_Z = mapPosZ;
            g = h = float.MaxValue / 2;
            this.nodeType = nodeType;
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
