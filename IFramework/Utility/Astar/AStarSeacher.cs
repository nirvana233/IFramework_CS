using System.Collections.Generic;

namespace IFramework.Utility.Astar
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class AStarSeacher<Node, Map> : IAStarSearcher<Node, Map> where Node : IAStarNode where Map : IAstarMap<Node>
    {
        private Map _map;
        private List<Node> _openList;
        private List<Node> _closeList;

        public void LoadMap(Map map)
        {
            this._map = map;
            _openList = new List<Node>();
            _closeList = new List<Node>();
            _pathNodes = new List<Node>();
        }

        private List<Node> _pathNodes;
        private Node _curNode;
        private Node[] _neighborNodes;



        public Node[] Search(Node start, Node end)
        {
            _map.Reset();
            _openList.Clear();
            _closeList.Clear();
            _pathNodes.Clear();
            start.TryUpdateFCost(0, _map.GetHCost(start, end), null);
            _curNode = start;
            while (!_curNode.Equals(end))
            {
                _neighborNodes = _map.GetNeighborNodes(_curNode);
                for (int i = 0; i < _neighborNodes.Length; i++)
                {
                    if (!_openList.Contains(_neighborNodes[i]) && !_closeList.Contains(_neighborNodes[i]))
                    {
                        _openList.Add(_neighborNodes[i]);
                        _neighborNodes[i].SetDefaultParent(_curNode);
                    }
                    _neighborNodes[i].TryUpdateFCost(_curNode.g + _map.GetHCost(_curNode, _neighborNodes[i]), _map.GetHCost(_neighborNodes[i], end), _curNode);
                }
                float smallestF = float.MaxValue;
                for (int i = 0; i < _openList.Count; i++)
                {
                    if (smallestF > _openList[i].f)
                    {
                        smallestF = _openList[i].f;
                        _curNode = _openList[i];
                    }
                }
                if (_openList.Count == 0)
                {
                    break;
                }
                _closeList.Add(_curNode);
                _openList.Remove(_curNode);
            }
            while (!_curNode.parentNode.Equals(start))
            {
                _pathNodes.Insert(0, _curNode);
                _curNode = (Node)_curNode.parentNode;
            }
            _pathNodes.Insert(0, _curNode);
            return _pathNodes.ToArray();
        }


    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
