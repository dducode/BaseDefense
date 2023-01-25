using System;

namespace BroadcastMessages
{
    ///<summary>Атрибут Listener добавляется к методу, который необходимо подписать на рассылку сообщений</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ListenerAttribute : Attribute
    {
        public MessageType MessageType { get; }
        public ListenerAttribute (MessageType messageType)
        {
            MessageType = messageType;
        }
    }
}

