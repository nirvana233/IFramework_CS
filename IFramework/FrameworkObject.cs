using System;

namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class FrameworkObject : IDisposable, IFrameworkObject
    {
        private bool _disposed;
        private Guid _guid = Guid.NewGuid();
        private string _name;

        public Guid guid { get { return _guid; } }
        public bool disposed { get { return _disposed; } }
        public string name { get { return _name; } set { _name = value; } }

        protected virtual void OnDispose() { }

        public virtual void Dispose()
        {
            OnDispose();

            _name = string.Empty;
            _guid = Guid.Empty;
            _disposed = true;
        }

    }

    public interface IFrameworkObject:IEventArgs
    {
        Guid guid { get; }
        bool disposed { get; }
        string name { get; set; }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
