using System.Collections.Generic;

namespace IFramework.Modules.ECS
{
    public abstract class ExcuteSystem<TEnity> : IExcuteSystem where TEnity : Enity
    {
        protected ECSModule _moudule { get; }
        protected ExcuteSystem(ECSModule module)
        {
            _moudule = module;
        }
        protected IEnumerable<Enity> GetEnitys()
        {
            return _moudule.GetEnitys();
        }

        public void Excute()
        {
            var _enitys = GetEnitys().GetEnumerator();
            while (_enitys.MoveNext())
            {
                var current = _enitys.Current as TEnity;
                if (current == null) continue;
                if (Fitter(current))
                    Excute(current);
            }
        }
        protected virtual bool Fitter(TEnity enity)
        {
            return true;
        }
        protected abstract void Excute(TEnity enity);
    }

}
