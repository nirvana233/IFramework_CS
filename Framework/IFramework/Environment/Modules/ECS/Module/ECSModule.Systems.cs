using System;
using System.Collections.Generic;

namespace IFramework.Modules.ECS
{
    public partial class ECSModule
    {
        private class Systems : IDisposable
        {
            private List<IExcuteSystem> _systems;
            private LockParam _lock = new LockParam();

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
