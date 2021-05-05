using System;

namespace IFramework.Fast
{
    class A
    {
        class ABox : EnvironmentEntity<ABox>
        {
            protected ABox() { }
            protected override EnvironmentType envType => EnvironmentType.Ev0;


        }
        class Model : IModel
        {
            public string value = "313221";
        }
        class SE : SystemEntity<ABox>
        {
            protected override void Awake()
            {
                this.SetModel(new Model());
                this.SetModelProcessor(new MP());
            }


        }
        class MP : ModelProcessor<SE>
        {
            [Injection.Inject(nameof(SE))] public Model model;
            protected override void Awake()
            {
                Console.WriteLine("awake" + GetType());
                Console.WriteLine(this.env.envType);
                Console.WriteLine(model.value);
            }
        }
        class V : View<SE, ABox>
        {
            [Injection.Inject(nameof(SE))] public Model model;
            protected override void Awake()
            {
                Console.WriteLine("awake" + GetType());
                Console.WriteLine(this.env.envType);
                Console.WriteLine(model.value);
            }
        }
        static void Main(string[] args)
        {
            ABox.Initialize();
            new SE();
            new V();
            while (true)
            {
                ABox.Update();
            }
        }
    }
}
