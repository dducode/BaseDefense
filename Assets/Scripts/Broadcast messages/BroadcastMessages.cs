using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

namespace BroadcastMessages
{
    ///<summary>Мессенджер используется для рассылки сообщений всем подписавшимся на рассылку</summary>
    ///<remarks>
    ///Методы MonoBehaviour классов можно подписать автоматически с помощью атрибута Listener.
    ///Также на рассылку можно подписаться вручную, вызвав метод AddListener()
    ///</remarks>
    public static class Messenger
    {
        public readonly static Dictionary<MessageType, List<ListenerInfo>> dict = 
            new Dictionary<MessageType, List<ListenerInfo>>();

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
                        WriteInDict(listener.MessageType, new ListenerInfo(behaviour, method));
                }
            }
        }

        static void WriteInDict(MessageType message, ListenerInfo listenerInfo)
        {
            if (!dict.ContainsKey(message))
                dict.Add(message, new List<ListenerInfo>());
            dict[message].Add(listenerInfo);
        }

        ///<summary>Добавляет подписчика на рассылку сообщений</summary>
        ///<param name="message">Тип отправляемого сообщения</param>
        ///<param name="listener">Метод, который необходимо подписать</param>
        public static void AddListener(MessageType message, Action listener)
        {
            WriteInDict(message, new ListenerInfo(listener.Target, listener.Method));
        }

        ///<summary>Отправляет сообщение всем подписчикам</summary>
        ///<param name="message">Тип отправляемого сообщения</param>
        public static void SendMessage(MessageType message)
        {
            foreach (ListenerInfo listenerInfo in dict[message])
            {
                if (listenerInfo.method.GetParameters().Length == 0)
                    listenerInfo.method.Invoke(listenerInfo.instance, null);
            }
        }

        public struct ListenerInfo
        {
            public object instance;
            public MethodInfo method;

            public ListenerInfo(object instance, MethodInfo method)
            {
                this.instance = instance;
                this.method = method;
            }
        }
    }
}