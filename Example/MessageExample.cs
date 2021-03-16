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
                Framework.env0.modules.Message.Subscribe<IPub>(Listen);

                Framework.env0.modules.Message.Subscribe<MessageExample>(this);
            }
            public void Listen(IMessage message)
            {
                Log.L(string.Format("Recieve code {0} by type {1}", message.code, message.type));
            }
        }


        protected override void Start()
        {
            Listenner listenner = new Listenner();
            Log.L("不要太频繁按键，因为 1秒 检测一次按键");
            //Framework.env0.modules.Message.fitSubType = false;
            Framework.env0.modules.Message.processesPerFrame = 1;

            IMessage message = null;
            message = Framework.env0.modules.Message.Publish<Pub>(null)
                .SetCode(100)
                .OnRecycle((msg)=> {
                    Log.L($"equals  {message == msg}");
                    Log.L($"state  {msg.state}   err   {msg.errorCode}");

                    Log.L("\n\n");
                    Log.L("开始按键");
                });
            Log.L(string.Format("the position is {0} priority is {1}", message.position, message.priority));
           

        }

        protected override void Update()
        {
            var key = Console.ReadKey();
            Log.L($"检测到按键  {key.Key} 并且发送消息 ,  按键序号为    {(int)key.Key}");
            Log.L($"剩余消息条数  {Framework.env0.modules.Message.count}");
            for (int i = 0; i < 2; i++)
            {
                Framework.env0.modules.Message.PublishByNumber(this, null, (int)key.Key - i)
                    .SetCode((int)key.Key - i)
                    .OnRecycle((msg) => {
                        Log.L($"state  {msg.state}   err   {msg.errorCode}");
                    });
            }

        }

        protected override void Stop()
        {
        }
    }

}
