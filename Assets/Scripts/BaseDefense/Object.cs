using System;
using System.Collections;
using BaseDefense.Exceptions;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace BaseDefense
{
    /// <summary>Базовый класс для всех игровых объектов</summary>
    public abstract class Object : MonoBehaviour
    {
        /// <summary>
        /// Создаёт новый объект
        /// </summary>
        /// <param name="original">Префаб, из которого создаётся объект</param>
        /// <param name="position">Позиция объекта при создании</param>
        /// <param name="rotation">Ориентация объекта при создании</param>
        /// <param name="parent">Родительский transform создаваемого объекта</param>
        /// <returns></returns>
        public static Object Create(
            in Object original, 
            in Vector3 position = default, 
            in Quaternion rotation = default,
            in Transform parent = null)
        {
            if (ObjectsPool.Get(original.name, out var obj))
            {
                obj.gameObject.SetActive(true);
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.transform.SetParent(parent);
                return obj;
            }
            else
            {
                var newObj = Instantiate(original, position, rotation, parent);
                newObj.name = original.name;
                return newObj;
            }
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
        /// <returns></returns>
        public static Object CreateFromFactory<T>(
            in Object original,
            in PlaceholderFactory<UnityEngine.Object, T> factory,
            in Vector3 position = default, 
            in Quaternion rotation = default,
            in Transform parent = null) where T : Object
        {
            if (ObjectsPool.Get(original.name, out var obj))
            {
                obj.gameObject.SetActive(true);
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.transform.SetParent(parent);
                return obj;
            }
            else
            {
                var newObj = factory.Create(original);
                newObj.name = original.name;
                newObj.transform.SetPositionAndRotation(position, rotation);
                newObj.transform.SetParent(parent);
                return newObj;
            }
        }

        /// <summary>
        /// Уничтожает объект
        /// </summary>
        public void Destroy()
        {
            gameObject.SetActive(false);
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            ObjectsPool.Push(this);
        }

        /// <summary>
        /// Уничтожает объект
        /// </summary>
        /// <param name="time">Время, спустя которое объект должен быть уничтожен</param>
        public void Destroy(float time)
        {
            StartCoroutine(Await(new WaitForSeconds(time), Destroy));
        }

        /// <summary>
        /// Уничтожает объект
        /// </summary>
        /// <param name="task">Задача, которая должна быть выполнена перед уничтожением объекта</param>
        public void Destroy(IEnumerator task)
        {
            StartCoroutine(Await(StartCoroutine(task), Destroy));
        }

        /// <summary>
        /// Уничтожает объект
        /// </summary>
        /// <param name="tweenTask">Анимация, которая должна проиграть перед уничтожением объекта</param>
        public void Destroy(Tween tweenTask)
        {
            tweenTask.OnComplete(Destroy);
        }

        private static IEnumerator Await(YieldInstruction task, Action callback)
        {
            yield return task;
            callback();
        }
        
        /*
         Разрешение на полное уничтожение объекта. Устанавливается только при выходе из игры,
         тем самым не будет выбрасываться исключение, когда Unity уничтожит объекты
        */ 
        private bool m_permissionDestroy;

        protected virtual void Awake()
        {
            Application.wantsToQuit += () => m_permissionDestroy = true;
        }

        private void OnDestroy()
        {
            if (m_permissionDestroy) return;
            
            const string message = 
                "Попытка уничтожить объект во время игры через UnityEngine.Object.Destroy. " +
                "Для уничтожения объекта используйте BaseDefense.Object.Destroy";
            throw new AttemptDestroyObjectException(message);
        }
    }
}
