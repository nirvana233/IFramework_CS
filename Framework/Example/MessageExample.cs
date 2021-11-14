using System;
using IFramework;
using IFramework.Modules.Message;

namespace Example
{
    public class MessageExample : Test
    {
        public interface IPub { }
        public class Pub : IPub { }
        public class Listenner : IMessageListener
        {
            public Listenner()
            {
                Framework.GetEnv( EnvironmentType.Ev0).modules.Message.Subscribe<IPub>(Listen);

                Framework.GetEnv(EnvironmentType.Ev0).modules.Message.Subscribe<MessageExample>(this);
            }
            public void Listen(IMessage message)
            {
                Log.L(string.Format("Recieve code {0} by type {1}", message.code, message.subject));
            }
        }


        protected override void Start()
        {
            Listenner listenner = new Listenner();
            Log.L("不要太频繁按键，因为 1秒 检测一次按键");
            Framework.GetEnv(EnvironmentType.Ev0).modules.Message.fitSubType = true;
            Framework.GetEnv(EnvironmentType.Ev0).modules.Message.processesPerFrame = 2;
            Framework.GetEnv(EnvironmentType.Ev0).modules.Message.Publish<Pub>(null)
                .SetCode(100)
                .OnCompelete((msg)=> {
                    Log.L($"state  {msg.state}   err   {msg.errorCode}");
                    Log.L("\n\n");
                    Log.L("开始按键");
                });
        }

        protected async override void Update()
        {
            var key = Console.ReadKey();
            Log.L($"检测到按键  {key.Key} 并且发送消息 ,  按键序号为    {(int)key.Key}");
            Log.L($"剩余消息条数  {Framework.GetEnv(EnvironmentType.Ev0).modules.Message.count}");
            for (int i = 0; i < 10; i++)
            {
                Log.L($"消息开始 {i}");
                await Framework.GetEnv(EnvironmentType.Ev0).modules.Message.PublishByNumber(this, null, (int)key.Key - i)
                       .SetCode((int)key.Key - i);
                Log.L($"消息结束 {i}");
            }

        }

        protected override void Stop()
        {

        }
    }

}
