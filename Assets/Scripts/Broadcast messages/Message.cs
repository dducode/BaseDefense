using System.Collections.Generic;
using System;

public class Message
{
    event Action message;
    readonly public List<Action> listeners = new List<Action>();

    public void AddListener(Action listener)
    {
        listeners.Add(listener);
        message += listener;
    }
    public void RemoveListener(Action listener)
    {
        listeners.Remove(listener);
        message -= listener;
    }
    public void SendMessage() => message?.Invoke();
}

public class Message<T1>
{
    event Action<T1> message;
    readonly public List<Action<T1>> listeners = new List<Action<T1>>();

    public void AddListener(Action<T1> listener)
    {
        listeners.Add(listener);
        message += listener;
    }
    public void RemoveListener(Action<T1> listener)
    {
        listeners.Remove(listener);
        message -= listener;
    }
    public void SendMessage(T1 a) => message?.Invoke(a);
}

public class Message<T1, T2>
{
    event Action<T1, T2> message;
    readonly public List<Action<T1, T2>> listeners = new List<Action<T1, T2>>();

    public void AddListener(Action<T1, T2> listener)
    {
        listeners.Add(listener);
        message += listener;
    }
    public void RemoveListener(Action<T1, T2> listener)
    {
        listeners.Remove(listener);
        message -= listener;
    }
    public void SendMessage(T1 a, T2 b) => message?.Invoke(a, b);
}

public class Message<T1, T2, T3>
{
    event Action<T1, T2, T3> message;
    readonly public List<Action<T1, T2, T3>> listeners = new List<Action<T1, T2, T3>>();

    public void AddListener(Action<T1, T2, T3> listener)
    {
        listeners.Add(listener);
        message += listener;
    }
    public void RemoveListener(Action<T1, T2, T3> listener)
    {
        listeners.Remove(listener);
        message -= listener;
    }
    public void SendMessage(T1 a, T2 b, T3 c) => message?.Invoke(a, b, c);
}

