//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace IFramework.Fast
//{
//    class EditorEntity : RootEntity<EditorEntity>
//    {
//        private class UnSafaScriptEnvCheckCommand : ICommand
//        {
//            public void Excute()
//            {
//                Console.WriteLine(6654654);
//            }
//        }
//        protected EditorEntity() { }
//        protected override EnvironmentType envType { get { return EnvironmentType.Ev2; } }

//        protected override void Awake()
//        {
//            Console.WriteLine("awake");
//            SendCommand(new UnSafaScriptEnvCheckCommand());
//        }

//        protected override void OnDispose()
//        {
//        }
//    }
//    class Class1
//    {
//        static void Main(string[] args)
//        {
//            EditorEntity.Initialize();
//            while (true)
//            {
//                EditorEntity.Update();
//            }
//            //  EditorEntity.Destory();
//        }
//    }
//}
