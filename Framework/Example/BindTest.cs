using IFramework;

namespace Example
{
    //数据绑定示例
    public class BindTest : Test
    {
        /// <summary>
        /// 单向绑定对象，继承ObservableObject
        /// </summary>
        class Observable_A : ObservableObject
        {
            private int _value;

            public int value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
        }



        class Binder_A : BindableObject
        {
            private int _value;

            public int value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
        }
        class Binder_B : BindableObject
        {
            private int _value;

            public int value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
        }

        class Binder_C : BindableObject
        {
            private int _value;

            public int value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
        }

        /// <summary>
        /// 单向/数据变化监听
        /// </summary>
        private void Observable_Test()
        {
            Log.L("——————————————————————————");
            Log.L("单向绑定监听数据变化示例");
            Log.L("需要新建一个ObservableObjectHandler对象用于对继承了ObservableObject类的对象变化进行监听");
            Log.L("新建对象binder和被观察的对象observedA");
            ObservableObjectHandler binder = new ObservableObjectHandler();
            Observable_A observedA = new Observable_A();
            Log.L($"observedA当前的value值为{observedA.value} \n");

            Log.L("开始绑定操作，参数getter和setter将会被调用一次");

            binder.BindProperty(
                                (value) =>
                                {
                                    observedA.value = value;
                                    Log.L($"值被改变，新值为{value}");
                                },
                                () =>
                                {
                                    return observedA.value;
                                }
                        );

            Log.L($"observedA当前的value值为{observedA.value} \n");

            Log.L("将observedA的value值改为1");
            observedA.value = 1;
            Log.L($"observedA当前的value值为{observedA.value} \n");


            Log.L("将observedA的value值改为2");
            observedA.value = 2;
            Log.L($"observedA当前的value值为{observedA.value} \n");
        }

        /// <summary>
        /// 双向/多向/数值同步变化
        /// </summary>
        private void Bind_Test()
        {

            Log.L("——————————————————————————");
            Log.L("多向绑定监听数据变化示例");
            BindableObjectHandler binder = new BindableObjectHandler();
            Binder_A a = new Binder_A();
            Binder_B b = new Binder_B();
            Binder_C c = new Binder_C();

            Log.L("将三个对象的value值 绑定监听");

            binder.BindProperty((value) => { a.value = value; }, () => { return a.value; });
            binder.BindProperty((value) => { b.value = value; }, () => { return b.value; });
            binder.BindProperty((value) => { c.value = value; }, () => { return c.value; });

            Log.L($"a的value值为{a.value}\tb的value值为{b.value}\tc的value值为{c.value}\n");
            Log.L("将a的value值改为1");
            a.value = 1;
            Log.L($"a的value值为{a.value}\tb的value值为{b.value}\tc的value值为{c.value}\n");

            Log.L("将c的value值改为2");
            c.value = 2;
            Log.L($"a的value值为{a.value}\tb的value值为{b.value}\tc的value值为{c.value}\n");

            Log.L("将b解绑，再将abc的值分别改为3、4、5(按对象解绑)");



            binder.UnBind();
            Log.L($"a的value值为{a.value}\tb的value值为{b.value}\tc的value值为{c.value}\n");
            a.value = 3;
            b.value = 4;
            c.value = 5;
            Log.L($"a的value值为{a.value}\tb的value值为{b.value}\tc的value值为{c.value}\n");
        }

        protected override void Start()
        {
            //单向绑定
            Observable_Test();
            //多向绑定
            Bind_Test();
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
