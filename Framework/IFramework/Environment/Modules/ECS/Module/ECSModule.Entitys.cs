using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework.Modules.ECS
{
    public partial class ECSModule
    {
        private partial class Entitys : IDisposable
        {
            private ECSModule _moudle;
            private Dictionary<IEntity, EntityContainer> _entitys = Framework.GlobalAllocate<Dictionary<IEntity, EntityContainer>>();
            private Dictionary<IComponent, int> _componentUseCount = Framework.GlobalAllocate<Dictionary<IComponent, int>>();
            private IComponent[] _components;
            private LockParam _lock = new LockParam();

            private int _count = 0;
            private int _capicity = 32;

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
                            //2:因为不重复使用的IComponent不记录使用次数
                            _componentUseCount.Add(component, 2);
                        return _index;
                    }
                }


                if (_count == _capicity)
                {
                    int curLen = _capicity;
                    _capicity *= 2;

                    IComponent[] newArry = Framework.GlobalAllocateArray<IComponent>(_capicity);
                    Array.Copy(_components, newArry, curLen);
                    Array.Clear(_components, 0, curLen);
                    _components.GlobalRecyle();
                    _components = newArry;
                }
                int index = _count;
                _components[index] = component;
                _count++;
                return index;
            }

            private void ReleaseComponent(int index)
            {
                IComponent component = _components[index];
                int cnt;
                bool release = true;
                //因为不重复使用的IComponent不记录使用次数，所以有可能找不到
                if (_componentUseCount.TryGetValue(component, out cnt))
                {
                    _componentUseCount[component] = --cnt;
                    release = cnt <= 0;
                    if (cnt == 0) _componentUseCount.Remove(component);
                }
                if (release)
                {
                    --_count;
                    if (_count != index) return;
                    _components[index] = _components[_count];
                    Type type = _components[index].GetType();
                    foreach (var c in _entitys.Values)
                    {
                        c.FreshComponentIndex(type, _count, index);
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



            public Entitys(ECSModule moudle)
            {
                using (new LockWait(ref _lock))
                {
                    this._moudle = moudle;
                    _components = Framework.GlobalAllocateArray<IComponent>(_capicity);
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
                    foreach (var item in _entitys.Values)
                    {
                        item.Dispose();
                        item.GlobalRecyle();
                    }
                    _entitys.Clear();
                    Array.Clear(_components, 0, _components.Length);
                    _componentUseCount.GlobalRecyle();
                    _components.GlobalRecyle();
                    _entitys.GlobalRecyle();
                }
            }







            public void SubscribeEntity(IEntity entity)
            {
                using (new LockWait(ref _lock))
                {
                    if (!_entitys.ContainsKey(entity))
                    {
                        entity.mou = _moudle;
                        EntityContainer c = Framework.GlobalAllocate<EntityContainer>();
                        _entitys.Add(entity, c);
                    }
                }

            }
            public void UnSubscribeEntity(IEntity entity)
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
                            ReleaseComponent(indexs[i]);
                        }
                        container.Dispose();
                        container.GlobalRecyle();
                        _entitys.Remove(entity);
                    }
                    else
                    {
                        throw new Exception("Not Exist Entity");
                    }
                }

            }


            public IComponent AddComponent(IEntity entity, Type type)
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
            public IComponent AddComponent(IEntity entity, IComponent component, bool useSame)
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

            public IComponent GetComponent(IEntity entity, Type type)
            {
                using (new LockWait(ref _lock))
                {
                    EntityContainer container;
                    if (FindContainer(entity, out container))
                    {
                        int index;
                        if (container.GetComponent(type, out index))
                            return _components[index];
                        else
                            return default(IComponent);
                    }
                    else
                    {
                        throw new Exception("Not Exist Entity");
                    }
                }

            }
            public void RemoveComponent(IEntity entity, Type type)
            {
                using (new LockWait(ref _lock))
                {
                    EntityContainer container;
                    if (FindContainer(entity, out container))
                    {
                        int index;
                        container.RemoveComponent(type, out index);
                        ReleaseComponent(index);
                    }
                    else
                    {
                        throw new Exception("Not Exist Entity");
                    }
                }

            }

            public IEnumerable<IEntity> GetEntitys()
            {
                using (new LockWait(ref _lock))
                {
                    return _entitys.Keys;
                }
            }
            public void ReFreshComponent(IEntity entity, Type type, IComponent component)
            {
                using (new LockWait(ref _lock))
                {
                    EntityContainer container;
                    if (FindContainer(entity, out container))
                    {
                        int index;
                        if (container.GetComponent(type, out index))
                            _components[index] = component;
                        else
                            throw new Exception(string.Format("The Entity {2} Componet {0} of Type {1} Not Exist", component, type, entity));
                    }
                    else
                    {
                        throw new Exception("Not Exist Entity");
                    }
                }

            }
        }

    }
}
