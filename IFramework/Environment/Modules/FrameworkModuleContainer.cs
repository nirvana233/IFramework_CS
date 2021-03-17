using System;
using System.Collections.Generic;

namespace IFramework.Modules
{
    /// <summary>
    /// 模块容器
    /// </summary>
    internal class FrameworkModuleContainer : DisposableObject, IFrameworkModuleContainer,IBelongToEnvironment
    {
        private bool _binded;
        private IEnvironment _env;
        /// <summary>
        /// 环境
        /// </summary>
        public IEnvironment env { get { return _env; } }

        private LockParam _lock=new LockParam();
        private Dictionary<Type, List<FrameworkModule>> moudle_dic;
        private List<UpdateFrameworkModule> update_list;


        /// <summary>
        /// 创建一个模块，创建完了自动绑定
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameworkModule CreateModule(Type type, string name = "")
        {
            using (new LockWait(ref _lock))
            {
                var mou = FrameworkModule.CreatInstance(type, name);
                mou.Bind(this);
                return mou;
            }
        }
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T CreateModule<T>(string name = "") where T : FrameworkModule
        {
            return CreateModule(typeof(T), name) as T;
        }

        /// <summary>
        /// 查找模块
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <param name="name">模块名称</param>
        /// <returns></returns>
        public FrameworkModule FindModule(Type type, string name = "")
        {
            using (new LockWait(ref _lock))
            {
                if (string.IsNullOrEmpty(name))
                    name = type.Name;
                if (!moudle_dic.ContainsKey(type)) return null;
                return moudle_dic[type].Find((m) => { return m.name == name; });
            }

        }
        /// <summary>
        /// 获取模块
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameworkModule GetModule(Type type, string name = "")
        {
            var tmp = FindModule(type, name);
            if (tmp == null)
            {
                tmp = CreateModule(type, name);
            }
            return tmp;
        }


        /// <summary>
        /// 查找模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T FindModule<T>(string name = "") where T : FrameworkModule
        {
            return FindModule(typeof(T), name) as T;
        }
        /// <summary>
        /// 获取模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetModule<T>(string name = "") where T : FrameworkModule
        {
            return GetModule(typeof(T), name) as T;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="bind"></param>
        public FrameworkModuleContainer( FrameworkEnvironment env, bool bind = true)
        {
            using (new LockWait(ref _lock))
            {
                this._env = env;
                update_list = new List<UpdateFrameworkModule>();
                moudle_dic = new Dictionary<Type, List<FrameworkModule>>();
                if (bind)
                    BindEnv();
            }
        }
        /// <summary>
        /// 绑定环境
        /// </summary>
        private void BindEnv()
        {
            if (_binded)
            {
                Log.E(string.Format("Have Bind Container Type : {0}", GetType()));
                return;
            }
            _binded = true;
            _env.BindUpdate(Update);
            _env.BindDispose(Dispose);
        }
        /// <summary>
        /// 解绑环境
        /// </summary>
        /// <param name="dispose"></param>
        private void UnBindEnv(bool dispose = true)
        {
            if (_binded)
            {
                _binded = false;
                _env.UnBindUpdate(Update);
                _env.UnBindDispose(Dispose);
            }
            else
            {
                //Log.E(string.Format("Have Not Bind Container Type : {0} chunck : {1}", GetType(), chunck));
            }
            if (dispose)
                Dispose();
        }
        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            using (new LockWait(ref _lock))
            {
                UnBindEnv(false);
                List<FrameworkModule> list = new List<FrameworkModule>();
                foreach (var item in moudle_dic.Values)
                {
                    for (int i = 0; i < item.Count; i++)
                    {
                        list.Add(item[i]);
                    }
                }
                list.Sort((x, y) => { return y.priority.CompareTo(x.priority); });
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Dispose();
                }
                update_list.Clear();
                moudle_dic.Clear();
                update_list = null;
                moudle_dic = null;
            }
        }
        private void Update()
        {
            using (new LockWait(ref _lock))
            {
                update_list.ForEach((m) => { m.Update(); });
            }
        }

        internal bool SubscribeModule(FrameworkModule moudle)
        {
            Type type = moudle.GetType();
            if (!moudle_dic.ContainsKey(type))
                moudle_dic.Add(type, new List<FrameworkModule>());
            var list = moudle_dic[type];
            var tmpModule = list.Find((m) => { return moudle.name == m.name; });
            if (tmpModule == null)
            {
                list.Add(moudle);
                if (moudle is UpdateFrameworkModule)
                {
                    update_list.Add(moudle as UpdateFrameworkModule);
                }
                return true;
            }
            else
            {
                Log.E(string.Format("Have Bind Module | Type {0}  Name {1}", type, moudle.name));
                return false;
            }
        }
        internal bool UnSubscribeBindModule(FrameworkModule moudle)
        {
            Type type = moudle.GetType();
            if (!moudle_dic.ContainsKey(type))
            {
                Log.E(string.Format("01,Have Not Bind Module | Type {0}  Name {1}", type, moudle.name));
                return false;
            }
            else
            {
                var tmpModule = moudle_dic[type].Find((m) => { return moudle.name == m.name; });
                if (tmpModule == null)
                {
                    Log.E(string.Format("02,Have Not Bind Module | Type {0}  Name {1}", type, moudle.name));
                    return false;
                }
                else
                {
                    if (moudle is UpdateFrameworkModule)
                    {
                        update_list.Remove(moudle as UpdateFrameworkModule);
                    }

                    moudle_dic[type].Remove(moudle);
                    return true;
                }
            }
        }


    }

}
