using IFramework.Pool;
using System;
using System.Collections.Generic;

namespace IFramework.Modules
{
    /// <summary>
    /// 时间任务状态
    /// </summary>
    public enum TaskState
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        Running, Stoped, Paused, None
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
    /// <summary>
    /// 计时器模块
    /// </summary>
    public class TimerModule : UpdateFrameworkModule
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
            private class TimeTaskPool : ListPool<FrameTask>
            {
                protected override FrameTask CreatNew(IEventArgs arg)
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

        private Timer timer;
        /// <summary>
        /// 运行一个时间任务
        /// </summary>
        /// <param name="space"></param>
        /// <param name="action"></param>
        /// <param name="onCompeleted"></param>
        /// <param name="delay"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string RunTask(double space, Action action, Action onCompeleted, double delay, int count)
        {
            return timer.RunTimerTask(space, action, onCompeleted, delay, count);
        }
        /// <summary>
        /// 结束一个时间任务
        /// </summary>
        /// <param name="id"></param>
        public void StopTask(string id)
        {
            timer.StopTimerTask(id);
        }
        /// <summary>
        /// 暂停一个时间任务
        /// </summary>
        /// <param name="id"></param>
        public void PauseTask(string id)
        {
            timer.PauseTimerTask(id);
        }
        /// <summary>
        /// 重启一个任务
        /// </summary>
        /// <param name="id"></param>
        public void UnPauseTask(string id)
        {
            timer.UnPauseTimerTask(id);
        }
        /// <summary>
        /// 获取任务的状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TaskState GetTaskState(string id)
        {
            return timer.GetTaskState(id);
        }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
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
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
}
