using System;
using System.Collections.Generic;

namespace IFramework.Modules
{
    /// <summary>
    /// 模块容器
    /// </summary>
    public class FrameworkModuleContainer : FrameworkObject
    {
        private string _chunck;
        private bool _binded;
        private FrameworkEnvironment _env;
        /// <summary>
        /// 代码块
        /// </summary>
        public string chunck { get { return _chunck; } }
        /// <summary>
        /// 环境
        /// </summary>
        public FrameworkEnvironment env { get { return _env; } }
        /// <summary>
        /// 是否绑定环境
        /// </summary>
        public bool binded { get { return _binded; } }

        private Dictionary<Type, List<FrameworkModule>> moudle_dic;
        private List<UpdateFrameworkModule> update_list;
        /// <summary>
        /// 查找时候模块不尊在
        /// </summary>
        public event Action<Type, string> onModuleNotExist;

        /// <summary>
        /// 创建一个模块，创建完了自动绑定
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameworkModule CreateModule(Type type,string name="")
        {
            var mou= FrameworkModule.CreatInstance(type, chunck,name);
            mou.Bind(this);
            return mou;
        }
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T CreateModule<T>(string name="") where T : FrameworkModule
        {
            return CreateModule(typeof(T),name) as T;
        }
        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameworkModule this[Type type, string name]
        {
            get { return FindModule(type, name); }
        }
        /// <summary>
        /// 查找模块
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <param name="name">模块名称</param>
        /// <returns></returns>
        public FrameworkModule FindModule(Type type, string name="")
        {
            if (string.IsNullOrEmpty(name))
                name = string.Format("{0}.{1}", _chunck,type.Name);
            FrameworkModule mou = default(FrameworkModule);
            if (moudle_dic.ContainsKey(type))
                mou = moudle_dic[type].Find((m) => { return m.name == name; });
            if (mou == null)
                if (onModuleNotExist != null)
                {
                    onModuleNotExist(type, name);
                    if (moudle_dic.ContainsKey(type))
                        mou = moudle_dic[type].Find((m) => { return m.name == name; });
                }
            return mou;
        }
        /// <summary>
        /// 查找模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T FindModule<T>(string name="") where T : FrameworkModule
        {
            return FindModule(typeof(T), name) as T;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="chunck"></param>
        /// <param name="env"></param>
        /// <param name="bind"></param>
        public FrameworkModuleContainer(string chunck,FrameworkEnvironment env,bool bind=true)
        {
            _chunck = chunck;
            this._env = env;
            update_list = new List<UpdateFrameworkModule>();
            moudle_dic = new Dictionary<Type, List<FrameworkModule>>();
            if (bind)
                BindEnv();
        }
        /// <summary>
        /// 绑定环境
        /// </summary>
        public void BindEnv()
        {
            if (_binded)
            {
                Log.E(string.Format("Have Bind Container Type : {0} chunck : {1}", GetType(), chunck));
                return;
            }
            _binded = true;
            _env.update += Update;
            _env.onDispose += Dispose;
        }
        /// <summary>
        /// 解绑环境
        /// </summary>
        /// <param name="dispose"></param>
        public void UnBindEnv(bool dispose=true)
        {
            if (_binded)
            {
                _binded = false;
                _env.update += Update;
                _env.onDispose += Dispose;
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
            base.OnDispose();
            UnBindEnv(false);
            //for (int i = update_list.Count - 1; i >= 0; i--)
            //{
            //    var m = update_list[i];
            //    m.Dispose();
            //}
            List<FrameworkModule> list = new List<FrameworkModule>();
            foreach (var item in moudle_dic.Values)
            {
                item.ForEach((index,m) => { list.Add(m); });
            }

            list.Sort((x, y) => { return y.priority.CompareTo(x.priority); });
            list.ForEach((index, m) => { m.Dispose(); });
            update_list.Clear();
            moudle_dic.Clear();
            update_list = null;
            moudle_dic = null;
        }
        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            update_list.ForEach((m) => { m.Update(); });
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
