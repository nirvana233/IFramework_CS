using System;

namespace IFramework.Modules.ECS
{
    public abstract class Enity
    {
        internal ECSModule mou;
        public IComponent GetComponent(Type type)
        {
            return mou.GetComponent(this, type);
        }
        public T GetComponent<T>() where T : IComponent
        {
            return (T)GetComponent(typeof(T));
        }

        public IComponent AddComponent(Type type)
        {
            return mou.AddComponent(this, type);
        }
        public T AddComponent<T>() where T : IComponent
        {
            return (T)AddComponent(typeof(T));
        }
        public IComponent AddComponent(IComponent component)
        {
            return mou.AddComponent(this, component);
        }
        public T AddComponent<T>(T component) where T : IComponent
        {
            return AddComponent(component);
        }

        public void RemoveComponent(Type type)
        {
            mou.RemoveComponent(this, type);
        }
        public void RemoveComponent<T>() where T : IComponent
        {
            RemoveComponent(typeof(T));
        }

        public bool ContainsComponent(Type type)
        {
            return mou.GetComponent(this, type) != null;
        }
        public bool ContainsComponent<T>() where T : IComponent
        {
            return ContainsComponent(typeof(T));
        }

        public void ReFreshComponent(Type type ,IComponent component)
        {
            mou.ReFreshComponent(this, type, component);
        }
        public void ReFreshComponent<T>(T component)where T:IComponent
        {
            ReFreshComponent(typeof(T), component);
        }
        public void Destory()
        {
            mou.Destory(this);
        }
        public virtual void OnDestory() { }
    }

}
