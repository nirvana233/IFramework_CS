using System;
using System.Collections.Generic;

namespace IFramework.Modules.Fsm
{
    /// <summary>
    /// 状态机
    /// </summary>
    [VersionAttribute(66)]
    public class FsmModule : UpdateFrameworkModule
    {
        private class StateInfo
        {
            private IState _state { get; }
            private List<Transition> _transitions { get; }
            public StateInfo(IState state)
            {
                this._state = state;
                _transitions = new List<Transition>();
            }

            public void CreateTransition(Transition stateTransition)
            {
                var sameOne = _transitions.Find((t) => { return t.trail == stateTransition.trail; });
                if (sameOne == null)
                    _transitions.Add(stateTransition);
                else
                    throw new Exception("The Trastion Same");
            }
            public void DestoryTransition(IState trail)
            {
                var sameOne = _transitions.Find((t) => { return t.trail == trail; });
                if (sameOne != null)
                    _transitions.Remove(sameOne);
                else
                    throw new Exception("The Trastion not Exist");
            }
            public IState TryGoNext()
            {
                for (int i = 0; i < _transitions.Count; i++)
                {
                    if (_transitions[i].IsMetCondition)
                    {
                        Transition transition = _transitions[i];
                        return transition.GoToNextState();
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// 退出状态
        /// </summary>
        public IState exitState { get; set; }
        /// <summary>
        /// 第一个状态
        /// </summary>
        public IState enterState { get; set; }
        /// <summary>
        /// 当状态改变
        /// </summary>
        public event Action<IState> onStateChange;
        private IState _currentState;
        /// <summary>
        /// 当前状态
        /// </summary>
        public IState currentState { get { return _currentState; }
           private set {
                if (value!= _currentState)
                {
                    _currentState = value;
                    if (onStateChange != null)
                        onStateChange(value);
                }
            }
        }
        /// <summary>
        /// 是否在运行
        /// </summary>
        public bool runing { get { return enable; } }

        private Dictionary<IState, StateInfo> _stateInfo;
        private Dictionary<Type, Dictionary<string, IConditionValue>> _conditionValues;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public override int priority { get { return 60; } }

        protected override void Awake()
        {
            enable = false;
            _stateInfo = new Dictionary<IState, StateInfo>();
            _conditionValues = new Dictionary<Type, Dictionary<string, IConditionValue>>();
        }
        protected override void OnDispose()
        {
            currentState = enterState = exitState = null;
            _stateInfo.Clear();
            _conditionValues.Clear();
        }
        protected override void OnUpdate()
        {
            //if (disposed) return;
            //if (!IsRuning) return;
            currentState.Update();
            TryGoNext();
            if (currentState == exitState)
                enable = false;
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /// <summary>
        /// 开始运行
        /// </summary>
        public void Start()
        {
            if (disposed) return;
            if (enterState == null) throw new Exception("EnterState Is Null");
            enable = true;
            enterState.OnEnter();
            currentState = enterState;
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
            if (currentState == null) return;
            var state = _stateInfo[currentState].TryGoNext();
            if (state == null) return;
            currentState = state;
        }



        /// <summary>
        /// 注册状态
        /// </summary>
        /// <param name="state"></param>
        public void SubscribeState(IState state)
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
        public void UnSubscribeState(IState state)
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
        public Transition CreateTransition(IState head, IState trail)
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
            Transition transition = new Transition();
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
        public void DestoryTransition(IState head, IState trail)
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
        public ConditionValue<T> CreateConditionValue<T>(string name, T value)
        {
            if (!_conditionValues.ContainsKey(typeof(T)))
                _conditionValues.Add(typeof(T), new Dictionary<string, IConditionValue>());
            if (!_conditionValues[typeof(T)].ContainsKey(name))
                _conditionValues[typeof(T)].Add(name, new ConditionValue<T>(name, value));
            else
                Log.E("ConditionValue Exsit " + name);
            return _conditionValues[typeof(T)][name] as ConditionValue<T>;
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
        public Condition<T> CreateCondition<T>(string conditionValName, T CompareValue, CompareType CompareType)
        {
            if (!_conditionValues.ContainsKey(typeof(T)) || !_conditionValues[typeof(T)].ContainsKey(conditionValName))
            {
                Log.E("Please Create ConditionVal First Type " + typeof(T) + "  Name  " + conditionValName);
                return default(Condition<T>);
            }
            return CreateCondition(_conditionValues[typeof(T)][conditionValName] as ConditionValue<T>, CompareValue, CompareType);
        }
        /// <summary>
        /// 创建过度条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="CompareValue"></param>
        /// <param name="CompareType"></param>
        /// <returns></returns>
        public Condition<T> CreateCondition<T>(ConditionValue<T> value, T CompareValue, CompareType CompareType)
        {
            ICondition condtion;
            if (typeof(T) == typeof(bool))
                condtion = new BoolCondition(value as ConditionValue<bool>, CompareValue, CompareType);
            else if (typeof(T) == typeof(float))
                condtion = new FloatCondition(value as ConditionValue<float>, CompareValue, CompareType);
            else if (typeof(T) == typeof(int))
                condtion = new IntCondition(value as ConditionValue<int>, CompareValue, CompareType);
            else
                throw new Exception("Fault Type Of T   " + typeof(T));
            return condtion as Condition<T>;
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


    }
}
