using System;
using System.Collections.Generic;

namespace BaseDefense.BroadcastMessages
{
    ///<summary>Мессенджер используется для рассылки сообщений всем подписавшимся на рассылку</summary>
    public static class Messenger
    {
        public static readonly Dictionary<MessageType, List<Action>> Dict = 
            new Dictionary<MessageType, List<Action>>();

        ///<summary>Добавляет подписчика на рассылку сообщений</summary>
        ///<param name="message">Тип отправляемого сообщения</param>
        ///<param name="listener">Подписчик, которого необходимо подписать</param>
        public static void AddListener(MessageType message, Action listener)
        {
            if (!Dict.ContainsKey(message))
                Dict.Add(message, new List<Action>());
            Dict[message].Add(listener);
        }

        ///<summary>Удаляет подписчика с рассылки сообщений</summary>
        ///<param name="message">Тип отправляемого сообщения</param>
        ///<param name="listener">Подписчик, которого необходимо удалить</param>
        public static void RemoveListener(MessageType message, Action listener)
        {
            if (!Dict.ContainsKey(message)) return;

            Dict[message].Remove(listener);
        }

        ///<summary>Отправляет сообщение всем подписчикам</summary>
        ///<param name="message">Тип отправляемого сообщения</param>
        public static void SendMessage(MessageType message)
        {
            if (!Dict.ContainsKey(message)) return;
            
            var listeners = new List<Action>(Dict[message]);
            foreach (var listener in listeners)
                listener.Invoke();
        }
    }
    
    public static class Messenger<T>
    {
        public static readonly Dictionary<MessageType, List<Action<T>>> Dict = 
            new Dictionary<MessageType, List<Action<T>>>();

        ///<summary>Добавляет подписчика на рассылку сообщений</summary>
        ///<param name="message">Тип отправляемого сообщения</param>
        ///<param name="listener">Подписчик, которого необходимо подписать</param>
        public static void AddListener(MessageType message, Action<T> listener)
        {
            if (!Dict.ContainsKey(message))
                Dict.Add(message, new List<Action<T>>());
            Dict[message].Add(listener);
        }

        ///<summary>Удаляет подписчика с рассылки сообщений</summary>
        ///<param name="message">Тип отправляемого сообщения</param>
        ///<param name="listener">Подписчик, которого необходимо удалить</param>
        public static void RemoveListener(MessageType message, Action<T> listener)
        {
            if (!Dict.ContainsKey(message)) return;

            Dict[message].Remove(listener);
        }

        /// <summary>Отправляет сообщение всем подписчикам</summary>
        /// <param name="message">Тип отправляемого сообщения</param>
        /// <param name="param">Параметр, отправляемый подписчикам вместе с сообщением</param>
        public static void SendMessage(MessageType message, T param)
        {
            if (!Dict.ContainsKey(message)) return;
            
            var listeners = new List<Action<T>>(Dict[message]);
            foreach (var listener in listeners)
                listener.Invoke(param);
        }
    }
}