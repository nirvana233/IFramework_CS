namespace IFramework.Utility.Astar
{
    /// <summary>
    /// 二维 A* 地图节点
    /// </summary>
    public class AStarNode2X : IAStarNode
    {
        private AStarNodeType _nodeType;
        private IAStarNode _parentNode;
        private float _g;
        private float _h;
        /// <summary>
        /// 节点类型（是否可以行走）
        /// </summary>
        public AStarNodeType nodeType { get { return _nodeType; } }
        /// <summary>
        /// 父节点，用于返回路径
        /// </summary>
        public IAStarNode parentNode { get { return _parentNode; } }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public float g { get { return _g; } }
        public float h { get { return _h; } }
        public float f { get { return _g + _h; ; } }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        private Point2 _mappos;
        /// <summary>
        /// 
        /// </summary>
        public Point2 mapPos { get { return _mappos; } }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="mapPos"></param>
        /// <param name="nodeType"></param>
        public AStarNode2X(Point2 mapPos, AStarNodeType nodeType)
        {
            this._mappos = mapPos;
            this._nodeType = nodeType;
            _g = _h = float.MaxValue / 2;
        }
        /// <summary>
        /// 设置默认父节点
        /// </summary>
        /// <param name="node"></param>
        public void SetDefaultParent(IAStarNode node)
        {
            this._parentNode = node;
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
            if (g + h < f)
            {
                this._g = g;
                this._h = h;
                this._parentNode = trySetNode;
            }
            return f;
        }
        /// <summary>
        /// 重置节点数据
        /// </summary>
        public void Reset()
        {
            _g = _h = float.MaxValue / 2;
            _parentNode = null;
        }


    }

}
