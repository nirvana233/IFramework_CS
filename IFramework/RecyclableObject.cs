using IFramework.Pool;
using System;

namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public interface IRecyclable
    {
        void Recyle();
        void ResetData();
        bool recyled { get; }
    }

    /// <summary>
    /// 可回收类
    /// </summary>
    public abstract class RecyclableObject :FrameworkObject, IRecyclable
    {
        private bool _recyled;
        private bool _datadirty;
        private FrameworkEnvironment _env;
        /// <summary>
        /// 是否被回收
        /// </summary>
        public bool recyled { get { return _recyled; } }
        /// <summary>
        /// 数据是否发生改变
        /// </summary>
        public bool dataDirty { get { return _datadirty; } }
        /// <summary>
        /// 当前所处环境
        /// </summary>
        public FrameworkEnvironment env { get { return _env; } set { _env = value; } }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name=" envType"></param>
        /// <returns></returns>
        public static RecyclableObject Allocate(Type type, EnvironmentType envType) 
        {
            FrameworkEnvironment _env = Framework.GetEnv( envType);
            return Allocate(type,_env);
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static RecyclableObject Allocate(Type type, FrameworkEnvironment env)
        {
            RecyclableObject t = env.cyclePool.Get(type) as RecyclableObject;
            t._env = env;
            t.OnAllocate();
            return t;
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <typeparam name="T"> RecyclableObject </typeparam>
        /// <param name=" envType"></param>
        /// <returns></returns>
        public static T Allocate<T>(EnvironmentType envType) where T : RecyclableObject
        {
            FrameworkEnvironment _env = Framework.GetEnv( envType);
            return Allocate<T>(_env);
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="env"></param>
        /// <returns></returns>
        public static T Allocate<T>(FrameworkEnvironment env) where T : RecyclableObject
        {
            T t = env.cyclePool.Get<T>();
            t._env = env;
            t.OnAllocate();
            return t;
        }
        /// <summary>
        /// 被分配时
        /// </summary>
        protected virtual void OnAllocate() {
            _recyled = false;
            ResetData();
        }
        /// <summary>
        /// 回收
        /// </summary>
        public void Recyle()
        {
            if (_recyled) return;
            OnRecyle();
            _recyled = true;
            _env.cyclePool.Set(this);
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        public void ResetData()
        {
            if (!_datadirty) return;
            OnDataReset();
            _datadirty = false;
        }
        /// <summary>
        /// 设置数据发生改动
        /// </summary>
        public void SetDataDirty()
        {
            _datadirty = true;
        }
        /// <summary>
        /// 被回收时
        /// </summary>
        protected virtual void OnRecyle() { ResetData(); }
        /// <summary>
        /// 数据重置时
        /// </summary>
        protected abstract void OnDataReset();

      
    }
    public class RecyclableObjectPool : BaseTypePool<IRecyclable> { }


#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
