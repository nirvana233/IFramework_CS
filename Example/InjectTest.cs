using IFramework;
using IFramework.Injection;

namespace Example
{
    public class InjectTest : Test
    {
        public interface ITestObject
        {
            void ToDo();
        }
        public class TestObject : ITestObject
        {
            public void ToDo()
            {
                Log.L("NNN");
            }
        }
        [Inject]
        public ITestObject _object;
        protected override void Start()
        {
            Framework.env0.container.Subscribe<ITestObject, TestObject>();

            //Framework.env0.container.SubscribeInstance<ITestObject>(new TestObject());
            Framework.env0.container.Inject(this);
            _object.ToDo();
        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
