
using IFramework.Pool;
using System;

namespace IFramework
{
    /// <summary>
    /// 框架内传递的所有消息的基类
    /// </summary>
    public interface IEventArgs { }
    /// <summary>
    /// 可回收接口
    /// </summary>
    public interface IRecyclable
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        void Recyle();
        void ResetData();
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
    /// <summary>
    /// 可回收类
    /// </summary>
    public abstract class RecyclableObject : IRecyclable
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
        protected virtual void OnRecyle() { }
        /// <summary>
        /// 数据重置时
        /// </summary>
        protected abstract void OnDataReset();
    }
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
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class RecyclableObjectPool : BaseTypePool<IRecyclable> { }


    public interface ILifeTimeObject
    {
        void Awake();
        void OnEnable();
        void OnDisable();
        void Update();
        void Destory();
        bool enable { get; set; }
        string name { get; set; }
    }
    public class LifeTimeObject : RecyclableObject, ILifeTimeObject,IDisposable
    {
        private bool _enable;
        private string _name;
        public string name { get { return _name; } set { _name = value; } }
        public bool enable
        {
            get { return _enable; }
            set
            {
                if (recyled) return;

                if (_enable != value)
                    _enable = value;
                if (_enable)
                    OnEnable();
                else
                    OnDisable();
            }
        }
        private bool _binded;
        public bool binded { get { return _binded; } }
        public void BindEnv()
        {
            Framework.BindEnvUpdate(Update, this.env);
            _binded = true;
        }
        public void UnBindEnv()
        {
            if (_binded)
            {
                Framework.UnBindEnvUpdate(Update, this.env);
                _binded = false;
            }
        }

        protected override void OnAllocate()
        {
            base.OnAllocate();
            (this as ILifeTimeObject).Awake();
            (this as ILifeTimeObject).enable = true; 

        }

        protected override void OnRecyle() { }
        protected override void OnDataReset() { }


        protected virtual void Awake() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void Update() { }
        protected virtual void OnDispose() { }
        protected virtual void OnDestory() { }


        public void Destory()
        {
            (this as ILifeTimeObject).enable = false;
            OnDestory();
            ResetData();
            UnBindEnv();
            Recyle();
        }

        void ILifeTimeObject.Awake()
        {
            Awake();
        }
        void ILifeTimeObject.OnEnable()
        {
            OnEnable();
        }
        void ILifeTimeObject.OnDisable()
        {
            OnDisable();
        }
        void ILifeTimeObject.Update()
        {
            if (recyled) return;
            Update();
        }
        void IDisposable.Dispose()
        {
            OnDispose();
        }

    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
