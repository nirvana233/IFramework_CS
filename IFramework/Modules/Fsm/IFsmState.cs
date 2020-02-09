namespace IFramework.Modules.Fsm
{
    public interface IFsmState
    {
        void OnEnter();
        void OnExit();
        void Update();
    }
}
