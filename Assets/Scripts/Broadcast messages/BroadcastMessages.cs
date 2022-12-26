using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

public static class BroadcastMessages
{
    readonly public static Dictionary<MessageType, Message> dict = new Dictionary<MessageType, Message>();

    [RuntimeInitializeOnLoadMethod]
    public static void AddAllListeners()
    {
        dict.Clear();
        MonoBehaviour[] behaviours = MonoBehaviour.FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            MethodInfo[] methods = behaviour.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );
            foreach (MethodInfo method in methods)
            {
                if (method.GetParameters().Length == 0)
                {
                    ListenerAttribute listener = Attribute.GetCustomAttribute(
                        method, typeof(ListenerAttribute)) as ListenerAttribute;
                    if (listener != null)
                    {
                        Action action = Delegate.CreateDelegate(typeof(Action), behaviour, method) as Action;
                        AddListener(listener.MessageType, action);
                    }
                }
            }
        }
    }

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

    [RuntimeInitializeOnLoadMethod]
    public static void AddAllListeners()
    {
        dict.Clear();
        MonoBehaviour[] behaviours = MonoBehaviour.FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            MethodInfo[] methods = behaviour.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            foreach (MethodInfo method in methods)
            {
                if (method.GetParameters().Length == 1)
                {
                    ListenerAttribute listener = Attribute.GetCustomAttribute(
                        method, typeof(ListenerAttribute)) as ListenerAttribute;
                    if (listener != null)
                    {
                        Action<T1> action = Delegate.CreateDelegate(
                            typeof(Action<T1>), behaviour, method) as Action<T1>;
                        AddListener(listener.MessageType, action);
                    }
                }
            }
        }
    }

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

    [RuntimeInitializeOnLoadMethod]
    public static void AddAllListeners()
    {
        dict.Clear();
        MonoBehaviour[] behaviours = MonoBehaviour.FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            MethodInfo[] methods = behaviour.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );
            foreach (MethodInfo method in methods)
            {
                if (method.GetParameters().Length == 2)
                {
                    ListenerAttribute listener = Attribute.GetCustomAttribute(
                        method, typeof(ListenerAttribute)) as ListenerAttribute;
                    if (listener != null)
                    {
                        Action<T1, T2> action = Delegate.CreateDelegate(
                            typeof(Action<T1, T2>), behaviour, method) as Action<T1, T2>;
                        AddListener(listener.MessageType, action);
                    }
                }
            }
        }
    }

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

    [RuntimeInitializeOnLoadMethod]
    public static void AddAllListeners()
    {
        dict.Clear();
        MonoBehaviour[] behaviours = MonoBehaviour.FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            MethodInfo[] methods = behaviour.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );
            foreach (MethodInfo method in methods)
            {
                if (method.GetParameters().Length == 3)
                {
                    ListenerAttribute listener = Attribute.GetCustomAttribute(
                        method, typeof(ListenerAttribute)) as ListenerAttribute;
                    if (listener != null)
                    {
                        Action<T1, T2, T3> action = Delegate.CreateDelegate(
                            typeof(Action<T1, T2, T3>), behaviour, method) as Action<T1, T2, T3>;
                        AddListener(listener.MessageType, action);
                    }
                }
            }
        }
    }

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

