using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework.Modules.ECS
{
    /// <summary>
    /// 模仿Ecs结构
    /// </summary>
    [ScriptVersionAttribute(633)]
    [VersionUpdateAttribute(552,"IComponent 采用Array统一管理")]
    [VersionUpdateAttribute(633, "内部代码上锁")]
    public class ECSModule : UpdateModule, IECSModule
    {
        private Systems _systems;
        private Entitys _entitys;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override ModulePriority OnGetDefaulyPriority()
        {
            return ModulePriority.ECS;
        }
        protected override void Awake()
        {
            _systems = new Systems();
            _entitys = new Entitys(this);
        }
        protected override void OnDispose()
        {
            _systems.Dispose();
            _entitys.Dispose();
            _systems = null;
            _entitys = null;
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
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity CreateEntity<TEntity>() where TEntity : IEntity
        {
            TEntity entity = Activator.CreateInstance<TEntity>();
            SubscribeEntity(entity);
            return entity;
        }
        /// <summary>
        /// 注册实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public void SubscribeEntity<TEntity>(TEntity entity) where TEntity : IEntity
        {
            _entitys.SubscribeEntity(entity);
        }
        /// <summary>
        /// 解除注册实体
        /// </summary>
        /// <param name="entity"></param>
        public void UnSubscribeEntity(IEntity entity)
        {
            _entitys.UnSubscribeEntity(entity);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEntity> GetEntitys()
        {
            return _entitys.GetEntitys();
        }


        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IComponent AddComponent(IEntity entity, Type type)
        {
            return _entitys.AddComponent(entity, type);
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <param name="useSame"></param>
        /// <returns></returns>
        public IComponent AddComponent(IEntity entity, IComponent component, bool useSame)
        {
            return _entitys.AddComponent(entity, component, useSame);
        }
        /// <summary>
        /// 刷新组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <param name="component"></param>
        public void ReFreshComponent(IEntity entity, Type type, IComponent component)
        {
            _entitys.ReFreshComponent(entity, type, component);
        }
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IComponent GetComponent(IEntity entity, Type type)
        {
            return _entitys.GetComponent(entity, type);
        }
        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        public void RemoveComponent(IEntity entity, Type type)
        {
            _entitys.RemoveComponent(entity, type);
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

        private class Entitys : IDisposable
        {
            private class EntityContainer : IDisposable
            {
                private ECSModule _moudle;
                private Dictionary<Type, int> _components;
                public List<int> componetIndexs { get { return _components.Values.ToList(); } }
                public EntityContainer(ECSModule moudle)
                {
                    this._moudle = moudle;
                    _components = new Dictionary<Type, int>();
                }
                public void Dispose()
                {
                    _moudle = null;
                    _components.Clear();
                }

                public bool GetComponent(Type type, out int index)
                {
                    return _components.TryGetValue(type, out index);
                }
                public void AddComponet(Type type, int index)
                {
                    int _index;
                    if (GetComponent(type, out _index))
                    {
                        throw new Exception(string.Format("Have Exist Component Type : {0}", type));
                    }
                    else
                    {
                        _components.Add(type, index);
                    }
                }

                public void RemoveComponent(Type type, out int index)
                {
                    int _index;
                    if (GetComponent(type, out _index))
                    {
                        _components.Remove(type);
                        index = _index;
                    }
                    else
                    {
                        throw new Exception(string.Format("Not Exist Component Type : {0}", type));
                    }
                }

                public void FreshIndex(int old,int _new)
                {
                    Type type=default(Type);
                    bool find = false;
                    foreach (var item in _components.Keys)
                    {
                        if (_components[item]==old)
                        {
                            type = item;
                            find = true;
                            break;
                        }
                       
                    }
                    if (find)
                    {
                        _components[type] = _new;
                    }
                }
            }

            private ECSModule _moudle;
            private Dictionary<IEntity, EntityContainer> _entitys;
            private IComponent[] _components;
            private Dictionary<IComponent, int> _componentUseCount;

            private int _count;
            private int _capicity=32;

            private int AddComponent(IComponent component, bool useSame)
            {
                if (useSame)
                {
                    int _index;
                    if (FindComponent(component, out _index))
                    {
                        int count;
                        if (_componentUseCount.TryGetValue(component, out count))
                            _componentUseCount[component]++;

                        else
                            _componentUseCount.Add(component, 2);
                        return _index;
                    }
                }


                if (_count==_capicity)
                {
                    int curLen = _capicity;
                    _capicity *= 2;
                    IComponent[] newArry = new IComponent[_capicity];
                    Array.Copy(_components, newArry, curLen);
                    Array.Clear(_components, 0, curLen);
                    _components = newArry;
                }
                int index = _count;
                _components[index] = component;
                _count++;
                return index;
            }
            private void FreeComponentIndex(int empty)
            {
                IComponent component = _components[empty];
                int cnt;
                if (_componentUseCount.TryGetValue(component, out cnt))
                {
                    _componentUseCount[component] = --cnt;
                    if (cnt == 0)
                    {
                        _componentUseCount.Remove(component);
                        int last = --_count;
                        //
                        if (last == empty) return;

                        _components[empty] = _components[last];
                        foreach (var c in _entitys.Values)
                        {
                            c.FreshIndex(last, empty);
                        }
                    }
                }
                else
                {
                    int last = --_count;
                    _components[empty] = _components[last];
                    foreach (var c in _entitys.Values)
                    {
                        c.FreshIndex(last, empty);
                    }
                   
                }

               
            }
            private bool FindContainer(IEntity entity, out EntityContainer container)
            {
                return _entitys.TryGetValue(entity, out container);
            }
            private bool FindComponent(IComponent component, out int index)
            {
                for (int i = 0; i < _components.Length; i++)
                {
                    var c = _components[i];
                    if (c == component)
                    {
                        index = i;
                        return true;
                    }
                }
                index = -1;
                return false;
            }

            private LockParam _lock=new LockParam();


            public Entitys(ECSModule moudle)
            {
                using (new LockWait(ref _lock))
                {
                    this._moudle = moudle;
                    _entitys = new Dictionary<IEntity, EntityContainer>();
                    _componentUseCount = new Dictionary<IComponent, int>();

                    _components = new IComponent[_capicity];
                    _count = 0;
                }

            }

            public void Dispose()
            {
                using (new LockWait(ref _lock))
                {
                    var em = _entitys.Keys.ToList();
                    for (int i = 0; i < em.Count; i++)
                    {
                        var e = em[i];
                        e.mou = null;
                        e.Destory();
                    }
                    _componentUseCount.Clear();
                    _entitys.Clear();
                    _components = null;
                    _entitys = null;
                    _moudle = null;
                }

            }







            internal void SubscribeEntity(IEntity entity)
            {
                using (new LockWait(ref _lock))
                {
                    if (!_entitys.ContainsKey(entity))
                    {
                        entity.mou = _moudle;
                        _entitys.Add(entity, new EntityContainer(_moudle));
                    }
                }

            }
            internal void UnSubscribeEntity(IEntity entity)
            {
                using (new LockWait(ref _lock))
                {
                    EntityContainer container;
                    if (FindContainer(entity, out container))
                    {
                        entity.mou = null;
                        var indexs = container.componetIndexs;
                        for (int i = 0; i < indexs.Count; i++)
                        {
                            FreeComponentIndex(indexs[i]);

                        }
                        
                        container.Dispose();
                        _entitys.Remove(entity);
                    }
                    else
                    {
                        throw new Exception("Not Exist Entity");
                    }
                }
              
            }


            internal IComponent AddComponent(IEntity entity, Type type)
            {
                using (new LockWait(ref _lock))
                {
                    EntityContainer container;
                    if (FindContainer(entity, out container))
                    {
                        IComponent ic = _moudle.CreateComponent(type);
                        int index = AddComponent(ic, false);

                        container.AddComponet(type, index);
                        return ic;
                    }
                    else
                        throw new Exception("Not Exist Entity");
                }

            }
            internal IComponent AddComponent(IEntity entity, IComponent component, bool useSame)
            {
                using (new LockWait(ref _lock))
                {
                    EntityContainer container;
                    if (FindContainer(entity, out container))
                    {
                        Type type = component.GetType();
                        int index = AddComponent(component, useSame);
                        container.AddComponet(type, index);
                        return component;
                    }
                    else
                    {
                        throw new Exception("Not Exist Entity");
                    }
                }
               
            }

            internal IComponent GetComponent(IEntity entity, Type type)
            {
                using (new LockWait(ref _lock))
                {
                    EntityContainer container;
                    if (FindContainer(entity, out container))
                    {
                        int index;
                        if (container.GetComponent(type, out index))
                        {
                            return _components[index];
                        }
                        else
                            return default(IComponent);
                    }
                    else
                    {
                        throw new Exception("Not Exist Entity");
                    }
                }
                
            }
            internal void RemoveComponent(IEntity entity, Type type)
            {
                using (new LockWait(ref _lock))
                {
                    EntityContainer container;
                    if (FindContainer(entity, out container))
                    {
                        int index;
                        container.RemoveComponent(type, out index);
                        FreeComponentIndex(index);
                    }
                    else
                    {
                        throw new Exception("Not Exist Entity");
                    }
                }
              
            }

            internal IEnumerable<IEntity> GetEntitys()
            {
                using (new LockWait(ref _lock))
                {
                    return _entitys.Keys;

                }
            }
            internal void ReFreshComponent(IEntity entity, Type type, IComponent component)
            {
                using (new LockWait(ref _lock))
                {
                    EntityContainer container;
                    if (FindContainer(entity, out container))
                    {
                        int index;
                        if (container.GetComponent(type, out index))
                        {
                            _components[index] = component;
                        }
                        else
                        {
                            throw new Exception(string.Format("The Entity {2} Componet {0} of Type {1} Not Exist", component, type, entity));
                        }
                    }
                    else
                    {
                        throw new Exception("Not Exist Entity");
                    }

                }
               
            }
        }

        private class Systems : IDisposable
        {
            private List<IExcuteSystem> _systems;
            private LockParam _lock=new LockParam();

            public Systems()
            {
                using (new LockWait(ref _lock))
                {
                    _systems = new List<IExcuteSystem>();
                }
            }
            private bool _dispose;
            public void Dispose()
            {
                using (new LockWait(ref _lock))
                {
                    _dispose = true;
                    for (int i = 0; i < _systems.Count; i++)
                    {
                        _systems[i].OnModuleDispose();
                    }
                    _systems.Clear();
                    _systems = null;
                }

            }

            internal void Update()
            {
                using (new LockWait(ref _lock))
                {
                    if (_dispose) return;
                    _systems.ForEach((sys) => { sys.Excute(); });
                }
            }

            internal void AddSystem(IExcuteSystem system)
            {
                using (new LockWait(ref _lock))
                {
                    if (!_systems.Contains(system))
                        _systems.Add(system);
                }
            }
            internal void RemoveSystem(IExcuteSystem system)
            {
                using (new LockWait(ref _lock))
                {
                    if (_systems.Contains(system))
                        _systems.Remove(system);
                }

            }
        }

    }
}
