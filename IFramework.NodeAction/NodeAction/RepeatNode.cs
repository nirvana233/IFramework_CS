using System.Collections.Generic;

namespace IFramework.NodeAction
{
    [VersionAttribute(3)]
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class RepeatNode : ContainerNode
    {
        private int _curRepeat;
        private int _repeat;
        private int curRepeat { get { return _curRepeat; } }
        public int repeat { get { return _repeat; } set { _repeat = value; } }
        public ActionNode node { get { return nodeList[0]; }set { nodeList[0] = value; } }

        public RepeatNode() : base() { }
        public void Config(int repeat, bool autoRecyle)
        {
            this._repeat = repeat;
            base.Config(autoRecyle);
        }
        public override void Append(ActionNode node)
        {
            if (nodeList.Count >= 1)
                Log.E("RepeatNode Can Have One Inner Node");
            else
                nodeList.Add(node);
        }



        protected override bool OnMoveNext()
        {
            if (_repeat == -1)
            {
                if (!node.MoveNext())
                    node.NodeReset();
                return true;
            }
            if (!node.MoveNext())
            {
                node.NodeReset();
                _curRepeat++;
            }
            if (_curRepeat >= _repeat)
                return false;
            return true;
        }
        

        protected override void OnDataReset()
        {
            base.OnDataReset();
            _repeat = -1;
            _curRepeat = 0;
        }

        protected override void OnNodeReset()
        {
            base.OnNodeReset();
            _curRepeat = 0;
        }

        protected override void OnBegin() { }
        protected override void OnCompelete() { }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
