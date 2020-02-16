using System;
using System.Collections.Generic;

namespace IFramework.Modules.Fsm
{
    /// <summary>
    /// 状态机
    /// </summary>
    [FrameworkVersion(66)]
    public class FsmModule : FrameworkModule
    {
        private class StateInfo
        {
            private IFsmState _state { get; }
            private List<FsmTransition> _transitions { get; }
            public StateInfo(IFsmState state)
            {
                this._state = state;
                _transitions = new List<FsmTransition>();
            }

            public void CreateTransition(FsmTransition stateTransition)
            {
                var sameOne = _transitions.Find((t) => { return t.trail == stateTransition.trail; });
                if (sameOne == null)
                    _transitions.Add(stateTransition);
                else
                    throw new Exception("The Trastion Same");
            }
            public void DestoryTransition(IFsmState trail)
            {
                var sameOne = _transitions.Find((t) => { return t.trail == trail; });
                if (sameOne != null)
                    _transitions.Remove(sameOne);
                else
                    throw new Exception("The Trastion not Exist");
            }
            public IFsmState TryGoNext()
            {
                for (int i = 0; i < _transitions.Count; i++)
                {
                    if (_transitions[i].IsMetCondition)
                    {
                        FsmTransition transition = _transitions[i];
                        return transition.GoToNextState();
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// 退出状态
        /// </summary>
        public IFsmState ExitState { get; set; }
        /// <summary>
        /// 第一个状态
        /// </summary>
        public IFsmState EnterState { get; set; }
        /// <summary>
        /// 当状态改变
        /// </summary>
        public event Action<IFsmState> onStateChange;
        private IFsmState _CurrentState;
        /// <summary>
        /// 当前状态
        /// </summary>
        public IFsmState CurrentState { get { return _CurrentState; }
           private set {
                if (value!= _CurrentState)
                {
                    _CurrentState = value;
                    if (onStateChange != null)
                        onStateChange(value);
                }
            }
        }
        /// <summary>
        /// 是否在运行
        /// </summary>
        public bool IsRuning { get { return enable; } }

        private Dictionary<IFsmState, StateInfo> _stateInfo;
        private Dictionary<Type, Dictionary<string, IFsmConditionValue>> _conditionValues;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override void Awake()
        {
            enable = false;
            _stateInfo = new Dictionary<IFsmState, StateInfo>();
            _conditionValues = new Dictionary<Type, Dictionary<string, IFsmConditionValue>>();
        }
        protected override void OnDispose()
        {
            CurrentState = EnterState = ExitState = null;
            _stateInfo.Clear();
            _conditionValues.Clear();
        }
        protected override void OnUpdate()
        {
            //if (disposed) return;
            //if (!IsRuning) return;
            CurrentState.Update();
            TryGoNext();
            if (CurrentState == ExitState)
                enable = false;
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /// <summary>
        /// 开始运行
        /// </summary>
        public void Start()
        {
            if (disposed) return;
            if (EnterState == null) throw new Exception("EnterState Is Null");
            enable = true;
            EnterState.OnEnter();
            CurrentState = EnterState;
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (disposed) return;
            enable = false;
        }
        /// <summary>
        /// 重新运行
        /// </summary>
        public void UnPause()
        {
            if (disposed) return;
            enable = true;
        }

        private void TryGoNext()
        {
            if (CurrentState == null) return;
            var state = _stateInfo[CurrentState].TryGoNext();
            if (state == null) return;
            CurrentState = state;
        }



        /// <summary>
        /// 注册状态
        /// </summary>
        /// <param name="state"></param>
        public void SubscribeState(IFsmState state)
        {
            if (!_stateInfo.ContainsKey(state))
                _stateInfo.Add(state, new StateInfo(state));
            else
                Log.E(" Have  Exist  State ");
        }
        /// <summary>
        /// 解除注册状态
        /// </summary>
        /// <param name="state"></param>
        public void UnSubscribeState(IFsmState state)
        {
            if (_stateInfo.ContainsKey(state))
                _stateInfo.Remove(state);
            else
                Log.E("Not  Exist  State ");
        }

        /// <summary>
        /// 创建过渡线
        /// </summary>
        /// <param name="head"></param>
        /// <param name="trail"></param>
        /// <returns></returns>
        public FsmTransition CreateTransition(IFsmState head, IFsmState trail)
        {
            if (!_stateInfo.ContainsKey(head))
            {
                Log.E("Subscribe Head State  Fist");
                return null;
            }
            if (!_stateInfo.ContainsKey(trail))
            {
                Log.E("Subscribe Trail State  Fist");
                return null;
            }
            FsmTransition transition = new FsmTransition();
            transition.head = head;
            transition.trail = trail;
            _stateInfo[head].CreateTransition(transition);
            return transition;
        }
        /// <summary>
        /// 删除过渡线
        /// </summary>
        /// <param name="head"></param>
        /// <param name="trail"></param>
        /// <returns></returns>
        public void DestoryTransition(IFsmState head, IFsmState trail)
        {
            if (!_stateInfo.ContainsKey(head))
            {
                Log.E("Subscribe Head State  Fist");
                return;
            }
            if (!_stateInfo.ContainsKey(trail))
            {
                Log.E("Subscribe Trail State  Fist");
                return;
            }
            _stateInfo[head].DestoryTransition(trail);
        }

        /// <summary>
        /// 创建过渡条件值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public FsmConditionValue<T> CreateConditionValue<T>(string name, T value)
        {
            if (!_conditionValues.ContainsKey(typeof(T)))
                _conditionValues.Add(typeof(T), new Dictionary<string, IFsmConditionValue>());
            if (!_conditionValues[typeof(T)].ContainsKey(name))
                _conditionValues[typeof(T)].Add(name, new FsmConditionValue<T>(name, value));
            else
                Log.E("ConditionValue Exsit " + name);
            return _conditionValues[typeof(T)][name] as FsmConditionValue<T>;
        }
        /// <summary>
        /// 删除过渡条件值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void DestoryConditionValue<T>(string name, T value)
        {
            if (!_conditionValues.ContainsKey(typeof(T)) || !_conditionValues[typeof(T)].ContainsKey(name))
                Log.E("ConditionValue Not Exsit " + name);
            else
                _conditionValues[typeof(T)].Remove(name);
        }
        /// <summary>
        /// 创建过度条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionValName"></param>
        /// <param name="CompareValue"></param>
        /// <param name="CompareType"></param>
        /// <returns></returns>
        public TransitionCondition<T> CreateCondition<T>(string conditionValName, T CompareValue, ConditionCompareType CompareType)
        {
            if (!_conditionValues.ContainsKey(typeof(T)) || !_conditionValues[typeof(T)].ContainsKey(conditionValName))
            {
                Log.E("Please Create ConditionVal First Type " + typeof(T) + "  Name  " + conditionValName);
                return default(TransitionCondition<T>);
            }
            return CreateCondition(_conditionValues[typeof(T)][conditionValName] as FsmConditionValue<T>, CompareValue, CompareType);
        }
        /// <summary>
        /// 创建过度条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="CompareValue"></param>
        /// <param name="CompareType"></param>
        /// <returns></returns>
        public TransitionCondition<T> CreateCondition<T>(FsmConditionValue<T> value, T CompareValue, ConditionCompareType CompareType)
        {
            ITransitionCondition condtion;
            if (typeof(T) == typeof(bool))
                condtion = new BoolTransitionCondition(value as FsmConditionValue<bool>, CompareValue, CompareType);
            else if (typeof(T) == typeof(string))
                condtion = new StringTransitionCondition(value as FsmConditionValue<string>, CompareValue, CompareType);
            else if (typeof(T) == typeof(float))
                condtion = new FloatTransitionCondition(value as FsmConditionValue<float>, CompareValue, CompareType);
            else if (typeof(T) == typeof(int))
                condtion = new IntTransitionCondition(value as FsmConditionValue<int>, CompareValue, CompareType);
            else
                throw new Exception("Fault Type Of T   " + typeof(T));
            return condtion as TransitionCondition<T>;
        }



        private bool TrySetConditionValue(Type valType, string name, object value)
        {
            if (!_conditionValues.ContainsKey(valType))
            {
                Log.E("Not Have Condition Type  " + valType);
                return false;
            }
            if (!_conditionValues[valType].ContainsKey(name))
            {
                Log.E("Not Have ConditionVal Name  " + name);
                return false;
            }
            _conditionValues[valType][name].value = value;
            return true;
        }
        /// <summary>
        /// 设置bool
        /// </summary>
        /// <param name="valName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetBool(string valName, bool value)
        {
            return TrySetConditionValue(typeof(bool), valName, value);
        }
        /// <summary>
        /// 设置int
        /// </summary>
        /// <param name="valName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetInt(string valName, int value)
        {
            return TrySetConditionValue(typeof(int), valName, value);
        }
        /// <summary>
        /// 设置float
        /// </summary>
        /// <param name="valName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetFloat(string valName, float value)
        {
            return TrySetConditionValue(typeof(float), valName, value);
        }
        /// <summary>
        /// 设置string
        /// </summary>
        /// <param name="valName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetString(string valName, string value)
        {
            return TrySetConditionValue(typeof(string), valName, value);
        }


    }
}
