using System;
namespace IFramework.Modules.Message
{
    public partial class MessageModule
    {
        private partial class HandlerQueue
        {
            private class DelegateSubscribe : IValueContainer<MessageListener>
            {
                public Type type;
                public MessageListener value { get; set; }
            }
        }
    }
}
