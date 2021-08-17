using System;
using IFramework;
using IFramework.NodeAction;

namespace Example
{
    public class NodeActionTest : Test
    {
        protected override void Start()
        {
            Dosth();
        }
        void Dosth()
        {

            this.Sequence(EnvironmentType.Ev0)
                .Repeat((r) => {
                    r.Sequence((s) =>
                    {
                        s.TimeSpan(TimeSpan.FromSeconds(0.2f))
                         .Event(() => { Log.L("Event"); })
                         .OnCompelete(() => { Log.L("Inner OnCompelete"); })
                         .OnBegin(() => { Log.L("Inner OnBegin"); });
                    })
                    ;
                }, 2)
                .TimeSpan(new TimeSpan(0, 0, 1))
                .OnCompelete((ss) => { Log.L("OnCompelete"); })
                .OnBegin((ss) => { Log.L("OnBegin"); })
                .OnRecyle(() => { Log.L("OnRecyle"); })
                .Run();
        }
        protected override void Stop()
        {
        }

        protected override void Update()
        {
            if (Console.ReadKey().Key== ConsoleKey.Spacebar)
            {
                Dosth();
            }
        }
    }
}
