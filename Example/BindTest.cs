using IFramework;

namespace Example
{
    public class BindTest : Test
    {

        class Binder_A:BindableObject
        {
            private string _value;

            public string value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
        }
        class Binder_B : BindableObject
        {
            private string _value;

            public string value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
        }


        private void Bind_Test()
        {

            Log.L("init  BindableObject Test");
            BindableObjectHandler binder = new BindableObjectHandler();
            Binder_A a = new Binder_A();
            Binder_B b = new Binder_B();
            Log.L($"a.value = {a.value}\tb.value= {b.value}");
            Log.L("bind");

            binder.BindProperty((value) => { a.value = value; }, () => { return a.value; })
                .BindProperty((value) => { b.value = value; }, () => { return b.value; });
            Log.L($"a.value = {a.value}\tb.value= {b.value}");
            Log.L("change value to a");
            a.value = "a";
            Log.L($"a.value = {a.value}\tb.value= {b.value}");


            Log.L("change value to b");
            b.value = "b";
            Log.L($"a.value = {a.value}\tb.value= {b.value}");
        }
        protected override void Start()
        {
            //单向
            Observable_Test();



            //双向
           // Bind_Test();
        }

        private void Observable_Test()
        {
            Log.L("init ObservableObject Test");
            ObservableObjectHandler binder = new ObservableObjectHandler();
            Observable_A a = new Observable_A();
            Log.L($"a.value = {a.value}");
            Log.L("bind");

            binder.BindProperty(
                                (value) => {
                                    a.value = value;
                                    Log.L($"listen change a.value to   {value }");
                                },
                                () => { return a.value; }
                        );

            Log.L($"a.value = {a.value}");

            Log.L("change value to a");
            a.value = "a";
            Log.L($"a.value = {a.value}");


            Log.L("change value to b");
            a.value = "b";
            Log.L($"a.value = {a.value}");
        }
        class Observable_A: ObservableObject
        {
            private string _value;

            public string value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
        }
        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
