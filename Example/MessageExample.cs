using System;
using IFramework;
using IFramework.Modules.Message;

namespace Example
{
    public class MessageExample : Test, IMessagePublisher
    {
        public interface IPub : IMessagePublisher { }
        public class Pub : IPub
        { }
        public class Listenner : IMessageListener
        {
            public Listenner()
            {
                Framework.env0.modules.Message.Subscribe<IPub>(Listen);

                 Framework.env0.modules.Message.Subscribe<MessageExample>(this);
            }
            public void Listen(Type eventType, int code, IEventArgs args, params object[] param)
            {
                Log.L(string.Format("Recieve code {0} by type {1}", code, eventType));
            }
        }


        protected override void Start()
        {
            Listenner listenner = new Listenner();
            Log.L("不要太频繁按键，因为 1秒 检测一次按键");
            Framework.env0.modules.Message.Publish<Pub>(100, null);
        }

        protected override void Update()
        {
            var key = Console.ReadKey();
            Log.L($"检测到按键  {key.Key} 并且发送消息 ,  按键序号为    {(int)key.Key}" );

            Framework.env0.modules.Message.Publish(this,(int)key.Key, null);
        }

        protected override void Stop()
        {
        }
    }

}
