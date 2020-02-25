using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework.Modules.ECS
{
    /// <summary>
    /// 模仿Ecs结构
    /// </summary>
    [FrameworkVersion(26)]
    public class ECSModule : FrameworkModule
    {
        private Systems _systems;
        private Enitys _enitys;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override void Awake()
        {
            _systems = new Systems();
            _enitys = new Enitys(this);
        }
        protected override void OnDispose()
        {
            _systems.Dispose();
            _enitys.Dispose();
            _systems = null;
            _enitys = null;
        }
        protected override void OnUpdate()
        {
            _systems.Update();
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释


        private IComponent CreateComponent(Type type)
        {
            return Activator.CreateInstance(type) as IComponent;
        }
        /// <summary>
        /// 创建实体，创建完，注册
        /// </summary>
        /// <typeparam name="TEnity"></typeparam>
        /// <returns></returns>
        public TEnity CreateEnity<TEnity>() where TEnity : IEnity
        {
            TEnity enity = Activator.CreateInstance<TEnity>();
            SubscribeEnity(enity);
            return enity;
        }
        /// <summary>
        /// 注册实体
        /// </summary>
        /// <typeparam name="TEnity"></typeparam>
        /// <param name="enity"></param>
        public void SubscribeEnity<TEnity>(TEnity enity) where TEnity : IEnity
        {
            _enitys.AddEnity(enity);
            enity._mou = this;
        }
        /// <summary>
        /// 解除注册实体
        /// </summary>
        /// <param name="enity"></param>
        public void UnSubscribeEnity(IEnity enity)
        {
            enity._mou = null;
            _enitys.UnSubscribeEnity(enity);
        }
        /// <summary>
        /// 注册系统
        /// </summary>
        /// <param name="system"></param>
        public void SubscribeSystem(IExcuteSystem system)
        {
            _systems.AddSystem(system);
        }
        /// <summary>
        /// 解除注册系统
        /// </summary>
        /// <param name="system"></param>
        public void UnSubscribeSystem(IExcuteSystem system)
        {
            _systems.RemoveSystem(system);
        }


        internal IComponent AddComponent(IEnity enity, IComponent component)
        {
            return _enitys.AddComponent(enity, component);
        }

        internal void ReFreshComponent(IEnity enity, Type type, IComponent component)
        {
            _enitys.ReFreshComponent(enity, type, component);
        }

        internal IComponent AddComponent(IEnity enity, Type type)
        {
            return _enitys.AddComponent(enity, type);
        }
        internal IComponent GetComponent(IEnity enity, Type type)
        {
            return _enitys.GetComponent(enity, type);
        }
        internal void RemoveComponent(IEnity enity, Type type)
        {
            _enitys.RemoveComponent(enity, type);
        }
        internal IEnumerable<IEnity> GetEnitys()
        {
            return _enitys.GetEnitys();
        }

        private class Enitys : IDisposable
        {
            private class EnityComponents : IDisposable
            {
                private ECSModule _moudle;
                private Dictionary<Type, IComponent> _components;

                public EnityComponents(ECSModule moudle)
                {
                    this._moudle = moudle;
                    _components = new Dictionary<Type, IComponent>();
                }
                public void Dispose()
                {
                    _moudle = null;
                    _components.Clear();
                }

                public IComponent GetComponent(Type type)
                {
                    IComponent comp;
                    _components.TryGetValue(type, out comp);
                    return comp;
                }
                public IComponent AddComponet(Type type)
                {
                    IComponent comp = GetComponent(type);
                    if (comp != null) throw new Exception(string.Format("Have Exist Component Type : {0}", type));
                    comp = _moudle.CreateComponent(type);
                    _components.Add(type, comp);
                    return comp;
                }
                public IComponent AddComponet(IComponent component)
                {
                    Type type = component.GetType();
                    IComponent comp = GetComponent(type);
                    if (comp != null) throw new Exception(string.Format("Have Exist Component Type : {0}", type));
                    comp = component;
                    _components.Add(type, comp);
                    return comp;
                }

                public void RemoveComponent(Type type)
                {
                    IComponent comp = GetComponent(type);
                    if (comp == null) throw new Exception(string.Format("Not Exist Component Type : {0}", type));
                    _components.Remove(type);
                }

                internal void ReFreshComponent(Type type, IComponent component)
                {
                    IComponent comp = GetComponent(type);
                    if (comp == null) throw new Exception(string.Format("Not Exist Component Type : {0}", type));
                    _components[type] = component;
                }
            }

            private ECSModule _moudle;
            private Dictionary<IEnity, EnityComponents> _enitys;

            public Enitys(ECSModule moudle)
            {
                this._moudle = moudle;
                _enitys = new Dictionary<IEnity, EnityComponents>();
            }
            public void Dispose()
            {
                var em = _enitys.Keys.ToList();
                //   Log.E("dispose  " + GetType());

                em.ForEach((e) =>
                {
                    e.Destory();
                });

                _enitys.Clear();
                _enitys = null;
                _moudle = null;
            }

            public void AddEnity(IEnity enity)
            {
                if (!_enitys.ContainsKey(enity))
                    _enitys.Add(enity, new EnityComponents(_moudle));
            }
            internal void UnSubscribeEnity(IEnity enity)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                comp.Dispose();
                _enitys.Remove(enity);
            }


            private EnityComponents FindComponent(IEnity enity)
            {
                EnityComponents comp;
                _enitys.TryGetValue(enity, out comp);
                return comp;
            }

            internal IComponent GetComponent(IEnity enity, Type type)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                return comp.GetComponent(type);
            }
            internal IComponent AddComponent(IEnity enity, Type type)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                return comp.AddComponet(type);
            }
            internal IComponent AddComponent(IEnity enity, IComponent component)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                return comp.AddComponet(component);
            }


            internal void RemoveComponent(IEnity enity, Type type)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                comp.RemoveComponent(type);
            }

            internal IEnumerable<IEnity> GetEnitys()
            {
                return _enitys.Keys;
            }

            internal void ReFreshComponent(IEnity enity, Type type, IComponent component)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                comp.ReFreshComponent(type, component);
            }
        }

        private class Systems : IDisposable
        {
            private List<IExcuteSystem> _systems;
            public Systems()
            {
                _systems = new List<IExcuteSystem>();
            }

            public void Dispose()
            {
                _systems.ForEach((sys) => { sys.OnModuleDispose(); });
                _systems.Clear();
                _systems = null;
            }

            internal void Update()
            {
                _systems.ForEach((sys) => { sys.Excute(); });
            }

            internal void AddSystem(IExcuteSystem system)
            {
                if (!_systems.Contains(system))
                    _systems.Add(system);
            }
            internal void RemoveSystem(IExcuteSystem system)
            {
                if (_systems.Contains(system))
                    _systems.Remove(system);
            }
        }

    }
}
