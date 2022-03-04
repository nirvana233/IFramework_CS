using IFramework;
using System;

namespace Example
{
    public class PoolTest : Test
    {
        public interface IObject { }
        public class Obj_A : IObject { }
        public class Obj_B : IObject { }
        public class Obj_C
        {
            public readonly int age;

            public Obj_C(int age)
            {
                Log.L("age is    "+age);
                this.age = age;
            }
        }

        private void FastExample()
        {
            Log.L("Create a auto create  pool");
            ActivatorCreatePool<Obj_A> pool = new ActivatorCreatePool<Obj_A>();
            Log.L("Get an instance from  pool");

            var _obj = pool.Get();
            Log.L($"the type of instance is {_obj.GetType()}");
            Log.L("Let's put the instance to pool");
            pool.Set(_obj);
            ActivatorCreatePool<Obj_C> pool_c = new ActivatorCreatePool<Obj_C>(15);
            pool_c.Get();
        }
        private class MyPool : ObjectPool<Obj_A>
        {
            protected override Obj_A CreatNew(IEventArgs arg)
            {
                return new Obj_A();
            }
        }
        private void NormalTest()
        {
            Log.L("Create a auto create  pool");
            MyPool pool = new MyPool();
            Log.L("Get an instance from  pool");

            var _obj = pool.Get();
            Log.L($"the type of instance is {_obj.GetType()}");
            Log.L("Let's put the instance to pool");
            pool.Set(_obj);
        }




        private class MutiPool : BaseTypePool<IObject> { }

        private class MyPool2 : ObjectPool<Obj_A>
        {
            protected override Obj_A CreatNew(IEventArgs arg)
            {
                Console.WriteLine("----------------------------");
                return new Obj_A();
            }
        }
        private void MutiTest()
        {
            Log.L("Create a auto create  pool");
            MutiPool pool = new MutiPool();
            Log.L("Get an instance  A   from  pool");
            pool.SetPool(new MyPool2());
            IObject _obj = pool.Get<Obj_A>();
            //IObject _obj = pool.Get(typeof(Obj_A));

            Log.L($"the type of instance is {_obj.GetType()}");
            Log.L("Let's put the instance to pool");
            pool.Set(_obj);
            //pool.Set(typeof(Obj_A),_obj);

            Log.L("Get an instance  B from  pool");

             _obj = pool.Get<Obj_B>();
            Log.L($"the type of instance is {_obj.GetType()}");
            Log.L("Let's put the instance to pool");
            pool.Set(_obj);


        }
        protected override void Start()
        {
            //Log.L("--------------------------");
            //FastExample();
            //Log.L("--------------------------");
            //NormalTest();
            //Log.L("--------------------------");
            //MutiTest();
            var arr = Framework.GlobalAllocateArray<Obj_C>(10);
            Console.WriteLine(arr.Length);
            arr[0] = new Obj_C(15);
            arr.GlobalRecyle();
            arr = Framework.GlobalAllocateArray<Obj_C>(10);
            Console.WriteLine(arr[0].age);
            Console.WriteLine(1231111111111);
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
