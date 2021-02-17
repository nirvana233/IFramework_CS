using System;

namespace IFramework
{
    /// <summary>
    /// 基类
    /// </summary>
    public class FrameworkObject : IDisposable, IFrameworkObject
    {
        private bool _disposed;
        private Guid _guid = Guid.NewGuid();
        private string _name;
        /// <summary>
        /// 唯一id
        /// </summary>
        public Guid guid { get { return _guid; } }
        /// <summary>
        /// 是否已经释放
        /// </summary>
        public bool disposed { get { return _disposed; } }

        /// <summary>
        /// 释放时
        /// </summary>
        protected virtual void OnDispose() { }
        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {
            OnDispose();

            _name = string.Empty;
            _guid = Guid.Empty;
            _disposed = true;
        }

    }
    /// <summary>
    /// 积累接口
    /// </summary>
    public interface IFrameworkObject:IEventArgs
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        Guid guid { get; }
        /// <summary>
        /// 是否释放
        /// </summary>
        bool disposed { get; }
    }
}
