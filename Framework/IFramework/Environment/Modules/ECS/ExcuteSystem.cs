using System.Collections.Generic;

namespace IFramework.Modules.ECS
{
    /// <summary>
    /// 处理系统
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ExcuteSystem<TEntity> : IExcuteSystem where TEntity : IEntity
    {
        internal IECSModule _moudule { get; }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="module"></param>
        protected ExcuteSystem(IECSModule module)
        {
            _moudule = module;
            _moduleDispose = false;
        }
        /// <summary>
        /// 获取所有实体（模块内）
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IEntity> GetEntitys()
        {
            return _moudule.GetEntitys();
        }
        private bool _moduleDispose;
        /// <summary>
        /// 适配
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual bool Fitter(TEntity entity)
        {
            return true;
        }
        /// <summary>
        /// 操作合法实体
        /// </summary>
        /// <param name="entity"></param>
        protected abstract void Excute(TEntity entity);
        /// <summary>
        /// 模块释放时
        /// </summary>
        protected virtual void OnModuleDispose() { }

        void ICommand.Excute()
        {
            if (_moduleDispose) return;
            var _Entitys = GetEntitys().GetEnumerator();
            while (_Entitys.MoveNext())
            {
                var current = (TEntity)_Entitys.Current;
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
