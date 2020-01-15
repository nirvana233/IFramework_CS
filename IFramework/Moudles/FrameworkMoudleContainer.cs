using System;
using System.Collections.Generic;

namespace IFramework.Moudles
{
    public class FrameworkMoudleContainer : IFrameworkMoudleContaner
    {
        private string _chunck;
        public string chunck { get { return _chunck; } }

        public bool binded { get { return _binded; } }
        private bool _binded;

        private Dictionary<Type, List<FrameworkMoudle>> moudle_dic;
        private List<FrameworkMoudle> moudle_list;

        public event Action<Type, string> onMoudleNotExist;


        public FrameworkMoudle CreateMoudle(Type type)
        {
            var mou= FrameworkMoudle.CreatInstance(type, chunck);
            mou.Bind(this);
            return mou;
        }
        public T CreateMoudle<T>() where T : FrameworkMoudle
        {
            return CreateMoudle(typeof(T)) as T;
        }

        public FrameworkMoudle this[Type type, string name]
        {
            get { return FindMoudle(type, name); }
        }
        public FrameworkMoudle FindMoudle(Type type, string name)
        {
            FrameworkMoudle mou = default(FrameworkMoudle);
            if (moudle_dic.ContainsKey(type))
                mou = moudle_dic[type].Find((m) => { return m.name == name; });
            if (mou == null)
                if (onMoudleNotExist != null)
                {
                    onMoudleNotExist(type, name);
                    if (moudle_dic.ContainsKey(type))
                        mou = moudle_dic[type].Find((m) => { return m.name == name; });
                }
            return mou;
        }
        public T FindMoudle<T>(string name) where T : FrameworkMoudle
        {
            return FindMoudle(typeof(T), name) as T;
        }


        public FrameworkMoudleContainer(string chunck,bool bind=true)
        {
            _chunck = chunck;
            moudle_list = new List<FrameworkMoudle>();
            moudle_dic = new Dictionary<Type, List<FrameworkMoudle>>();
            if (bind)
                BindFramework();


        }
        public void BindFramework()
        {
            if (_binded)
            {
                Log.E(string.Format("Have Bind Container Type : {0} chunck : {1}", GetType(), chunck));
                return;
            }
            _binded = true;
            Framework.update += Update;
            Framework.onDispose += Dispose;
        }
        public void UnBindFramework(bool dispose=true)
        {
            if (_binded)
            {
                _binded = false;
                Framework.update += Update;
                Framework.onDispose += Dispose;
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
            UnBindFramework(false);
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

        internal void AddMoudle(FrameworkMoudle moudle)
        {
            Type type = moudle.GetType();
            if (!moudle_dic.ContainsKey(type))
                moudle_dic.Add(type, new List<FrameworkMoudle>());
            var list = moudle_dic[type];
            var tmpMoudle = list.Find((m) => { return moudle.name == m.name; });
            if (tmpMoudle == null)
            {
                list.Add(moudle);
                moudle_list.Add(moudle);
            }
            else
                Log.E(string.Format("Have Bind Moudle | Type {0}  Name {1}", type, moudle.name));
        }
        internal void RemoveBindMoudle(FrameworkMoudle moudle)
        {
            Type type = moudle.GetType();
            if (!moudle_dic.ContainsKey(type))
                Log.E(string.Format("01,Have Not Bind Moudle | Type {0}  Name {1}", type, moudle.name));
            else
            {
                var tmpMoudle = moudle_dic[type].Find((m) => { return moudle.name == m.name; });
                if (tmpMoudle == null)
                    Log.E(string.Format("02,Have Not Bind Moudle | Type {0}  Name {1}", type, moudle.name));
                else
                {
                    moudle_list.Remove(moudle);
                    moudle_dic[type].Remove(moudle);
                }
            }
        }
    }

}
