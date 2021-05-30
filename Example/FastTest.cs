using IFramework;
using IFramework.Fast;
using IFramework.Injection;
using IFramework.Modules.Message;

namespace Example
{
    public class FastTest : Test
    {
        class Ev02Entity : EnvironmentEntity<Ev02Entity>
        {
            protected Ev02Entity()
            {
                Log.L($"{envType}  {GetType()}---------->初始化");
            }
            protected override EnvironmentType envType => EnvironmentType.Ev1;
            public override void OnDispose()
            {
                Log.L($"{envType}  {GetType()}---------->销毁");
            }

        }

        private class TestModel : ObservableValue<string>, IModel
        {
            //public string _value = "313221";

            //public string value { get { return GetProperty(ref _value); } set { SetProperty(ref _value, value); } }
            public TestModel(string value) : base(value)
            {
            }
        }
        class TestSysEntity : SystemEntity<Ev02Entity>
        {
            protected override void Awake()
            {
                this.SetModel(new TestModel("313221"));
            }
        }
        class TestProcessor : Processor<TestSysEntity, Ev02Entity>
        {
            [Inject(nameof(TestSysEntity))] public TestModel model;

            public void Listen(IMessage message)
            {
                Log.L("收到一数据修改个事件------------------>" + message.args);

                if (message.args.Is<ChangeEvent>())
                {
                    ChangeEvent arg = message.args.As<ChangeEvent>();
                    Log.L($"修改model 的数据 to-------->{arg.value}");
                    model.value = arg.value;
                }
            }

            protected override void Awake()
            {
                Log.L("添加监听");
                this.SubscribeMessage<ChangeEvent>(this.Listen);
                Log.L("Awake------------------>" + GetType());
                Log.L($"所属环境-------------->{env.envType}");
                Log.L($"所属 SysEntity-------->{systemEntity}");
                Log.L($"注入的 model 数据----->{model.value}");
                string value = "第一次修改";
                Log.L($"修改 model 数据 to----->{value}");

                model.value = value;
                Log.L($"注入的 model 数据----->{model.value}");
            }
        }
        class ChangeEvent : IEventArgs
        {
            public string value;

            public ChangeEvent(string value)
            {
                this.value = value;
            }
        }
        class TestCmd : ICommand
        {
            public void Excute()
            {
                Log.L("Excute Command------------------>" + GetType());
            }
        }
        class TestView : View<TestSysEntity, Ev02Entity>
        {
            [Inject(nameof(TestSysEntity))] public TestModel model;
            protected override void Awake()
            {
                model.Subscribe(() => {
                    Log.L("通过数据绑定监听到变化 to------->" + model.value);
                });

                Log.L("");

                Log.L("Awake------------------>" + GetType());
                Log.L($"所属环境-------------->{env.envType}");
                Log.L($"所属 SysEntity-------->{systemEntity}");
                Log.L($"注入的 model 数据----->{model.value}");
                Log.L($"发送一个命令 TestCmd----->{model.value}");
                this.SendCommand(new TestCmd());
                Log.L("发送一数据修改个事件------------------>" + GetType());
                this.PublishMessage<ChangeEvent>(new ChangeEvent("第二次修改"), MessageUrgencyType.Common).OnRecycle((msag) =>
                {
                    Log.L("事件处理完成，来看看 model 的数据");
                    Log.L($"注入的 model 数据----->{model.value}");

                });
                this.PublishMessage<ChangeEvent>(new ChangeEvent("第三次修改"), MessageUrgencyType.VeryUrgent).OnRecycle((msag) =>
                {
                    Log.L("事件处理完成，来看看 model 的数据");
                    Log.L($"注入的 model 数据----->{model.value}");
                });
            }
        }

        protected override void Start()
        {
            Ev02Entity.Initialize();
            new TestSysEntity();
            new TestProcessor();
            new TestView();
        }

        protected override void Stop()
        {
            Ev02Entity.Destory();

        }

        protected override void Update()
        {
            Ev02Entity.Update();

        }
    }



}
