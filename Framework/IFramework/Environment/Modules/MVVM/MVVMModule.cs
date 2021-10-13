﻿using IFramework.Modules.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework.Modules.MVVM
{
    /// <summary>
    /// MVVM 模块
    /// </summary>
    [ScriptVersion(56)]
    public class MVVMModule : UpdateFrameworkModule, IMVVMModule
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public override int priority { get { return 70; } }

        protected override void Awake()
        {
            _message = CreatInstance<MessageModule>("");
            _groupmap = new Dictionary<string, MVVMGroup>();
        }
        protected override void OnDispose()
        {
            var em = _groupmap.Values.ToList();
            em.ForEach((e) =>
            {
                e.Dispose();
            });
            _groupmap = null;
            (_message as FrameworkModule).Dispose();
            _message = null;
        }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        internal IMessageModule _message;
        private Dictionary<string, MVVMGroup> _groupmap;

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
                group.module = this;
                //  group.message = _message;
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
        /// <summary>
        /// 刷新
        /// </summary>
        protected override void OnUpdate()
        {
            (_message as UpdateFrameworkModule).Update();
        }
    }
}
