using IFramework;

namespace Example
{
    public class MouduleTest : Test
    {
        private class MyModule : IFramework.Modules.UpdateFrameworkModule
        {
            public override int priority => 300;

            protected override void Awake()
            {
                Log.L("Awake "+guid);
            }
            protected override void OnEnable()
            {
                Log.L("OnEnable "+guid);

            }
            protected override void OnUpdate()
            {
                Log.L("OnUpdate "+guid);

            }
            protected override void OnDisable()
            {
                Log.L("OnDisable "+guid);

            }
            public void Say()
            {
                Log.L("Say "+guid);

            }
            protected override void OnDispose()
            {
                Log.L("OnDispose "+guid);

            }
        }
        protected override void Start()
        {
            Log.L("Create An module ");

            Framework.env0.modules.CreateModule<MyModule>();
            var module= Framework.env0.modules.FindModule<MyModule>();
            Log.L("the module id "+module.guid);
            Log.L("say ");

            module.Say();
            Log.L("Dispose ");
            module.UnBind();
            Log.L("say ");
            module.Say();

            Log.L("Get An other ");
            for (int i = 0; i < 10; i++)
            {
               Log.L(Framework.env0.modules.GetModule<MyModule>("XXL").guid);
            }
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
