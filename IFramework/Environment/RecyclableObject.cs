using System;

namespace IFramework
{

    /// <summary>
    /// 可回收类
    /// </summary>
    [FrameworkVersion(20)]
    [ScriptVersionUpdate(20,"增加未回收实例的控制")]
    public abstract class RecyclableObject : FrameworkObject, IRecyclable
    {
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name=" envType"></param>
        /// <returns></returns>
        public static RecyclableObject Allocate(Type type, EnvironmentType envType)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envType);
            return Allocate(type, _env);
        }
        /// <summary>
        /// 分配一个实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static RecyclableObject Allocate(Type type, FrameworkEnvironment env)
        {
            RecyclableObject t = env.cycleCollection.Get(type) as RecyclableObject;
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
            FrameworkEnvironment _env = Framework.GetEnv(envType);
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
            T t = env.cycleCollection.Get<T>();
            t._env = env;
            t.OnAllocate();
            return t;
        }


        /// <summary>
        /// 通过唯一id回收对象
        /// </summary>
        /// <param name="env"></param>
        /// <param name="guid"></param>
        public static void RecyleByGuid(FrameworkEnvironment env, Guid guid)
        {
            env.cycleCollection.Recyle(guid);
        }
        /// <summary>
        /// 回收所有实例
        /// </summary>
        /// <param name="env"></param>
        public static void RecyleAll(FrameworkEnvironment env)
        {
            env.cycleCollection.RecyleAll();
        }
        /// <summary>
        /// 获取没有回收的实例
        /// </summary>
        /// <param name="env"></param>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetFromMemory(FrameworkEnvironment env, Guid id, out RecyclableObject obj)
        {
            return env.cycleCollection.GetFromMemory(id, out obj);
        }


        /// <summary>
        /// 通过唯一id回收对象
        /// </summary>
        /// <param name="envType"></param>
        /// <param name="guid"></param>
        public static void RecyleByGuid(EnvironmentType envType, Guid guid)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envType);

            RecyleByGuid(_env, guid);
        }
        /// <summary>
        /// 回收所有实例
        /// </summary>
        /// <param name="envType"></param>
        public static void RecyleAll(EnvironmentType envType)
        {
            FrameworkEnvironment _env = Framework.GetEnv(envType);
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
            FrameworkEnvironment _env = Framework.GetEnv(envType);
            return GetFromMemory(_env, id, out obj);
        }

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
        /// 被分配时
        /// </summary>
        protected virtual void OnAllocate()
        {
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
            _env.cycleCollection.Set(this);
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
}
