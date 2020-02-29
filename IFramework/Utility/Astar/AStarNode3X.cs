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
        private Point3 _mappos;
        /// <summary>
        /// 
        /// </summary>
        public Point3 mapPos { get { return _mappos; } }

        /// <summary>
        /// ctor
        /// </summary>
        public AStarNode3X(Point3 mapPos, AStarNodeType nodeType)
        {
            this._mappos = mapPos;
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
