using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFramework.Modules.Timer
{
    public enum TaskState
    {
        Running, Stoped, Paused, None
    }
    public interface ITimerModule
    {
        string RunTask(double space, Action action, Action onCompeleted, double delay, int count);
        void StopTask(string id);
        void PauseTask(string id);
        void UnPauseTask(string id);
        TaskState GetTaskState(string id);
    }
    public class TimerModule : FrameworkModule, ITimerModule
    {

        private class FrameTask
        {
            public string id;
            public bool Paused { get; private set; }
            public Action Finish;
            public bool NeedSet { get; private set; }
            public void Pause() { Paused = true; }
            public void UnPause() { Paused = false; }


            private double delay;
            private int count;
            private double space;
            private Action action;

            private double startTime;
            private bool AwalysRun;
            public FrameTask SetVal(double space, Action action, Action Finish, double delay, int count)
            {
                NeedSet = false;
                this.id = Guid.NewGuid().ToString();
                this.space = space;
                this.action = action;
                this.count = count;
                this.delay = delay;
                AwalysRun = count < 0;
                startTime = GetUtcFrame(DateTime.UtcNow);
                this.Finish = Finish;
                return this;
            }

            private double GetUtcFrame(DateTime time)
            {
                DateTime timeZerro = new DateTime(1970, 1, 1);
                return (time - timeZerro).TotalMilliseconds;
            }

            private DateTime LastTime;
            private DateTime CurTime;
            private double helpFrame;
            public void Tick()
            {
                if (Paused) return;
                CurTime = DateTime.UtcNow;
                if (GetUtcFrame(CurTime) - startTime < delay) return;
                helpFrame -= (CurTime - LastTime).TotalMilliseconds;
                if (helpFrame <= 0)
                {
                    count--;
                    helpFrame = space;
                    if (count <= 0 && !AwalysRun) NeedSet = true;
                    if (action != null && !NeedSet) action();
                }
                LastTime = CurTime;
            }
        }
        private class Timer:IDisposable
        {
            private class TimeTaskPool : ObjectPool<FrameTask>
            {
                protected override FrameTask CreatNew(IEventArgs arg, params object[] param)
                {
                    return new FrameTask();
                }
            }
            private TimeTaskPool pool;
            private Dictionary<string,FrameTask> Tasks;
            private LockParam param;
            public Timer() { pool = new TimeTaskPool(); Tasks = new  Dictionary<string, FrameTask>(); param = new LockParam(); }
            public void Tick()
            {
                using (new LockWait(ref param))
                {
                    List<FrameTask> setOnes = new List<FrameTask>();
                    foreach (var item in Tasks)
                    {
                        item.Value.Tick();
                        if (item.Value.NeedSet)
                        {
                            setOnes.Add(item.Value);
                        }
                    }
                    if (setOnes != null)
                    {
                        setOnes.ForEach((setOne) => {
                            if (setOne.Finish != null)
                                setOne.Finish();
                            pool.Set(setOne);
                            Tasks.Remove(setOne.id);
                        });
                    }
                }
            }

            public string RunTimerTask(double space, Action action, Action finish, double delay, int count)
            {
                using (new LockWait(ref param))
                {
                    var task = pool.Get().SetVal(space, action, finish, delay, count);
                    Tasks.Add(task.id,task);
                    return task.id;
                }
            }
            public void StopTimerTask(string id)
            {
                using (new LockWait(ref param))
                {
                    if (!Tasks.ContainsKey(id)) return;

                    var task = Tasks[id];
                    if (task != null)
                    {
                        if (task.Finish != null)
                            task.Finish();
                        Tasks.Remove(task.id);
                        pool.Set(task);
                    }
                }
            }

            public TaskState GetTaskState(string id)
            {
                using (new LockWait(ref param))
                {
                    if (pool.Contains((task) => { return task.id == id; }))
                        return TaskState.Stoped;
                    if (Tasks.ContainsKey(id))
                    {
                        var ta = Tasks[id];
                        if (ta != null) return ta.Paused ? TaskState.Paused : TaskState.Running;
                    }
                    return TaskState.None;
                }
            }
            public void PauseTimerTask(string id)
            {
                using (new LockWait(ref param))
                {
                    if (Tasks.ContainsKey(id))
                    {
                        var ta = Tasks[id];
                        if (ta != null) ta.Pause();
                    }                   
                }
            }
            public void UnPauseTimerTask(string id)
            {
                using (new LockWait(ref param))
                {
                    if (Tasks.ContainsKey(id))
                    {
                        var ta = Tasks[id];
                        if (ta != null) ta.UnPause();
                    }
                }
            }

            public void Dispose()
            {
                pool.Dispose();
                Tasks.Clear();
            }
        }
        protected override bool needUpdate { get { return true; } }

        private Timer timer;

        public string RunTask(double space, Action action, Action onCompeleted, double delay, int count)
        {
            return timer.RunTimerTask(space, action, onCompeleted, delay, count);
        }

        public void StopTask(string id)
        {
            timer.StopTimerTask(id);
        }
        public void PauseTask(string id)
        {
            timer.PauseTimerTask(id);
        }
        public void UnPauseTask(string id)
        {
            timer.UnPauseTimerTask(id);
        }
        public TaskState GetTaskState(string id)
        {
            return timer.GetTaskState(id);
        }

        protected override void OnUpdate()
        {
            timer.Tick();

        }
        protected override void OnDispose()
        {
            timer.Dispose();
            timer = null;
        }
        protected override void Awake()
        {
            timer = new Timer();
        }
    }
}
