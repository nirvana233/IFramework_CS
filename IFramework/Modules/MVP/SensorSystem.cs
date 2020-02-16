using IFramework.Modules.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFramework.Modules.MVP
{
    /// <summary>
    /// 消息探头
    /// </summary>
    public abstract class SensorSystem : MVPSystem
    {
        protected SensorSystem() : base() { }
        protected override void OnSetMessage(MessageModule message) { }

        protected void SendSensor(int code, IEventArgs args, params object[] param)
        {
            SendMessage(typeof(SensorSystem), code, args, param);
        }
        protected override void Excute() { }
    }

}
