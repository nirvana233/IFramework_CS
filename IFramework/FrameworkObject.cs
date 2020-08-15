using System;

namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class FrameworkObject : IDisposable
    {
        private string _name;
        private bool _disposed;
        private Guid _guid = Guid.NewGuid();
        public Guid guid { get { return _guid; } }
        public bool disposed { get { return _disposed; } }
        public string name { get { return _name; } set { _name = value; } }

        protected virtual void OnDispose() { }
        public virtual void Dispose()
        {
            Dispose(null, null);
        }
        protected void Dispose(Action frontofonDispose, Action frontof_disposed)
        {
            if (_disposed) return;
            if (frontofonDispose != null)
                frontofonDispose();
            OnDispose();
            if (frontof_disposed != null)
                frontof_disposed();
            _name = string.Empty;
            _guid = Guid.Empty;
            _disposed = true;
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
