using System;
using System.Collections.Generic;

namespace IFramework.Modules
{
    /// <summary>
    /// 模块容器
    /// </summary>
    class FrameworkModuleContainer : Unit, IFrameworkModuleContainer, IBelongToEnvironment
    {
        private IEnvironment _env;
        /// <summary>
        /// 环境
        /// </summary>
        public IEnvironment env { get { return _env; } }

        private LockParam _lock = new LockParam();
        private Dictionary<Type, Dictionary<string, FrameworkModule>> moudle_dic;
        private List<UpdateFrameworkModule> update_list;

        /// <summary>
        /// 创建一个模块，创建完了自动绑定
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameworkModule CreateModule(Type type, string name = FrameworkModule.defaultName)
        {
            var mou = FrameworkModule.CreatInstance(type, name);
            mou.Bind(this);
            return mou;
        }
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T CreateModule<T>(string name = FrameworkModule.defaultName) where T : FrameworkModule
        {
            return CreateModule(typeof(T), name) as T;
        }


        /// <summary>
        /// 查找模块
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <param name="name">模块名称</param>
        /// <returns></returns>
        public FrameworkModule FindModule(Type type, string name = FrameworkModule.defaultName)
        {
            if (string.IsNullOrEmpty(name))
                name = type.Name;
            if (!moudle_dic.ContainsKey(type)) return null;
            if (!moudle_dic[type].ContainsKey(name)) return null;
            var module = moudle_dic[type][name];
            return module;

        }
        /// <summary>
        /// 获取模块
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameworkModule GetModule(Type type, string name = FrameworkModule.defaultName)
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
        public T FindModule<T>(string name = FrameworkModule.defaultName) where T : FrameworkModule
        {
            return FindModule(typeof(T), name) as T;
        }
        /// <summary>
        /// 获取模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetModule<T>(string name = FrameworkModule.defaultName) where T : FrameworkModule
        {
            return GetModule(typeof(T), name) as T;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="env"></param>
        public FrameworkModuleContainer(FrameworkEnvironment env)
        {
            this._env = env;
            update_list = new List<UpdateFrameworkModule>();
            moudle_dic = new Dictionary<Type, Dictionary<string, FrameworkModule>>();
        }
        /// <summary>
        /// 绑定环境
        /// </summary>

        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            // using (new LockWait(ref _lock))
            {
                List<FrameworkModule> list = new List<FrameworkModule>();
                foreach (var item in moudle_dic.Values)
                {
                    foreach (var _item in item.Values)
                    {
                        list.Add(_item);
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
        internal void Update()
        {
            //  using (new LockWait(ref _lock))
            {
                for (int i = 0; i < update_list.Count; i++)
                {
                    update_list[i].Update();
                }
            }
        }


        internal bool SubscribeModule(FrameworkModule moudle)
        {
            using (new LockWait(ref _lock))
            {
                Type type = moudle.GetType();
                if (!moudle_dic.ContainsKey(type))
                    moudle_dic.Add(type, new Dictionary<string, FrameworkModule>());
                var list = moudle_dic[type];
                if (list.ContainsKey(moudle.name))
                {
                    Log.E(string.Format("Have Bind Module | Type {0}  Name {1}", type, moudle.name));
                    return false;
                }
                else
                {
                    list.Add(moudle.name, moudle);
                    if (moudle is UpdateFrameworkModule)
                    {
                        update_list.Add(moudle as UpdateFrameworkModule);
                    }
                    return true;
                }
            }


        }
        internal bool UnSubscribeBindModule(FrameworkModule moudle)
        {
            using (new LockWait(ref _lock))
            {
                Type type = moudle.GetType();
                if (!moudle_dic.ContainsKey(type))
                {
                    Log.E(string.Format("01,Have Not Bind Module | Type {0}  Name {1}", type, moudle.name));
                    return false;
                }
                else
                {
                    var list = moudle_dic[type];

                    if (!list.ContainsKey(moudle.name))
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
                        moudle_dic[type].Remove(moudle.name);
                        return true;
                    }
                }
            }

        }


    }

}
