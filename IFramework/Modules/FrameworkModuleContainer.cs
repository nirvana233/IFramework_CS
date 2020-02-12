using System;
using System.Collections.Generic;

namespace IFramework.Modules
{
    public class FrameworkModuleContainer : IFrameworkModuleContaner
    {
        private string _chunck;
        private bool _binded;
        private FrameworkEnvironment _env;

        public string chunck { get { return _chunck; } }
        public FrameworkEnvironment env { get { return _env; } }
        public bool binded { get { return _binded; } }

        private Dictionary<Type, List<FrameworkModule>> moudle_dic;
        private List<FrameworkModule> moudle_list;

        public event Action<Type, string> onModuleNotExist;


        public FrameworkModule CreateModule(Type type,string name="")
        {
            var mou= FrameworkModule.CreatInstance(type, chunck,name);
            mou.Bind(this);
            return mou;
        }
        public T CreateModule<T>(string name="") where T : FrameworkModule
        {
            return CreateModule(typeof(T),name) as T;
        }

        public FrameworkModule this[Type type, string name]
        {
            get { return FindModule(type, name); }
        }
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
        public T FindModule<T>(string name="") where T : FrameworkModule
        {
            return FindModule(typeof(T), name) as T;
        }


        public FrameworkModuleContainer(string chunck,FrameworkEnvironment env,bool bind=true)
        {
            _chunck = chunck;
            this._env = env;
            moudle_list = new List<FrameworkModule>();
            moudle_dic = new Dictionary<Type, List<FrameworkModule>>();
            if (bind)
                BindEnv();
        }
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

        public void Dispose()
        {
            UnBindEnv(false);
            for (int i = 0; i < moudle_list.Count; i++)
            {
                var m = moudle_list[i];
                m.Dispose();
            }
            moudle_list.Clear();
            moudle_dic.Clear();
            moudle_list = null;
            moudle_dic = null;
        }
        public void Update()
        {
            moudle_list.ForEach((m) => { m.Update(); });
        }

        internal bool AddModule(FrameworkModule moudle)
        {
            Type type = moudle.GetType();
            if (!moudle_dic.ContainsKey(type))
                moudle_dic.Add(type, new List<FrameworkModule>());
            var list = moudle_dic[type];
            var tmpModule = list.Find((m) => { return moudle.name == m.name; });
            if (tmpModule == null)
            {
                list.Add(moudle);
                moudle_list.Add(moudle);
                return true;
            }
            else
            {
                Log.E(string.Format("Have Bind Module | Type {0}  Name {1}", type, moudle.name));
                return false;
            }
        }
        internal bool RemoveBindModule(FrameworkModule moudle)
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
                    moudle_list.Remove(moudle);
                    moudle_dic[type].Remove(moudle);
                    return true;
                }
            }
        }
    }

}
