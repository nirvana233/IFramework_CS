//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace IFramework
//{
//    class Program
//    {
//        class BindA : BindableObject
//        {
//            private bool _startGame;

//            public bool startGame
//            {
//                get { return GetProperty(ref _startGame, "startGame"); }
//                set { SetProperty(ref _startGame, value, "startGame"); }
//            }


//        }
//        private class CallClass<T>
//        {
//            private event Action<string, T> call;

//            public void Invoke(string name,T t)
//            {
//                if (call == null) return;
//                call.Invoke(name, t);
//            }
//            public static CallClass<T> operator +(CallClass<T> _class, Action<string, T> call)
//            {
//                _class.call += call;
//                return _class;
//            }
//            public static CallClass<T> operator -(CallClass<T> _class, Action<string, T> call)
//            {
//                _class.call -= call;
//                return _class;
//            }
//        }

//        static void Main(string[] args)
//        {
//            //BindA a = new BindA();
//            //BindA b = new BindA();
//            //a.bindOperation = BindableObject.BindOperation.Listen;

//            //BindableObjectHandler binder = new BindableObjectHandler();
//            //binder.BindProperty((value) => { b.startGame = value; }, () => { return b.startGame; });
//            //binder.BindProperty((value) => { a.startGame = value; }, () => { return a.startGame; });
//            //Console.WriteLine($"A :{a.startGame}   B :{b.startGame}");

//            //a.startGame = true;
//            //Console.WriteLine($"A :{a.startGame}   B :{b.startGame}");

//            ////binder.UnBind(a);

//            //a.startGame = false;
//            //Console.WriteLine($"A :{a.startGame}   B :{b.startGame}");


//            ////b.startGame = false;
//            ////Console.WriteLine($"A :{a.startGame}   B :{b.startGame}");
//            //b.startGame = true;

//            //Console.WriteLine($"A :{a.startGame}   B :{b.startGame}");

//            Console.WriteLine(ValueCurve.linecurve.GetPercent(1).ToString());
//            while (true)
//            {

//            }

//        }
//    }
//}
