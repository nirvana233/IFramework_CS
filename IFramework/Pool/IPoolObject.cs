namespace IFramework
{
    public interface IPoolObject
    {
        void OnCreate(IEventArgs arg, params object[] param);
        void OnGet(IEventArgs arg, params object[] param);
        void OnSet(IEventArgs arg, params object[] param);
        void OnClear(IEventArgs arg, params object[] param);
    }

}
