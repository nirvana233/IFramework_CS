using System;
using System.Collections.Generic;

namespace IFramework.Modules.ECS
{
    [FrameworkVersion(10)]
    public class ECSModule : FrameworkModule
    {
        protected override bool needUpdate { get { return true; } }
        private Systems _systems;
        private Enitys _enitys;
        
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


        private IComponent CreateComponent(Type type)
        {
            return Activator.CreateInstance(type) as IComponent;
        }
        public TEnity CreateEnity<TEnity>()where TEnity :Enity
        {
            TEnity enity = Activator.CreateInstance<TEnity>();
            SubscribeEnity(enity);
            return enity;
        }
        public void SubscribeEnity<TEnity>( TEnity enity) where TEnity : Enity
        {
            _enitys.AddEnity(enity);
            enity.mou = this;
        }

        public void AddSystem(IExcuteSystem system)
        {
            _systems.AddSystem(system);
        }
        public void RemoveSystem(IExcuteSystem system)
        {
            _systems.RemoveSystem(system);
        }


        internal IComponent AddComponent(Enity enity, IComponent component)
        {
            return _enitys.AddComponent(enity, component);
        }

        internal void ReFreshComponent(Enity enity, Type type, IComponent component)
        {
            _enitys.ReFreshComponent(enity, type, component);
        }

        internal IComponent AddComponent(Enity enity, Type type)
        {
            return _enitys.AddComponent(enity, type);
        }
        internal IComponent GetComponent(Enity enity, Type type)
        {
            return _enitys.GetComponent(enity, type);
        }
        internal void RemoveComponent(Enity enity, Type type)
        {
            _enitys.RemoveComponent(enity, type);
        }
        internal void Destory(Enity enity)
        {
            _enitys.Destory(enity);
        }
        internal IEnumerable<Enity> GetEnitys()
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
                    IComponent comp= GetComponent(type);
                    if (comp != null) throw new Exception(string .Format("Have Exist Component Type : {0}",type));
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
            private Dictionary<Enity, EnityComponents> _enitys;

            public Enitys(ECSModule moudle)
            {
                this._moudle = moudle;
                _enitys = new Dictionary<Enity, EnityComponents>();
            }
            public void Dispose()
            {
                foreach (var item in _enitys.Values)
                {
                    item.Dispose();
                }
                _enitys.Clear();
                _enitys = null;
                _moudle = null;
            }

            public void AddEnity(Enity enity)
            {
                if (!_enitys.ContainsKey(enity))
                    _enitys.Add(enity, new EnityComponents(_moudle));
            }
            internal void Destory(Enity enity)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                comp.Dispose();
                _enitys.Remove(enity);
            }


            private EnityComponents FindComponent(Enity enity)
            {
                EnityComponents comp;
                _enitys.TryGetValue(enity, out comp);
                return comp;
            }

            internal IComponent GetComponent(Enity enity, Type type)
            {
                EnityComponents comp= FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                return comp.GetComponent(type);
            }
            internal IComponent AddComponent(Enity enity, Type type)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                return comp.AddComponet(type);
            }
            internal IComponent AddComponent(Enity enity, IComponent component)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                return comp.AddComponet(component);
            }


            internal void RemoveComponent(Enity enity, Type type)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                comp.RemoveComponent(type);
            }

            internal IEnumerable<Enity> GetEnitys()
            {
                return _enitys.Keys;
            }

            internal void ReFreshComponent(Enity enity, Type type, IComponent component)
            {
                EnityComponents comp = FindComponent(enity);
                if (comp == null) throw new Exception("Not Exist Enity");
                comp.ReFreshComponent(type,component);
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
