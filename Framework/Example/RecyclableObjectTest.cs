using IFramework;

namespace Example
{
    //可回收对象使用示例
    public class RecyclableObjectTest : Test
    {
        public class MyObject : RecyclableObject
        {
            private int value;
            protected override void OnAllocate()
            {
                base.OnAllocate();
                Log.L("*OnAllocate");
                Log.L("将value值更改为10，由于数据变动，设置dirty标记");
                Log.L("如果没有设置，在回收的时候不会调用OnDataReset");
                value = 10;
                SetDataDirty();
            }
            protected override void OnRecyle()
            {
                Log.L("*OnRecyle");
                base.OnRecyle();

            }
            protected override void OnDispose()
            {
                Log.L("*OnDispose");
            }
            protected override void OnDataReset()
            {
                Log.L("*OnDataReset");
                Log.L("将value值更改为默认值（也就是0）");
                value = default;
            }

            /// <summary>
            /// 输出Value的值
            /// </summary>
            public void PrintValue()
            {
                Log.L($"value的值为{value}");
            }
        }


        protected override void Start()
        {
            Log.L("");
            Log.L("使用Allocate方法分配一个实例");
            MyObject _object = MyObject.Allocate<MyObject>(EnvironmentType.Ev0);
            Log.L($"当前实例的GUID为 {_object.guid}");
            _object.PrintValue();
            Log.L("将这个实例回收掉");

            _object.Recyle();

            Log.L("重新获取一个新的");

            _object = MyObject.Allocate<MyObject>(EnvironmentType.Ev0);
            Log.L($"当前实例的GUID为 {_object.guid}");
            Log.L("可以发现GUID是一样的，获取的是同一个实例");

        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
