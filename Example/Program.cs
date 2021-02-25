using System;
using System.Timers;
using IFramework;
using IFramework.Modules.Fsm;

namespace Example
{

    class Program
    {
        private static System.Timers.Timer timer =new System.Timers.Timer(1000);
        static void Main(string[] args)
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Framework.CreateEnv("", EnvironmentType.Ev0).InitWithAttribute();
            Log.L("按键盘 esc 退出 ");

            TestScript();
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {

            }
            Framework.env0.Dispose();
        }
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Framework.env0.Update();
        }

        private static void TestScript()
        {
            // new AstarTest();             //  A 星寻路
            // new NodeActionTest();       // 节点事件
            // new InjectTest();              // 依赖注入
            // new DataTableTest();              // 数据表 CSV
            //new BindTest();                   //数据绑定 （单向/数据变化监听，双向/数值同步变化）
            // new NetTest();                     // 网络测试



            // new MessageExample();               //消息模块
            // new RecorderTest();              //操作记录模块
            //  new ConfigTest();                // 配置模块
            // new CoroutineTest();              //协程 模块
           /// new FsmTest();                    // 状态机模块
        }

    }
    public abstract class Test:FrameworkObject
    {

        public Test()
        {
            Framework.BindEnvUpdate(Update, EnvironmentType.Ev0);
            Log.L("开始测试----------->"+GetType());
            Start();
        }
        protected abstract void Start();

        protected abstract void Update();
        protected abstract void Stop();

        protected override void OnDispose()
        {
            Stop();
            Framework.UnBindEnvUpdate(Update, EnvironmentType.Ev0);
            base.OnDispose();
        }
    }

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
        IFsmModule fsm { get { return Framework.env0.modules.Fsm; } }
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
