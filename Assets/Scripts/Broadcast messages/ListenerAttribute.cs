using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Method)]
public class ListenerAttribute : Attribute
{
    public MessageType MessageType { get; }
    public ListenerAttribute (MessageType messageType)
    {
        MessageType = messageType;
    }
}
