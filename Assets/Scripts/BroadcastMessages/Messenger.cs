using System;
using System.Collections.Generic;
using BaseDefense.BroadcastMessages.Messages;
using UnityEngine;

namespace BaseDefense.BroadcastMessages {

    /// <summary>
    /// Мессенджер для рассылки <see cref="Message">сообщений</see> всем подписавшимся на рассылку
    /// </summary>
    public static class Messenger {

        #region Subscription

        /// <summary>
        /// Добавляет подписчика к рассылке
        /// </summary>
        /// <param name="subscriber">Подписчик с параметрами</param>
        /// <typeparam name="T">Тип сообщения, рассылаемого подписчикам</typeparam>
        public static void SubscribeTo<T> (Action<T> subscriber) where T : Message {
            Subscribers<T>.Container.Add(subscriber);
        }


        /// <summary>
        /// Добавляет подписчика к рассылке
        /// </summary>
        /// <param name="subscriber">Подписчик без параметров</param>
        /// <typeparam name="T">Тип сообщения, рассылаемого подписчикам</typeparam>
        public static void SubscribeTo<T> (Action subscriber) where T : Message {
            var messageType = typeof(T);
            if (!Subscribers.Container.ContainsKey(messageType))
                Subscribers.Container.Add(messageType, new List<Action>());
            Subscribers.Container[messageType].Add(subscriber);
        }

        #endregion



        #region Unsubscription

        /// <summary>
        /// Отписывает подписчика от рассылки
        /// </summary>
        /// <param name="subscriber">Подписчик с параметрами</param>
        /// <typeparam name="T">Тип сообщения, рассылаемого подписчикам</typeparam>
        public static void UnsubscribeFrom<T> (Action<T> subscriber) where T : Message {
            if (!Subscribers<T>.Container.Contains(subscriber)) {
                Debug.LogWarning($"Подписчик {subscriber.Method} не подписывался на сообщение {typeof(T)}");
                return;
            }

            Subscribers<T>.Container.Remove(subscriber);
        }


        /// <summary>
        /// Отписывает подписчика от рассылки
        /// </summary>
        /// <param name="subscriber">Подписчик без параметров</param>
        /// <typeparam name="T">Тип сообщения, рассылаемого подписчикам</typeparam>
        public static void UnsubscribeFrom<T> (Action subscriber) where T : Message {
            var messageType = typeof(T);

            if (!Subscribers.Container.ContainsKey(messageType)) {
                Debug.LogWarning($"На рассылку сообщения {messageType} никто не подписался");
                return;
            }

            if (!Subscribers.Container[messageType].Contains(subscriber)) {
                Debug.LogWarning($"Подписчик {subscriber.Method} не подписывался на сообщение {messageType}");
                return;
            }

            Subscribers.Container[messageType].Remove(subscriber);
        }

        #endregion



        #region Sending

        /// <summary>
        /// Рассылает сообщение всем подписчикам
        /// </summary>
        /// <param name="message"></param>
        public static void SendMessage<T> (T message) where T : Message {
            if (Subscribers<T>.Container.Count == 0) {
                SendLogInDebugConsole(typeof(T));

                return;
            }

            var subscribers = new List<Action<T>>(Subscribers<T>.Container);
            foreach (var subscriber in subscribers)
                subscriber.Invoke(message);
        }


        /// <summary>
        /// Рассылает сообщение всем подписчикам
        /// </summary>
        public static void SendMessage<T> () where T : Message {
            var messageType = typeof(T);

            if (!Subscribers.Container.ContainsKey(messageType)) {
                SendLogInDebugConsole(typeof(T));

                return;
            }

            if (Subscribers.Container[messageType].Count == 0) {
                SendLogInDebugConsole(typeof(T));

                return;
            }

            var subscribers = new List<Action>(Subscribers.Container[messageType]);
            foreach (var subscriber in subscribers)
                subscriber.Invoke();
        }


        private static void SendLogInDebugConsole (Type type) {
            var logMessage = $"Сообщение {type} было отправлено, но его никто не получил";
            Debug.LogWarning(logMessage);
        }

        #endregion



        /// <summary>
        /// Хранит в себе подписчиков с параметрами
        /// </summary>
        /// <typeparam name="T">Тип сообщения, отправляемый подписчикам</typeparam>
        private static class Subscribers<T> {

            public static readonly List<Action<T>> Container = new List<Action<T>>();

        }



        /// <summary>
        /// Хранит в себе подписчиков без параметров
        /// </summary>
        private static class Subscribers {

            public static readonly Dictionary<Type, List<Action>> Container =
                new Dictionary<Type, List<Action>>();

        }

    }

}