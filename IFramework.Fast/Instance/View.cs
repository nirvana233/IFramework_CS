using System;
using IFramework.Injection;
using IFramework.Modules.Message;

namespace IFramework.Fast
{
    /// <summary>
    /// 界面
    /// </summary>
    /// <typeparam name="TSystemEntity"></typeparam>
    /// <typeparam name="TEnvironmentEntity"></typeparam>
    public abstract class View<TSystemEntity,TEnvironmentEntity>: Processor<TSystemEntity, TEnvironmentEntity>, ICanSendCommand
        where TEnvironmentEntity : EnvironmentEntity<TEnvironmentEntity>
        where TSystemEntity:ISystemEntity
    {
       
    }
}
