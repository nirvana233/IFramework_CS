using System;
using System.Reflection;
namespace IFramework.Moudles
{
    internal interface IFrameworkMoudle : IDisposable
    {
        string name { get; }
        bool disposed { get; }
        void Update();
    }
    public abstract class FrameworkMoudle : IFrameworkMoudle
    {
        private string _name;
        private bool _disposed;
        public bool disposed { get { return _disposed; } }
        protected FrameworkMoudle(string chunck)
        {
            _name = string.Format("{0}.{1}", chunck, GetType().Name);
        }
        public static T CreatInstance<T>(string chunk = "Framework") where T : FrameworkMoudle
        {
            return Activator.CreateInstance(typeof(T),
                BindingFlags.Instance| BindingFlags.Public| BindingFlags.NonPublic,
                null,new object[] { chunk },null) as T;
        }
        public string name { get { return _name; } }

        public void Dispose()
        {
            _disposed = true;
            _name = string.Empty;
            OnDispose();
        }
        protected abstract void OnDispose();
        public abstract void Update();
    }
}
