using System.Collections.Generic;
using System;

public static class BroadcastMessages
{
    readonly public static Dictionary<MessageType, Message> dict = new Dictionary<MessageType, Message>();

    public static void AddListener(MessageType message, Action listener)
    {
        if (!dict.ContainsKey(message))
            dict.Add(message, new Message());
        dict[message].AddListener(listener);
    }
    public static void RemoveListener(MessageType message, Action listener)
    {
        if (dict.ContainsKey(message))
            dict[message].RemoveListener(listener);
        if (dict[message].listeners.Count == 0)
            dict.Remove(message);
    }
    public static void SendMessage(MessageType message)
    {
        if (dict.ContainsKey(message))
            dict[message].SendMessage();
    }
}

public static class BroadcastMessages<T1>
{
    readonly public static Dictionary<MessageType, Message<T1>> dict = new Dictionary<MessageType, Message<T1>>();

    public static void AddListener(MessageType message, Action<T1> listener)
    {
        if (!dict.ContainsKey(message))
            dict.Add(message, new Message<T1>());
        dict[message].AddListener(listener);
    }
    public static void RemoveListener(MessageType message, Action<T1> listener)
    {
        if (dict.ContainsKey(message))
            dict[message].RemoveListener(listener);
        if (dict[message].listeners.Count == 0)
            dict.Remove(message);
    }
    public static void SendMessage(MessageType message, T1 a)
    {
        if (dict.ContainsKey(message))
            dict[message].SendMessage(a);
    }
}

public static class BroadcastMessages<T1, T2>
{
    readonly public static Dictionary<MessageType, Message<T1, T2>> dict = new Dictionary<MessageType, Message<T1, T2>>();

    public static void AddListener(MessageType message, Action<T1, T2> listener)
    {
        if (!dict.ContainsKey(message))
            dict.Add(message, new Message<T1, T2>());
        dict[message].AddListener(listener);
    }
    public static void RemoveListener(MessageType message, Action<T1, T2> listener)
    {
        if (dict.ContainsKey(message))
            dict[message].RemoveListener(listener);
        if (dict[message].listeners.Count == 0)
            dict.Remove(message);
    }
    public static void SendMessage(MessageType message, T1 a, T2 b)
    {
        if (dict.ContainsKey(message))
            dict[message].SendMessage(a, b);
    }
}

public static class BroadcastMessages<T1, T2, T3>
{
    readonly public static Dictionary<MessageType, Message<T1, T2, T3>> dict = new Dictionary<MessageType, Message<T1, T2, T3>>();

    public static void AddListener(MessageType message, Action<T1, T2, T3> listener)
    {
        if (!dict.ContainsKey(message))
            dict.Add(message, new Message<T1, T2, T3>());
        dict[message].AddListener(listener);
    }
    public static void RemoveListener(MessageType message, Action<T1, T2, T3> listener)
    {
        if (dict.ContainsKey(message))
            dict[message].RemoveListener(listener);
        if (dict[message].listeners.Count == 0)
            dict.Remove(message);
    }
    public static void SendMessage(MessageType message, T1 a, T2 b, T3 c)
    {
        if (dict.ContainsKey(message))
            dict[message].SendMessage(a, b, c);
    }
}

