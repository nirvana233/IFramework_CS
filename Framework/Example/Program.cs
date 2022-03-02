using System;
using System.Collections.Generic;
using System.Timers;
using IFramework;

namespace Example
{
    public abstract class Test : Unit
    {

        public Test()
        {
            Framework.BindEnvUpdate(Update, EnvironmentType.Ev0);
            Framework.BindEnvDispose(Dispose, EnvironmentType.Ev0);

            Log.L("开始测试----------->" + GetType());
            Start();
        }
        protected abstract void Start();

        protected abstract void Update();
        protected abstract void Stop();

        protected override void OnDispose()
        {
            Stop();
            Framework.UnBindEnvDispose(Dispose, EnvironmentType.Ev0);

            Framework.UnBindEnvUpdate(Update, EnvironmentType.Ev0);
        }
    }
    class Program
    {
        private static System.Timers.Timer timer = new System.Timers.Timer(1000);
        static void Main(string[] args)
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Log.L("按键盘 esc 关闭测试环境 ");
            Log.L("开启  0 号环境 测试 ");
            Framework.CreateEnv(EnvironmentType.Ev0).InitWithAttribute();

            TestScripts();
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
            Framework.GetEnv(EnvironmentType.Ev0).Dispose();
            Log.L(" 0 号环境 已经关闭 ");

            Log.L("按键盘 esc 退出 ");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Framework.GetEnv(EnvironmentType.Ev0).Update();
        }

        private static void TestScripts()
        {
    
            // new AstarTest();                 //  A 星寻路
              new SerializationTest();             // 数据表 CSV
            //  new BindTest();                   //数据绑定 （单向/数据变化监听，双向/数值同步变化）
            //new NetTest();                    // 网络测试
            //   new SingletonTest();             //单例测试
             //  new PoolTest();                   //对象池测试
            // new PriorityQueueTest();            //优先级队列

            // new MvvmTest();
            //  new MessageExample();            //消息模块
            //new RecyclableObjectTest();      //可回收对象测试
            //  new NodeActionTest();            // 节点事件
            // new RecorderTest();              //操作记录模块
            // new InjectTest();                // 依赖注入
            //  new ConfigTest();               // 配置模块
            //new CoroutineTest();             //协程 模块
            // new FsmTest();                   // 状态机模块
            // new MouduleTest();                //自定义模块

        }

    }



}
