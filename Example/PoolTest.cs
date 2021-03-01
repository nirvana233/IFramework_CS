using IFramework;

namespace Example
{
    public class PoolTest : Test
    {
        public interface IObject { }
        public class Obj_A : IObject { }
        public class Obj_B : IObject { }

        private void FastExample()
        {
            Log.L("Create a auto create  pool");
            ActivatorCreatePool<Obj_A> pool = new ActivatorCreatePool<Obj_A>();
            Log.L("Get an instance from  pool");

            var _obj = pool.Get();
            Log.L($"the type of instance is {_obj.GetType()}");
            Log.L("Let's put the instance to pool");
            pool.Set(_obj);
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


        private void MutiTest()
        {
            Log.L("Create a auto create  pool");
            MutiPool pool = new MutiPool();
            Log.L("Get an instance  A   from  pool");

            IObject _obj = pool.Get<Obj_A>();
            Log.L($"the type of instance is {_obj.GetType()}");
            Log.L("Let's put the instance to pool");
            pool.Set(_obj);

            Log.L("Get an instance  B from  pool");

             _obj = pool.Get<Obj_B>();
            Log.L($"the type of instance is {_obj.GetType()}");
            Log.L("Let's put the instance to pool");
            pool.Set(_obj);


        }
        protected override void Start()
        {
              FastExample();
           // NormalTest();
           // MutiTest();
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
