using System;
using System.Collections.Generic;

namespace IFramework.Utility.Astar
{
    /// <summary>
    /// 三维A* 地图
    /// </summary>
    public class AStarMap3X : IAstarMap<AStarNode3X>
    {
        private bool _walkSideways;
        /// <summary>
        /// 是否可以斜着走
        /// </summary>
        public bool walkSideways { get { return _walkSideways; } set { _walkSideways = value; } }
        private List<AStarNode3X> neighborNodes;
        private AStarNode3X[,,] map;
        private Point3 size;
      //  private int len, wid, hei;
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
        /// <returns></returns>
        public AStarNode3X this[Point3 point]
        {
            get { return map[(int)point.x, (int)point. y, (int)point.z]; }
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
            size = new Point3(map.GetLength(1), map.GetLength(0), map.GetLength(2));


            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    for (int k = 0; k < size.z; k++)
                    {
                        map[i, j, k] = new AStarNode3X(new Point3(i, j, k), func(arr[i, j, k]));

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
            return Point3.Distance(start.mapPos, end.mapPos);
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

                            if (node.mapPos.x - 1 + i >= 0 && node.mapPos.x - 1 + i < size.x)
                            {
                                if (node.mapPos.y - 1 + j >= 0 && node.mapPos.y - 1 + j < size.y)
                                {
                                    if (node.mapPos.z - 1 + k >= 0 && node.mapPos.z - 1 + k < size.z)
                                    {
                                        if (i == 1 && j == 1 && k == 1) continue;
                                        if (map[(int)node.mapPos.x - 1 + i, (int)node.mapPos.y - 1 + j, (int)node.mapPos.z - 1 + k].nodeType == AStarNodeType.Walkable)
                                        {
                                            neighborNodes.Add(map[(int)node.mapPos.x - 1 + i, (int)node.mapPos.y - 1 + j, (int)node.mapPos.z - 1 + k]);
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

                            if (node.mapPos.x - 1 + i >= 0 && node.mapPos.x - 1 + i < size.x)
                            {
                                if (node.mapPos.y - 1 + j >= 0 && node.mapPos.y - 1 + j < size.y)
                                {
                                    if (node.mapPos.z - 1 + k >= 0 && node.mapPos.z - 1 + k < size.z)
                                    {
                                        if ((i - 1) * (j - 1) * (k - 1) == 0 && i != j && j != k)
                                        {
                                            if (map[(int)node.mapPos.x - 1 + i, (int)node.mapPos.y - 1 + j, (int)node.mapPos.z - 1 + k].nodeType == AStarNodeType.Walkable)
                                            {
                                                neighborNodes.Add(map[(int)node.mapPos.x - 1 + i, (int)node.mapPos.y - 1 + j, (int)node.mapPos.z - 1 + k]);
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
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    for (int k = 0; k < size.z; k++)
                    {
                        map[i, j, k].Reset();
                    }
                }
            }
        }
    }

}
