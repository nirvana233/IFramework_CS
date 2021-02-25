using System.Collections;
using IFramework;

namespace Example
{
    public class CoroutineTest : Test
    {
        protected override void Start()
        {
            Framework.env0.modules.Coroutine.StartCoroutine(wait2());
        }
        IEnumerator wait()
        {
            Log.L("wait Go");

            yield return new IFramework.Modules.Coroutine.WaitForSeconds(2);
            Log.L("wait end");

        }
        IEnumerator wait1()
        {
            Log.L("wait1 Go");
            yield return wait();
            Log.L("wait1 end");

        }
        IEnumerator wait2()
        {
            Log.L("wait2 Go");
            yield return wait();
            Log.L("wait2 end");


            Log.L("wait2 Go");
            yield return wait1();
            Log.L("wait2 end");
        }
        protected override void Stop()
        {
        }

        protected override void Update()
        {
        }
    }
}
