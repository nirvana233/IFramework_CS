using System;
using IFramework;
using IFramework.Modules.Recorder;

namespace Example
{
    public class RecorderTest : Test
    {
        IOperationRecorderModule module { get { return Framework.env0.modules.Recoder; } }
       int value = 0;
        protected override void Start()
        {
            Log.L("按下  A/D   切换状态");

            Log.L($"The value is   {value}");
            module.AllocateAction().SetCommand(() => { value++; }, () => { value--; }).Subscribe();
            Log.L($"The value is   {value}");
            module.AllocateAction().SetCommand(() => { value+=4; }, () => { value-=4; }).Subscribe();
            Log.L($"The value is   {value}");
            module.AllocateAction().SetCommand(() => { value += 8; }, () => { value -= 8; }).Subscribe();
            Log.L($"The value is   {value}");

        }

        protected override void Stop()
        {
        }

        protected override void Update()
        {
            if (Console.ReadKey().Key == ConsoleKey.A)
            {
                bool bo = module.Undo();
                Log.L("");

                Log.L($"Undo  success {bo}");
                Log.L($"The value is   {value}");
            }
            if (Console.ReadKey().Key == ConsoleKey.D)
            {
                bool bo = module.Redo();
                Log.L("");

                Log.L($"Redo sucess   {bo}");
                Log.L($"The value is   {value}");
            }
        }
    }
}
