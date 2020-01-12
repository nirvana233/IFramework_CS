using System;
using System.Collections.Generic;

namespace IFramework.Moudles.Fsm
{
    public class FsmMoudle : FrameworkMoudle
    {
        private class StateInfo
        {
            private IFsmState state { get;  }
            private List<FsmTransition> transitions { get; }
            public StateInfo(IFsmState state)
            {
                this.state = state;
                transitions = new List<FsmTransition>();
            }

            public void CreateTransition(FsmTransition stateTransition)
            {
                var sameOne = transitions.Find((t) => { return t.Trail == stateTransition.Trail; });
                if (sameOne == null)
                    transitions.Add(stateTransition);
                else
                    throw new Exception("The Trastion Same");
            }
            public void DestoryTransition(IFsmState trail)
            {
                var sameOne = transitions.Find((t) => { return t.Trail == trail; });
                if (sameOne != null)
                    transitions.Remove(sameOne);
                else
                    throw new Exception("The Trastion not Exist");
            }
            public IFsmState TryGoNext()
            {
                for (int i = 0; i < transitions.Count; i++)
                {
                    if (transitions[i].IsMetCondition)
                    {
                        FsmTransition transition = transitions[i];
                        return transition.GoToNextState();
                    }
                }
                return null;
            }
        }
        public IFsmState ExitState { get; set; }
        public IFsmState EnterState { get; set; }
        public IFsmState CurrentState { get; private set; }

        public bool IsRuning { get; private set; }
        public bool IsDisposed { get; private set; }

        private Dictionary<IFsmState, StateInfo> stateInfo;
        private Dictionary<Type, Dictionary<string, IFsmConditionValue>> conditionValues;

        public FsmMoudle(string chunck):base(chunck)
        {
            IsRuning = false;
            IsDisposed = false;
            stateInfo = new Dictionary<IFsmState, StateInfo>();
            conditionValues = new Dictionary<Type, Dictionary<string, IFsmConditionValue>>();
        }
        protected override void OnDispose()
        {
            IsDisposed = true;
            IsRuning = false;
            CurrentState = EnterState =ExitState = null;
            stateInfo.Clear();
            conditionValues.Clear();
        }
        public void Start()
        {
            if (IsDisposed) return;
            if (EnterState == null) throw new Exception("EnterState Is Null");
            IsRuning = true;
            EnterState.OnEnter();
            CurrentState = EnterState;
        }
        public void Pause()
        {
            if (IsDisposed) return;
            IsRuning = false;
        }
        public void UnPause()
        {
            if (IsDisposed) return;
            IsRuning = true;
        }

        public override void Update()
        {
            if (IsDisposed) return;
            if (!IsRuning) return;
            CurrentState.Update();
            TryGoNext();
            if (CurrentState == ExitState)
                IsRuning = false;
        }
        private void TryGoNext()
        {
            if (CurrentState == null) return;
            var state= stateInfo[CurrentState].TryGoNext();
            if (state == null) return;
            CurrentState = state;
        }




        public void SubscribeState(IFsmState state)
        {
            if (!stateInfo.ContainsKey(state))
                stateInfo.Add(state, new StateInfo(state));
            else
                Log.E(" Have  Exist  State ");
        }
        public void UnSubscribeState(IFsmState state)
        {
            if (stateInfo.ContainsKey(state))
                stateInfo.Remove(state);
            else
                Log.E("Not  Exist  State ");
        }


        public FsmTransition CreateTransition(IFsmState head, IFsmState trail)
        {
            if (!stateInfo.ContainsKey(head))
            {
                Log.E("Subscribe Head State  Fist");
                return null;
            }
            if (!stateInfo.ContainsKey(trail))
            {
                Log.E("Subscribe Trail State  Fist");
                return null;
            }
            FsmTransition transition = new FsmTransition();
            transition.Head = head;
            transition.Trail = trail;
            stateInfo[head].CreateTransition(transition);
            return transition;
        }
        public void DestoryTransition(IFsmState head, IFsmState trail)
        {
            if (!stateInfo.ContainsKey(head))
            {
                Log.E("Subscribe Head State  Fist");
                return;
            }
            if (!stateInfo.ContainsKey(trail))
            {
                Log.E("Subscribe Trail State  Fist");
                return;
            }
            stateInfo[head].DestoryTransition(trail);
        }


        public FsmConditionValue<T> CreateConditionValue<T>(string name, T value)
        {
            if (!conditionValues.ContainsKey(typeof(T)))
                conditionValues.Add(typeof(T), new Dictionary<string, IFsmConditionValue>());
            if (!conditionValues[typeof(T)].ContainsKey(name))
                conditionValues[typeof(T)].Add(name, new FsmConditionValue<T>() { Name = name, Value = value });
            else
                Log.E("ConditionValue Exsit " + name);
            return conditionValues[typeof(T)][name] as FsmConditionValue<T>;
        }
        public void DestoryConditionValue<T>(string name, T value)
        {
            if (!conditionValues.ContainsKey(typeof(T)) || !conditionValues[typeof(T)].ContainsKey(name))
                Log.E("ConditionValue Not Exsit " + name);
            else
                conditionValues[typeof(T)].Remove(name);
        }

        public TransitionCondition<T> CreateCondition<T>(string conditionValName, T CompareValue, ConditionCompareType CompareType)
        {
            if (!conditionValues.ContainsKey(typeof(T)) || !conditionValues[typeof(T)].ContainsKey(conditionValName))
            {
                Log.E("Please Create ConditionVal First Type " + typeof(T) + "  Name  " + conditionValName);
                return default(TransitionCondition<T>);
            }
            return CreateCondition(conditionValues[typeof(T)][conditionValName] as FsmConditionValue<T>, CompareValue, CompareType);
        }
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



        public bool TrySetConditionValue(Type valType, string name,object value)
        {
            if (!conditionValues.ContainsKey(valType))
            {
                Log.E("Not Have Condition Type  " + valType);
                return false;
            }
            if (!conditionValues[valType].ContainsKey(name))
            {
                Log.E("Not Have ConditionVal Name  " + name);
                return false;
            }
            conditionValues[valType][name].Value = value;
            return true;
        }
        public bool SetBool(string valName, bool value)
        {
            return TrySetConditionValue(typeof(bool), valName, value);
        }
        public bool SetInt(string valName, int value)
        {
            return TrySetConditionValue(typeof(int), valName, value);
        }
        public bool SetFloat(string valName, float value)
        {
            return TrySetConditionValue(typeof(float), valName, value);
        }
        public bool SetString(string valName, string value)
        {
            return TrySetConditionValue(typeof(string), valName, value);
        }

    }
}
