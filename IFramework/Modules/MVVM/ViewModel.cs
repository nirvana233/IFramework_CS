using IFramework.Modules.Message;
namespace IFramework.Modules.MVVM
{
    internal interface IViewModel
    {
        void SubscribeMessage();
        void UnSubscribeMessage();
        void SyncModelValue();
    }
    /// <summary>
    /// VM
    /// </summary>
    [FrameworkVersion(12)]
    public abstract class ViewModel : ObservableObject, IViewModel
    {
        internal MVVMGroup group { get; set; }
        /// <summary>
        /// 消息转发
        /// </summary>
        protected MessageModule message { get { return group.message; } }
        /// <summary>
        /// 数据
        /// </summary>
        public IDataModel model { get { return group.model; } }

        void IViewModel.SubscribeMessage()
        {
            SyncModelValue();
            SubscribeMessage();
        }
        void IViewModel.UnSubscribeMessage()
        {
            UnSubscribeMessage();
        }
        void IViewModel.SyncModelValue()
        {
            SyncModelValue();
        }
        /// <summary>
        /// 同步model数据
        /// </summary>
        protected abstract void SyncModelValue();
        /// <summary>
        /// 注册消息监听
        /// </summary>
        protected virtual void SubscribeMessage() { }
        /// <summary>
        /// 取消消息监听
        /// </summary>
        protected virtual void UnSubscribeMessage() { }

        /// <summary>
        /// 释放时
        /// </summary>
        protected override void OnDispose() { }

    }
    /// <summary>
    /// 方便书写
    /// </summary>
    public abstract class TViewModel<T> : ViewModel where T : IDataModel
    {
        /// <summary>
        /// 方便书写
        /// </summary>
        public T Tmodel { get { return (T)group.model; } }

    }

    //public class TestViewModel : TViewModel<data>
    //{
    //    private int _value = 100;
    //    public string name;
    //    public int value
    //    {
    //        get { return GetProperty(ref _value, this.GetPropertyName(() => _value)); }
    //        private set
    //        {
    //            Tmodel.value = value;
    //            SetProperty(ref _value, value, this.GetPropertyName(() => _value));
    //        }
    //    }
    //    protected override void SyncModelValue()
    //    {
    //        this.value = Tmodel.value;
    //    }
    //    protected override void SubscribeMessage()
    //    {
    //        this.message.Subscribe<TestView>(Listen);
    //    }
    //    protected override void UnSubscribeMessage()
    //    {
    //        this.message.Unsubscribe<TestView>(Listen);
    //    }
    //    private void Listen(Type publishType, int code, IEventArgs args, object[] param)
    //    {
    //        Console.WriteLine(name + "  get Message   " + code);
    //        value = code;
    //    }
    //}
    //class TestView : TView<TestViewModel>, IMessagePublisher
    //{
    //    public int value;
    //    protected override void BindProperty()
    //    {
    //        base.BindProperty();
    //        handler.BindProperty(() => { this.value = Tcontext.value; });
    //    }

    //    public void ChangeValue(int value)
    //    {
    //        this.message.Publish<TestView>(value, null);
    //    }
    //}
    //public class data : IDataModel
    //{
    //    public int value;
    //}
    //class Test
    //{

    //    static void Main(string[] args)
    //    {
    //        MVVMModule mo = MVVMModule.CreatInstance<MVVMModule>("test", "txt");
    //        MVVMGroup group = new MVVMGroup("test",
    //            new TestView(),
    //            new TestViewModel() { name = "VM 01" },
    //            new data()
    //            );
    //        mo.AddGroup(group);



    //        Console.WriteLine(string.Format("VM Value : {0}     V Value : {1}  M Value {2}", (group.viewModel as TestViewModel).value, (group.view as TestView).value, (group.model as data).value));
    //        (group.view as TestView).ChangeValue(18);
    //        Console.WriteLine(string.Format("VM Value : {0}     V Value : {1}  M Value {2}", (group.viewModel as TestViewModel).value, (group.view as TestView).value, (group.model as data).value));
    //        (group.view as TestView).ChangeValue(123);
    //        Console.WriteLine(string.Format("VM Value : {0}     V Value : {1}  M Value {2}", (group.viewModel as TestViewModel).value, (group.view as TestView).value, (group.model as data).value));
    //        Console.WriteLine();
    //        Console.WriteLine("Change VM 01    To    VM 02");
    //        group.viewModel = new TestViewModel() { name = "VM 02" };
    //        Console.WriteLine(string.Format("VM Value : {0}     V Value : {1}  M Value {2}", (group.viewModel as TestViewModel).value, (group.view as TestView).value, (group.model as data).value));
    //        (group.view as TestView).ChangeValue(222);
    //        Console.WriteLine(string.Format("VM Value : {0}     V Value : {1}  M Value {2}", (group.viewModel as TestViewModel).value, (group.view as TestView).value, (group.model as data).value));


    //        while (true)
    //        {
    //            // mo.Update();
    //        }
    //    }
    //}
}
