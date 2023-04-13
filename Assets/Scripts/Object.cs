using System;
using System.Collections;
using BaseDefense.Exceptions;
using BaseDefense.Properties;
using DG.Tweening;
using SaveSystem;
using UnityEngine;
using Zenject;

namespace BaseDefense {

    /// <summary>Базовый класс для всех игровых объектов</summary>
    [Icon("Assets/EditorUI/object.png")]
    public abstract class Object : MonoBehaviour, IPersistentObject {

        /// <inheritdoc cref="ObjectId"/>
        [Tooltip("Идентификатор объекта является уникальным только для объектов разных видов. " +
                 "Объекты одного вида (напр. LowEnemy) имеют одинаковый id")]
        [SerializeField]
        private ObjectId objectId;

        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        /// <remarks>
        /// Идентификатор объекта является уникальным только для объектов разных видов.
        /// Объекты одного вида (напр. LowEnemy) имеют одинаковый id
        /// </remarks>
        public int Id => objectId.id;
        
        /// <summary>
        /// Возвращает true, если объект уничтожен, иначе false
        /// </summary>
        /// <remarks>
        /// Объект считается уничтоженным, если он был добавлен в пул и отключён на сцене
        /// </remarks>
        public bool IsDestroyed { get; private set; }


        /// <summary>
        /// Создаёт новый объект
        /// </summary>
        /// <param name="original">Префаб, из которого создаётся объект</param>
        /// <param name="position">Позиция объекта при создании</param>
        /// <param name="rotation">Ориентация объекта при создании</param>
        /// <param name="parent">Родительский transform создаваемого объекта</param>
        public static T Create<T> (
            in T original,
            Transform parent = null,
            Vector3 position = default,
            Quaternion rotation = default
        ) where T : Object {
            if (ObjectsPool.Get(original, out var obj)) {
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.transform.SetParent(parent);
                obj.IsDestroyed = false;
                obj.gameObject.SetActive(true);

                return obj;
            }
            else {
                var newObj = Instantiate(original, position, rotation, parent);
                newObj.name = original.name;

                return newObj;
            }
        }


        /// <summary>
        /// Создаёт новый объект
        /// </summary>
        /// <param name="id">Идентификатор создаваемого объекта</param>
        /// <param name="position">Позиция объекта при создании</param>
        /// <param name="rotation">Ориентация объекта при создании</param>
        /// <param name="parent">Родительский transform создаваемого объекта</param>
        public static T Create<T> (
            int id,
            Transform parent = null,
            Vector3 position = default,
            Quaternion rotation = default
        ) where T : Object {
            foreach (var original in Resources.FindObjectsOfTypeAll<Object>()) {
                if (original.GetComponent<T>() is { } originalObject && originalObject.Id == id) {
                    if (ObjectsPool.Get(originalObject, out var obj)) {
                        obj.transform.SetPositionAndRotation(position, rotation);
                        obj.transform.SetParent(parent);
                        obj.IsDestroyed = false;
                        obj.gameObject.SetActive(true);

                        return obj;
                    }
                    else {
                        var newObj = Instantiate(originalObject, position, rotation, parent);
                        newObj.name = original.name;

                        return newObj;
                    }
                }
            }

            const string messageIncorrectId = "Некорректный id";
            Debug.LogError(messageIncorrectId);

            return null;
        }


        /// <summary>
        /// Создаёт новый объект, используя фабрику
        /// </summary>
        /// <param name="original">Префаб, из которого создаётся объект</param>
        /// <param name="factory">Фабрика, с помощью которой создаётся объект</param>
        /// <param name="position">Позиция объекта при создании</param>
        /// <param name="rotation">Ориентация объекта при создании</param>
        /// <param name="parent">Родительский transform создаваемого объекта</param>
        /// <typeparam name="T">Тип создаваемого объекта</typeparam>
        public static T CreateFromFactory<T> (
            in T original,
            in PlaceholderFactory<UnityEngine.Object, T> factory,
            Transform parent = null,
            Vector3 position = default,
            Quaternion rotation = default
        ) where T : Object {
            if (ObjectsPool.Get(original, out var obj)) {
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.transform.SetParent(parent);
                obj.IsDestroyed = false;
                obj.gameObject.SetActive(true);

                return obj;
            }
            else {
                var newObj = factory.Create(original);
                newObj.name = original.name;
                newObj.transform.SetPositionAndRotation(position, rotation);
                newObj.transform.SetParent(parent);

                return newObj;
            }
        }


        /// <summary>
        /// Создаёт новый объект, используя фабрику
        /// </summary>
        /// <param name="id">Идентификатор создаваемого объекта</param>
        /// <param name="factory">Фабрика, с помощью которой создаётся объект</param>
        /// <param name="position">Позиция объекта при создании</param>
        /// <param name="rotation">Ориентация объекта при создании</param>
        /// <param name="parent">Родительский transform создаваемого объекта</param>
        /// <typeparam name="T">Тип создаваемого объекта</typeparam>
        public static T CreateFromFactory<T> (
            int id,
            PlaceholderFactory<UnityEngine.Object, T> factory,
            Transform parent = null,
            Vector3 position = default,
            Quaternion rotation = default
        ) where T : Object {
            foreach (var original in Resources.FindObjectsOfTypeAll<Object>()) {
                if (original.GetComponent<T>() is { } originalObject && originalObject.Id == id) {
                    if (ObjectsPool.Get(originalObject, out var obj)) {
                        obj.transform.SetPositionAndRotation(position, rotation);
                        obj.transform.SetParent(parent);
                        obj.IsDestroyed = false;
                        obj.gameObject.SetActive(true);

                        return obj;
                    }
                    else {
                        var newObj = factory.Create(original);
                        newObj.name = original.name;
                        newObj.transform.SetPositionAndRotation(position, rotation);
                        newObj.transform.SetParent(parent);

                        return newObj;
                    }
                }
            }

            const string messageIncorrectId = "Некорректный id";
            Debug.LogError(messageIncorrectId);

            return null;
        }


        /// <returns>Возвращает true, если объекты имеют одинаковый id, иначе возвращает false</returns>
        public override bool Equals (object other) {
            return other is Object otherObject && Id == otherObject.Id;
        }


        public override int GetHashCode () {
            return base.GetHashCode();
        }


        /// <summary>
        /// Сохраняет данные объекта в файл
        /// </summary>
        /// <param name="writer"><see cref="UnityWriter"/></param>
        public virtual void Save (UnityWriter writer) {
            writer.Write(transform.position);
            writer.Write(transform.rotation);
        }


        /// <summary>
        /// Загружает данные объекта из файла
        /// </summary>
        /// <param name="reader"><see cref="UnityReader"/></param>
        public virtual void Load (UnityReader reader) {
            transform.position = reader.ReadPosition();
            transform.rotation = reader.ReadRotation();
        }


        /// <summary>
        /// Уничтожает объект
        /// </summary>
        public void Destroy () {
            if (IsDestroyed) {
                var message = $"Попытка повторного уничтожения объекта - объект {name} уже уничтожен";
                Debug.LogWarning(message);
                return;
            }

            gameObject.SetActive(false);
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.SetParent(null);
            ObjectsPool.Push(this);
            IsDestroyed = true;
        }


        /// <summary>
        /// Уничтожает объект
        /// </summary>
        /// <param name="tweenTask">Анимация, которая должна проиграть перед уничтожением объекта</param>
        public void Destroy (Tween tweenTask) {
            tweenTask.OnComplete(Destroy);
        }


        /// <summary>
        /// Уничтожает объект
        /// </summary>
        /// <param name="time">Время, спустя которое объект должен быть уничтожен</param>
        public void Destroy (float time) {
            StartCoroutine(Await(new WaitForSeconds(time), Destroy));
        }


        /// <summary>
        /// Уничтожает объект
        /// </summary>
        /// <param name="task">Задача, которая должна быть выполнена перед уничтожением объекта</param>
        public void Destroy (IEnumerator task) {
            StartCoroutine(Await(StartCoroutine(task), Destroy));
        }


        private static IEnumerator Await (YieldInstruction task, Action callback) {
            yield return task;
            callback();
        }


        /*
         Разрешение на полное уничтожение объекта. Устанавливается только при выходе из игры,
         тем самым не будет выбрасываться исключение, когда Unity уничтожит объекты
        */
        private bool m_permissionDestroy;


        protected virtual void Awake () {
            Application.wantsToQuit += () => m_permissionDestroy = true;
        }


        private void OnDestroy () {
            if (m_permissionDestroy) return;

            var message =
                $"Попытка уничтожить объект {name} во время игры через UnityEngine.Object.Destroy. " +
                "Для уничтожения объекта используйте BaseDefense.Object.Destroy";

            throw new AttemptDestroyObjectException(message);
        }

    }

}