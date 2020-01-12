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
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public AStarNode2X this[int x, int y]
        {
            get { return map[x, y]; }
        }
        private int len, wid;
        private bool walkSideways;
        /// <summary>
        /// 是否可以斜着走
        /// </summary>
        public bool WalkSideways { get { return walkSideways; } set { walkSideways = value; } }
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
            return (float)Math.Sqrt((start.X - end.X) * (start.X - end.X) + (start.Y - end.Y) * (start.Y - end.Y));
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
                        if (node.X - 1 + i >= 0 && node.X - 1 + i < len)
                        {
                            if (node.Y - 1 + j >= 0 && node.Y - 1 + j < wid)
                            {
                                if (j == 1 && i == 1) continue;
                                if (map[node.X - 1 + i, node.Y - 1 + j].NodeType == AStarNodeType.Walkable)
                                {
                                    neighborNodes.Add(map[node.X - 1 + i, node.Y - 1 + j]);
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
                        if (node.X - 1 + i >= 0 && node.X - 1 + i < len)
                        {
                            if (node.Y - 1 + j >= 0 && node.Y - 1 + j < wid)
                            {
                                if ((i - 1) * (j - 1) == 0 && i != j)
                                {
                                    if (map[node.X - 1 + i, node.Y - 1 + j].NodeType == AStarNodeType.Walkable)
                                    {
                                        neighborNodes.Add(map[node.X - 1 + i, node.Y - 1 + j]);
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
            this.wid = map.GetLength(1);
            this.len = map.GetLength(0);
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < wid; j++)
                {
                    map[i, j] = new AStarNode2X(i, j, func(arr[i, j]));
                }
            }
        }
        /// <summary>
        /// 重置地图数据
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < wid; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    map[i, j].Reset();
                }
            }
        }
    }

}
