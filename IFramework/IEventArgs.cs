
using IFramework.Pool;

namespace IFramework
{
    /// <summary>
    /// 框架内传递的所有消息的基类
    /// </summary>
    public interface IEventArgs { }
    /// <summary>
    /// 可回收接口
    /// </summary>
    /// <summary>
    /// 可回收消息基类
    /// </summary>
    public abstract class FrameworkArgs : RecyclableObject, IEventArgs
    {
        private bool _argsDirty;
        /// <summary>
        /// 消息是否可用
        /// </summary>
        public bool argsDirty { get { return _argsDirty; }set { _argsDirty = value; } }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override void OnAllocate()
        {
            base.OnAllocate();
            _argsDirty = false;
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }

}
