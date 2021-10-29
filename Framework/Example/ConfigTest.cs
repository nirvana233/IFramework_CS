using IFramework;
using IFramework.Modules.Config;

namespace Example
{
    public class ConfigTest : Test
    {

        IConfigModule module { get { return Framework.GetEnv(EnvironmentType.Ev0).modules.Config; } }

        class Binder_A : BindableObject
        {
            private int _value;

            public int value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
        }
        const string value = "value";
        protected override void Start()
        {


            module.SetConfig<int>(100, value);
            Log.L($"Get Config  {module.GetConfig<int>(value)}");

            Binder_A a = new Binder_A();
            Log.L($"a.value =   {a.value}");


            Log.L($"Let's bind the config");

            module.BindConfig((value) => { a.value = value; }, () => { return a.value; });
            Log.L($"a.value =   {a.value}");


        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
