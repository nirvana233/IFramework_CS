using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework.MVVM
{
    /// <summary>
    /// MVVM 模块
    /// </summary>
    [ScriptVersion(56)]
    public class MVVMGroups : Unit
    {
        /// <summary>
        /// 注销
        /// </summary>
        protected override void OnDispose()
        {
            var em = _groupmap.Values.ToList();
            em.ForEach((e) =>
            {
                e.Dispose();
            });
            _groupmap = null;
        }

        private Dictionary<string, MVVMGroup> _groupmap=new Dictionary<string, MVVMGroup>();

        /// <summary>
        /// 查找组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MVVMGroup FindGroup(string name)
        {
            MVVMGroup _group;
            _groupmap.TryGetValue(name, out _group);
            return _group;
        }
        /// <summary>
        /// 注册一个 MVVM
        /// </summary>
        public void AddGroup(MVVMGroup group)
        {
            MVVMGroup _group = FindGroup(group.name);
            if (_group != null)
                throw new Exception("Have Add Group " + group.name);
            else
            {
                _groupmap.Add(group.name, group);
            }
        }
        /// <summary>
        /// 移除组
        /// </summary>
        /// <param name="name"></param>
        public void RemoveGroup(string name)
        {
            MVVMGroup _group = FindGroup(name);
            if (_group == null)
                throw new Exception("Have not Add Group " + name);
            else
            {
                _groupmap.Remove(name);
            }
        }
        /// <summary>
        /// 移除组
        /// </summary>
        /// <param name="group"></param>
        public void RemoveGroup(MVVMGroup group)
        {
            RemoveGroup(group.name);
        }
       
    }
}
