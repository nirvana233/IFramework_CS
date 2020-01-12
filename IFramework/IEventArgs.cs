
using System;

namespace IFramework
{
    public interface IEventArgs { }
    public interface IEventArgs<T> : IEventArgs { Type EventType { get; } }
}
