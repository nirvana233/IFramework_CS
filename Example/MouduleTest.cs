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
                Log.L("Awake");
            }
            protected override void OnEnable()
            {
                Log.L("OnEnable");

            }
            protected override void OnUpdate()
            {
                Log.L("OnUpdate");

            }
            protected override void OnDisable()
            {
                Log.L("OnDisable");

            }
            public void Say()
            {
                Log.L("Say");

            }
            protected override void OnDispose()
            {
                Log.L("OnDispose");

            }
        }
        protected override void Start()
        {
            Framework.env0.modules.CreateModule<MyModule>();
            var moudule= Framework.env0.modules.FindModule<MyModule>();
            moudule.Say();
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
