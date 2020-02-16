namespace IFramework
{
    public interface IPoolObject
    {
        void OnCreate(IEventArgs arg);
        void OnGet(IEventArgs arg);
        void OnSet(IEventArgs arg);
        void OnClear(IEventArgs arg);
    }

}
