using System;
using System.Collections.Generic;

namespace IFramework.Utility.Astar
{
    /// <summary>
    /// 二维A* 地图
    /// </summary>
    public class AStarMap2X : IAstarMap<AStarNode2X>
    {
        private List<AStarNode2X> neighborNodes;
        private AStarNode2X[,] map;
        /// <summary>
        /// 便于调用点位
        /// </summary>
        /// <returns></returns>
        public AStarNode2X this[Point2 pos]
        {
            get { return map[(int)pos.x, (int)pos.y]; }
        }
        private Point2 size;
       // private int len, wid;
        private bool _walkSideways;
        /// <summary>
        /// 是否可以斜着走
        /// </summary>
        public bool walkSideways { get { return _walkSideways; } set { _walkSideways = value; } }
        /// <summary>
        /// Ctor
        /// </summary>
        public AStarMap2X()
        {
            neighborNodes = new List<AStarNode2X>(8);
        }
        /// <summary>
        /// 计算地图上两点距离
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public float GetHCost(AStarNode2X start, AStarNode2X end)
        {
            return (float)Point2.Distance(start.mapPos, end.mapPos);
        }
        /// <summary>
        /// 获取邻居节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public AStarNode2X[] GetNeighborNodes(AStarNode2X node)
        {
            neighborNodes.Clear();
            if (walkSideways)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (node.mapPos.x - 1 + i >= 0 && node.mapPos.x - 1 + i < size.x)
                        {
                            if (node.mapPos.y- 1 + j >= 0 && node.mapPos.y - 1 + j < size.y)
                            {
                                if (j == 1 && i == 1) continue;
                                if (map[(int)node.mapPos.x - 1 + i, (int)node.mapPos.y - 1 + j].nodeType == AStarNodeType.Walkable)
                                {
                                    neighborNodes.Add(map[(int)node.mapPos.x - 1 + i, (int)node.mapPos.y - 1 + j]);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (node.mapPos.x - 1 + i >= 0 && node.mapPos.x - 1 + i < size.x)
                        {
                            if (node.mapPos.y - 1 + j >= 0 && node.mapPos.y - 1 + j < size.y)
                            {
                                if ((i - 1) * (j - 1) == 0 && i != j)
                                {
                                    if (map[(int)node.mapPos.x - 1 + i, (int)node.mapPos.y - 1 + j].nodeType == AStarNodeType.Walkable)
                                    {
                                        neighborNodes.Add(map[(int)node.mapPos.x - 1 + i, (int)node.mapPos.y - 1 + j]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return neighborNodes.ToArray();
        }
        /// <summary>
        /// 加载地图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">更具输入数据转化为NodeType</param>
        /// <param name="arr">二维数组</param>
        /// <param name="walkSideways">是否可以斜着走</param>
        public void ReadMap<T>(Func<T, AStarNodeType> func, T[,] arr, bool walkSideways = true)
        {
            this.walkSideways = walkSideways;
            map = new AStarNode2X[arr.GetLength(0), arr.GetLength(1)];
            size = new Point2(map.GetLength(1), map.GetLength(0));
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    map[i, j] = new AStarNode2X(new Point2(i,j), func(arr[i, j]));
                }
            }
        }
        /// <summary>
        /// 重置地图数据
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    map[i, j].Reset();
                }
            }
        }
    }

}
