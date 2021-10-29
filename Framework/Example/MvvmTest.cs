using IFramework;
using IFramework.Modules.Message;
using IFramework.Modules.MVVM;

namespace Example
{
    public class MvvmTest : Test
    {
        class MyModel : IModel
        {
            public int value;
        }
        class MyViewModel : ViewModel<MyModel>
        {
            private int _value;
            public int value { get { return GetProperty(ref _value); } set {

                    Tmodel.value = value;
                    SetProperty(ref _value, value); } }

            protected override void SyncModelValue()
            {
                value = Tmodel.value;
                this.value++;
            }
            protected override void SubscribeMessage()
            {
                message.Subscribe<MyView>(Listen);
            }

            private void Listen(IMessage message)
            {
                Log.E("Message rec");
            }
        }
        class MyView : View<MyViewModel>
        {
            protected override void BindProperty()
            {
                base.BindProperty();
                this.handler.BindProperty(() => { var value = Tcontext.value;

                    Log.E(value);
                });
                Log.E("Message pub");

            }
            public override void OnSetMessage()
            {
                message.Publish<MyView>(null);
            }
        }
        protected override void Start()
        {
            Framework.GetEnv(EnvironmentType.Ev0).modules.MVVM
                .AddGroup(new MVVMGroup("name", new MyView(), new MyViewModel(), new MyModel()));

        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }



}
