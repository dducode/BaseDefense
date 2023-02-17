using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

namespace BroadcastMessages
{
    ///<summary>Мессенджер используется для рассылки сообщений всем подписавшимся на рассылку</summary>
    ///<remarks>
    ///Для подписки создаваемых в runtime объектов рекомендуется использовать AddListener метод.
    ///Для всех остальных подписку можно реализовать с использованием атрибута Listener
    ///</remarks>
    public static class Messenger
    {
        public readonly static Dictionary<MessageType, List<Action>> dict = 
            new Dictionary<MessageType, List<Action>>();

        [RuntimeInitializeOnLoadMethod]
        static void AddAllListeners()
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

        ///<summary>Добавляет подписчика на рассылку сообщений</summary>
        ///<param name="message">Тип отправляемого сообщения</param>
        ///<param name="listener">Подписчик, которого необходимо подписать</param>
        public static void AddListener(MessageType message, Action listener)
        {
            if (!dict.ContainsKey(message))
                dict.Add(message, new List<Action>());
            dict[message].Add(listener);
        }

        ///<summary>Удаляет подписчика с рассылки сообщений</summary>
        ///<param name="message">Тип отправляемого сообщения</param>
        ///<param name="listener">Подписчик, которого необходимо удалить</param>
        public static void RemoveListener(MessageType message, Action listener)
        {
            if (dict.ContainsKey(message))
                foreach (Action action in dict[message])
                    if (action == listener)
                    {
                        dict[message].Remove(action);
                        return;
                    }
        }

        ///<summary>Отправляет сообщение всем подписчикам</summary>
        ///<param name="message">Тип отправляемого сообщения</param>
        public static void SendMessage(MessageType message)
        {
            if (dict.ContainsKey(message))
            {
                List<Action> listeners = new List<Action>(dict[message]);
                foreach (Action listener in listeners)
                    listener.Invoke();
            }
        }
    }
}