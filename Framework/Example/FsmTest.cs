using System;
using IFramework;
using IFramework.Modules.Fsm;

namespace Example
{
    public class FsmTest : Test
    {
        public class State : IState
        {
            public void OnEnter()
            {
                Log.L(GetType() + "  Enter");
            }
            public void OnExit()
            {
                Log.L(GetType() + "  OnExit");

            }
            public void Update()
            {
                Log.L(GetType() + "  Update");
            }
        }
        public class State1 : State { }
        public class State2 : State { }
        IFsmModule fsm { get { return Framework.GetEnv(EnvironmentType.Ev0).modules.Fsm; } }
        protected override void Start()
        {
            Log.L("按下  A/D   切换状态");
            State1 s1 = new State1();
            State2 s2 = new State2();
            fsm.SubscribeState(s1);
            fsm.enterState = s1;
            fsm.SubscribeState(s2);
            // fsm.exitState = s2;
            var val = fsm.CreateConditionValue<bool>("bool", true);

            var t1 = fsm.CreateTransition(s1, s2);
            var t2 = fsm.CreateTransition(s2, s1);

            t1.BindCondition(fsm.CreateCondition<bool>("bool", false, CompareType.Equals));
            t2.BindCondition(fsm.CreateCondition<bool>(val, true, CompareType.Equals));

            fsm.Start();
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
            if (Console.ReadKey().Key== ConsoleKey.D)
            {
                fsm.SetBool("bool", false);
            }
            if (Console.ReadKey().Key == ConsoleKey.A)
            {
                fsm.SetBool("bool", true);
            }
        }
    }
}
