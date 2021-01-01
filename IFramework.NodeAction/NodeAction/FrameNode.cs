namespace IFramework.NodeAction
{
    class FrameNode : ActionNode, IFrameNode
    {
        private int _frame;
        private int _current;
        internal void Config(int frame,bool autoRecyle)
        {
            this._frame = frame;
            base.Config(autoRecyle);
        }
        protected override void OnBegin() { }

        protected override void OnCompelete() { }

        protected override bool OnMoveNext()
        {
            return _current++ < _frame;
        }
        protected override void OnDataReset()
        {
            base.OnDataReset();
            _current = 0;
            _frame = 0;
        }
        protected override void OnNodeReset()
        {
            _current = 0;
        }
    }

}
