
using System;

namespace IFramework
{
    public interface IEventArgs { }
    public interface IEventArgs<T> : IEventArgs { Type EventType { get; } }
    public abstract class FrameworkArgs : IEventArgs
    {
        public static T Allocate<T>() where T : FrameworkArgs
        {
            T arg = Framework.argPool.Allocate<T>();
            arg.Reset();
            return arg;
        }
        public void Recyle()
        {
            Framework.argPool.Set(this);
        }

        private bool _dirty;
        public bool dirty { get { return _dirty; } }

        protected abstract void OnReset();
        public void Reset()
        {
            _dirty = false;
            OnReset();
        }
        public void SetDirty()
        {
            _dirty = true;
        }
    }
}
