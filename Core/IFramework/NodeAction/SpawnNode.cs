namespace IFramework.NodeAction
{
    [ScriptVersion(3)]
    class SpawnNode : ContainerNode, ISpawnNode
    {
        private int _mFinishCount;
        
        protected override void OnDataReset()
        {
            base.OnDataReset();
            _mFinishCount = 0;
        }
        protected override void OnNodeReset()
        {
            base.OnNodeReset();
            _mFinishCount = 0;
        }
        protected override bool OnMoveNext()
        {
            if (_mFinishCount >= count) return false;
            for (var i = count - 1; i >= 0; i--)
            {
                var node = nodeList[i];
                if (!node.MoveNext())
                    _mFinishCount++;
            }
            return _mFinishCount < count;
        }
    }

}
