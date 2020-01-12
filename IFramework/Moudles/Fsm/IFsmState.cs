namespace IFramework.Moudles.Fsm
{
    public interface IFsmState
    {
        void OnEnter();
        void OnExit();
        void Update();
    }
}
