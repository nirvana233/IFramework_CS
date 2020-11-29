//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using IFramework.Modules.Message;
//namespace IFramework
//{
//    class Program:IMessagePublisher
//    {

//        static MessageModule moudle;
//        static void Main(string[] args)
//        {
//            moudle = MessageModule.CreatInstance<MessageModule>("", "");
//            moudle.Subscribe<Program>(Listen);
//            for (int i = 0; i < 10; i++)
//            {
//                moudle.Publish(typeof(Program),10- i, null, 10 - i);
//            }

//            while (true)
//            {
//                moudle.Update();
//                //Console.Clear();
//                ////Console.WriteLine(System.DateTime.Now.ToString());

//                ////moudle.Publish<Program>(3, null, MessageUrgencyType.Unimportant);
//                ////moudle.Publish<Program>(0, null, MessageUrgencyType.VeryUrgent);

//                //moudle.Update();
//                //Thread.Sleep(99);

//            }

//        }

//        private static void Listen(Type publishType, int code, IEventArgs args, object[] param)
//        {
//            Console.WriteLine("code "+code);
//            Thread.Sleep(100);
//            moudle.Publish(typeof(Program), code, null, code);
//        }
//    }
//}
