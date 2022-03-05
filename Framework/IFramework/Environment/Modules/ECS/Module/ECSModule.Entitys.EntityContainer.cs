using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework.Modules.ECS
{
    public partial class ECSModule
    {
        private partial class Entitys
        {
            private class EntityContainer : IDisposable
            {
                private Dictionary<Type, int> _components;
                public List<int> componetIndexs { get { return _components.Values.ToList(); } }
                public EntityContainer()
                {
                    _components = new Dictionary<Type, int>();
                }
                public void Dispose()
                {
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

                public void FreshComponentIndex(Type type,int old, int _new)
                {
                    int _index;
                    if (GetComponent(type, out _index))
                    {
                        _components.Remove(type);
                        if (_index != old)
                        {
                            throw new Exception(string.Format(" Component Index Err Type : {0}", type));
                        }
                        _components[type] = _new;
                    }
                }
            }
        }

    }
}
