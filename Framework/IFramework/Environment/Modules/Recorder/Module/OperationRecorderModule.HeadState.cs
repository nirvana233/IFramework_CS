namespace IFramework.Modules.Recorder
{
    public partial class OperationRecorderModule
    {
        private class HeadState : BaseState
        {
            protected override void OnRedo() { }

            protected override void OnUndo() { }

            protected override void OnReset() { }
        }
    }
}
