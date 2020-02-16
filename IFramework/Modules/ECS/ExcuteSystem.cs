using System.Collections.Generic;

namespace IFramework.Modules.ECS
{
    /// <summary>
    /// 处理系统
    /// </summary>
    /// <typeparam name="TEnity"></typeparam>
    public abstract class ExcuteSystem<TEnity> : IExcuteSystem where TEnity : Enity
    {
        internal ECSModule _moudule { get; }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="module"></param>
        protected ExcuteSystem(ECSModule module)
        {
            _moudule = module;
            _moduleDispose = false;
        }
        /// <summary>
        /// 获取所有实体（模块内）
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<Enity> GetEnitys()
        {
            return _moudule.GetEnitys();
        }
        private bool _moduleDispose;
        /// <summary>
        /// 适配
        /// </summary>
        /// <param name="enity"></param>
        /// <returns></returns>
        protected virtual bool Fitter(TEnity enity)
        {
            return true;
        }
        /// <summary>
        /// 操作合法实体
        /// </summary>
        /// <param name="enity"></param>
        protected abstract void Excute(TEnity enity);
        /// <summary>
        /// 模块释放时
        /// </summary>
        protected virtual void OnModuleDispose() { }

        void IExcuteSystem.Excute()
        {
            if (_moduleDispose) return;
            var _enitys = GetEnitys().GetEnumerator();
            while (_enitys.MoveNext())
            {
                var current = _enitys.Current as TEnity;
                if (current == null) continue;
                if (Fitter(current))
                    Excute(current);
            }
        }
        void IExcuteSystem.OnModuleDispose()
        {
            _moduleDispose = true;
            OnModuleDispose();
        }
    }

}
