using System;
using System.Collections.Generic;

namespace IFramework.Utility.Astar
{
    /// <summary>
    /// 三维A* 地图
    /// </summary>
    public class AStarMap3X : IAstarMap<AStarNode3X>
    {
        private bool walkSideways;
        /// <summary>
        /// 是否可以斜着走
        /// </summary>
        public bool WalkSideways { get { return walkSideways; } set { walkSideways = value; } }
        private List<AStarNode3X> neighborNodes;
        private AStarNode3X[,,] map;
        private int len, wid, hei;
        /// <summary>
        /// Ctor
        /// </summary>
        public AStarMap3X()
        {
            neighborNodes = new List<AStarNode3X>(27 - 1);
        }
        /// <summary>
        /// 便于调用点位
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public AStarNode3X this[int x, int y, int z]
        {
            get { return map[x, y, z]; }
        }
        /// <summary>
        /// 加载地图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">更具输入数据转化为NodeType</param>
        /// <param name="arr">二维数组</param>
        /// <param name="walkSideways">是否可以斜着走</param>
        public void ReadMap<T>(Func<T, AStarNodeType> func, T[,,] arr, bool walkSideways = true)
        {

            this.walkSideways = walkSideways;

            map = new AStarNode3X[arr.GetLength(0), arr.GetLength(1), arr.GetLength(2)];
            this.wid = map.GetLength(1);
            this.len = map.GetLength(0);
            this.hei = map.GetLength(2);

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < wid; j++)
                {
                    for (int k = 0; k < hei; k++)
                    {
                        map[i, j, k] = new AStarNode3X(i, j, k, func(arr[i, j, k]));

                    }
                }
            }
        }
        /// <summary>
        /// 计算地图上两点距离
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public float GetHCost(AStarNode3X start, AStarNode3X end)
        {
            return (float)Math.Sqrt((start.X - end.X) * (start.X - end.X) + (start.Y - end.Y) * (start.Y - end.Y) + (start.Z - end.Z) * (start.Z - end.Z));
        }
        /// <summary>
        /// 获取邻居节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public AStarNode3X[] GetNeighborNodes(AStarNode3X node)
        {
            neighborNodes.Clear();
            if (walkSideways)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {

                            if (node.X - 1 + i >= 0 && node.X - 1 + i < len)
                            {
                                if (node.Y - 1 + j >= 0 && node.Y - 1 + j < wid)
                                {
                                    if (node.Z - 1 + k >= 0 && node.Z - 1 + k < hei)
                                    {
                                        if (i == 1 && j == 1 && k == 1) continue;
                                        if (map[node.X - 1 + i, node.Y - 1 + j, node.Z - 1 + k].NodeType == AStarNodeType.Walkable)
                                        {
                                            neighborNodes.Add(map[node.X - 1 + i, node.Y - 1 + j, node.Z - 1 + k]);
                                        }
                                    }
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
                        for (int k = 0; k < 3; k++)
                        {

                            if (node.X - 1 + i >= 0 && node.X - 1 + i < len)
                            {
                                if (node.Y - 1 + j >= 0 && node.Y - 1 + j < wid)
                                {
                                    if (node.Z - 1 + k >= 0 && node.Z - 1 + k < hei)
                                    {
                                        if ((i - 1) * (j - 1) * (k - 1) == 0 && i != j && j != k)
                                        {
                                            if (map[node.X - 1 + i, node.Y - 1 + j, node.Z - 1 + k].NodeType == AStarNodeType.Walkable)
                                            {
                                                neighborNodes.Add(map[node.X - 1 + i, node.Y - 1 + j, node.Z - 1 + k]);
                                            }
                                        }
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
        /// 重置地图数据
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < wid; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    for (int k = 0; k < hei; k++)
                    {
                        map[i, j, k].Reset();
                    }
                }
            }
        }
    }

}
