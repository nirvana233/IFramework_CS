using System;
using System.Collections.Generic;

namespace IFramework.Modules.Recorder
{
    /// <summary>
    /// 操作记录
    /// </summary>
    public class OperationRecorderModule : Module, IOperationRecorderModule
    {
        private class StatePool : BaseTypePool<BaseState> { }
        private class HeadState : BaseState
        {
            protected override void OnRedo() { }

            protected override void OnUndo() { }

            protected override void OnReset() { }
        }
        private HeadState _head;
        private StatePool _pool;
        //private List<BaseState> _states;
        private BaseState _current;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override ModulePriority OnGetDefaulyPriority()
        {
            return ModulePriority.Recorder;
        }
        protected override void Awake()
        {
            _head = new HeadState();
            //_states = new List<BaseState>();
            _pool = new StatePool();
        }
        protected override void OnDispose()
        {
            //_states.Clear();
            _pool.Dispose();
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /// <summary>
        /// 分配状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Allocate<T>()where T: BaseState,new ()
        {
            var state=_pool.Get<T>();
            state.recorder = this;
            return state;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="state"></param>
        /// <param name="redo"></param>
        public void Subscribe(BaseState state,bool redo=true)
        {
            if (state.recorder == null)
                Log.E("Don't Create new State ,Use Allocate,");
            //_states.Add(state);
            if (_current == null) _current = _head;
            if (_current.next!=null) Recyle(_current.next);
            _current.next = state;
            state.front = _current;
            if (redo) state.Redo();
            _current = state;
        }
        private void Recyle(BaseState state)
        {
            do
            {
                var now = state;
                state = state.next;
                now.Reset();
                _pool.Set(now);
            } while (state!=null);
        }
        /// <summary>
        /// 撤回
        /// </summary>
        /// <returns></returns>
        public bool Undo()
        {
            if (_current == _head) return false;
            _current.Undo();
            _current = _current.front;
            return true;
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public bool Redo()
        {
            if (_current.next == null) return false;
            _current = _current.next;
            _current.Redo();
            return true;
        }
    }

    /// <summary>
    /// 扩展
    /// </summary>
    public static class OperationRecorderEx
    {
        /// <summary>
        /// 分配命令
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CommandState AllocateCommand(this IOperationRecorderModule t) 
        {
            return t.Allocate<CommandState>();
        }
        /// <summary>
        /// 分配回调
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ActionState AllocateAction(this IOperationRecorderModule t)
        {
            return t.Allocate<ActionState>();
        }
        /// <summary>
        /// 分配命令组
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CommandGroupState AllocateCommandGroup(this IOperationRecorderModule t)
        {
            return t.Allocate<CommandGroupState>();
        }
        /// <summary>
        /// 分配回调组
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ActionGroupState AllocateActionGroup(this IOperationRecorderModule t)
        {
            return t.Allocate<ActionGroupState>();
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetCommand<T>(this T t,ICommand redo,ICommand undo)where T:CommandState
        {
            t.SetValue(redo, undo);
            return t;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetGroupCommand<T>(this T t, ICommand redo, ICommand undo) where T : CommandGroupState
        {
            t.SetValue(redo, undo);
            return t;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetCommand<T>(this T t, Action redo, Action undo) where T : ActionState
        {
            t.SetValue(redo, undo);
            return t;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetGroupCommand<T>(this T t, Action redo, Action undo) where T : ActionGroupState
        {
            t.SetValue(redo, undo);
            return t;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <returns></returns>
        public static T Subscribe<T>(this T t, bool redo = true) where T :BaseState
        {
            t.recorder.Subscribe(t,redo);
            return t;
        }
    }
}
