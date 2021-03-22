using System;

namespace IFramework
{

    /// <summary>
    /// 可回收类
    /// </summary>
    [ScriptVersionAttribute(20)]
    [VersionUpdateAttribute(20, "增加未回收实例的控制")]
    public abstract class RecyclableObject : DisposableObject, IRecyclable, IBelongToEnvironment, IUniqueIDObject
    {
        private static RecyclableObjectCollection GetCollection(IEnvironment env)
        {
            return (env as FrameworkEnvironment).cycleCollection;
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name=" envType"></param>
        /// <returns></returns>
        public static RecyclableObject Allocate(Type type, EnvironmentType envType)
        {
            var _env = Framework.GetEnv(envType);
            return Allocate(type, _env);
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static RecyclableObject Allocate(Type type, IEnvironment env)
        {
            RecyclableObject t = GetCollection(env).Get(type) as RecyclableObject;
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
            var _env = Framework.GetEnv(envType);
            return Allocate<T>(_env);
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="env"></param>
        /// <returns></returns>
        public static T Allocate<T>(IEnvironment env) where T : RecyclableObject
        {
            T t = GetCollection(env).Get<T>();
            t._env = env;
            t.OnAllocate();
            return t;
        }


        /// <summary>
        /// 通过唯一id回收对象
        /// </summary>
        /// <param name="env"></param>
        /// <param name="guid"></param>
        public static void RecyleByGuid(IEnvironment env, Guid guid)
        {
            GetCollection(env).Recyle(guid);
        }
        /// <summary>
        /// 回收所有实例
        /// </summary>
        /// <param name="env"></param>
        public static void RecyleAll(IEnvironment env)
        {
            GetCollection(env).RecyleAll();
        }
        /// <summary>
        /// 获取没有回收的实例
        /// </summary>
        /// <param name="env"></param>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetFromMemory(IEnvironment env, Guid id, out RecyclableObject obj)
        {
            return GetCollection(env).GetFromMemory(id, out obj);
        }


        /// <summary>
        /// 通过唯一id回收对象
        /// </summary>
        /// <param name="envType"></param>
        /// <param name="guid"></param>
        public static void RecyleByGuid(EnvironmentType envType, Guid guid)
        {
            var _env = Framework.GetEnv(envType);

            RecyleByGuid(_env, guid);
        }
        /// <summary>
        /// 回收所有实例
        /// </summary>
        /// <param name="envType"></param>
        public static void RecyleAll(EnvironmentType envType)
        {
            var _env = Framework.GetEnv(envType);
            RecyleAll(_env);
        }
        /// <summary>
        /// 获取没有回收的实例
        /// </summary>
        /// <param name="envType"></param>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetFromMemory(EnvironmentType envType, Guid id, out RecyclableObject obj)
        {
            var _env = Framework.GetEnv(envType);
            return GetFromMemory(_env, id, out obj);
        }

        private bool _recyled;
        private bool _datadirty;
        private IEnvironment _env;
        private Guid _guid=new Guid();

        /// <summary>
        /// 是否被回收
        /// </summary>
        public bool recyled { get { return _recyled; } }
        /// <summary>
        /// 数据是否发生改变
        /// </summary>
        protected bool dataDirty { get { return _datadirty; } }
        /// <summary>
        /// 当前所处环境
        /// </summary>
        public IEnvironment env { get { return _env; } internal set { _env = value; } }
        /// <summary>
        /// 唯一 id
        /// </summary>
        public Guid guid { get { return _guid; } }

        /// <summary>
        /// 回收
        /// </summary>
        public void Recyle()
        {
            if (_recyled) return;
            OnRecyle();
            _recyled = true;
            GetCollection(_env).Set(this);
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        protected void ResetData()
        {
            if (!_datadirty) return;
            OnDataReset();
            _datadirty = false;
        }
        /// <summary>
        /// 设置数据发生改动
        /// </summary>
        protected void SetDataDirty()
        {
            _datadirty = true;
        }

        /// <summary>
        /// 被分配时
        /// </summary>
        protected virtual void OnAllocate()
        {
            _recyled = false;
            ResetData();
        }
        /// <summary>
        /// 被回收时
        /// </summary>
        protected virtual void OnRecyle() { ResetData(); }
        /// <summary>
        /// 数据重置时
        /// </summary>
        protected abstract void OnDataReset();
        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            _guid = Guid.Empty;
        }
    }
}
