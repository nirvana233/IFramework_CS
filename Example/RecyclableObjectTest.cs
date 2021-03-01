using IFramework;

namespace Example
{
    public class RecyclableObjectTest : Test
    {
        public class MyObject : RecyclableObject
        {
            private int value;
            protected override void OnAllocate()
            {
                base.OnAllocate();
                Log.L("OnAllocate");


                Log.L("Let's  change some values");
                value = 10;
                SetDataDirty();
            }
            protected override void OnRecyle()
            {
                Log.L("OnRecyle");
                base.OnRecyle();

            }
            protected override void OnDispose()
            {
                Log.L("OnDispose");
            }
            protected override void OnDataReset()
            {
                Log.L("OnDataReset");
                value = 0;
            }
        }
        protected override void Start()
        {
            Log.L("Let's Allocate an instance");

            MyObject _object = MyObject.Allocate<MyObject>(EnvironmentType.Ev0);
            Log.L($" the instance id is  {_object.guid}");

            Log.L("Let's Recyle the instance");

            _object.Recyle();

            Log.L("Let's Allocate an instance   again");

            _object = MyObject.Allocate<MyObject>(EnvironmentType.Ev0);
            Log.L($" the instance id is  {_object.guid}");

        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
