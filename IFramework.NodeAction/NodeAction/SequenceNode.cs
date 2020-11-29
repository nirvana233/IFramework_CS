namespace IFramework.NodeAction
{
    [ScriptVersion(3)]
    class SequenceNode : ContainerNode, ISequenceNode
    {
        private int _curIndex;
        private ActionNode curAction
        {
            get
            {
                return nodeList[_curIndex];
            }
        }

        protected override bool OnMoveNext()
        {
            if (_curIndex >= count) return false;
            bool bo = curAction.MoveNext();
            if (bo) return true;
            _curIndex++;
            if (_curIndex >= count) return false;
            return true;
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            //    Log.L(_curIndex + "_ " + count + "_" + isDone);

            _curIndex = 0;
        }

        protected override void OnNodeReset()
        {
            base.OnNodeReset();
            _curIndex = 0;
        }
        protected override void OnBegin()
        {
            //  Log.L(_curIndex+" "+count+""+isDone);
        }
        protected override void OnCompelete() { }
    }

}
