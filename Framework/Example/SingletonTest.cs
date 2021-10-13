using IFramework;

namespace Example
{
    public class SingletonTest : Test
    {
        public class S : IFramework.Singleton.Singleton<S>
        {
            private S() {
                Log.L("Ctor");
            }
            protected override void OnSingletonInit()
            {
                Log.L("OnSingletonInit");
            }
            public void DO()
            {
                Log.L("DO");
            }

            protected override void OnDispose()
            {
                
            }
        }
        protected override void Start()
        {
            S.instance.DO();
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
