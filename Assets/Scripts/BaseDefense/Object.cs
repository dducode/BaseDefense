using System;
using System.Collections;
using System.Linq;
using BaseDefense.Exceptions;
using BaseDefense.Properties;
using BaseDefense.SaveSystem;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Windows;
using Zenject;

namespace BaseDefense
{
    /// <summary>Базовый класс для всех игровых объектов</summary>
    [Icon("Assets/EditorUI/object.png")]
    public abstract class Object : MonoBehaviour
    {
        /// <inheritdoc cref="ObjectId"/>
        [Tooltip("Идентификатор объекта является уникальным только для объектов разных видов. " + 
                 "Объекты одного вида (напр. LowEnemy) имеют одинаковый id")]
        [SerializeField] private ObjectId objectId;

        public int Id => objectId.id;
        public bool IsDestroyed { get; private set; }
        
        /// <summary>
        /// Создаёт новый объект
        /// </summary>
        /// <param name="original">Префаб, из которого создаётся объект</param>
        /// <param name="position">Позиция объекта при создании</param>
        /// <param name="rotation">Ориентация объекта при создании</param>
        /// <param name="parent">Родительский transform создаваемого объекта</param>
        public static Object Create(
            in Object original, 
            in Vector3 position = default, 
            in Quaternion rotation = default,
            in Transform parent = null)
        {
            if (ObjectsPool.Get(original, out var obj))
            {
                obj.gameObject.SetActive(true);
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.transform.SetParent(parent);
                obj.IsDestroyed = false;
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
        /// Создаёт новый объект
        /// </summary>
        /// <param name="id">Идентификатор создаваемого объекта</param>
        /// <param name="position">Позиция объекта при создании</param>
        /// <param name="rotation">Ориентация объекта при создании</param>
        /// <param name="parent">Родительский transform создаваемого объекта</param>
        public static Object Create(
            int id, 
            Vector3 position = default, 
            Quaternion rotation = default, 
            Transform parent = null)
        {
            const string prefabsPath = "Assets/Prefabs";
            const string messageNotFoundDirectory = "Путь Assets/Prefabs не существует";
            Assert.IsTrue(Directory.Exists(prefabsPath), messageNotFoundDirectory);
            
            var prefabsGuids = AssetDatabase.FindAssets("t:Prefab", new []{ prefabsPath });

            foreach (var guid in prefabsGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var original = PrefabUtility.LoadPrefabContents(path);
                
                if (original.GetComponent<Object>() is { } originalObject && originalObject.Id != id)
                {
                    if (ObjectsPool.Get(originalObject, out var obj))
                    {
                        obj.gameObject.SetActive(true);
                        obj.transform.SetPositionAndRotation(position, rotation);
                        obj.transform.SetParent(parent);
                        obj.IsDestroyed = false;
                        return obj;
                    }
                    else
                    {
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
        public static Object CreateFromFactory<T>(
            in Object original,
            in PlaceholderFactory<UnityEngine.Object, T> factory,
            in Vector3 position = default, 
            in Quaternion rotation = default,
            in Transform parent = null) where T : Object
        {
            if (ObjectsPool.Get(original, out var obj))
            {
                obj.gameObject.SetActive(true);
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.transform.SetParent(parent);
                obj.IsDestroyed = false;
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
        /// Создаёт новый объект, используя фабрику
        /// </summary>
        /// <param name="id">Идентификатор создаваемого объекта</param>
        /// <param name="factory">Фабрика, с помощью которой создаётся объект</param>
        /// <param name="position">Позиция объекта при создании</param>
        /// <param name="rotation">Ориентация объекта при создании</param>
        /// <param name="parent">Родительский transform создаваемого объекта</param>
        /// <typeparam name="T">Тип создаваемого объекта</typeparam>
        public Object CreateFromFactory<T>(
            int id,
            PlaceholderFactory<UnityEngine.Object, T> factory,
            Vector3 position = default,
            Quaternion rotation = default,
            Transform parent = null) where T : Object
        {
            const string prefabsPath = "Assets/Prefabs";
            const string messageNotFoundDirectory = "Путь Assets/Prefabs не существует";
            Assert.IsTrue(Directory.Exists(prefabsPath), messageNotFoundDirectory);
            
            var prefabsGuids = AssetDatabase.FindAssets("t:Prefab", new []{ prefabsPath });

            foreach (var guid in prefabsGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var original = PrefabUtility.LoadPrefabContents(path);

                if (original.GetComponent<Object>() is { } originalObject && originalObject.Id == id)
                {
                    if (ObjectsPool.Get(originalObject, out var obj))
                    {
                        obj.gameObject.SetActive(true);
                        obj.transform.SetPositionAndRotation(position, rotation);
                        obj.transform.SetParent(parent);
                        obj.IsDestroyed = false;
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
            }
            
            const string messageIncorrectId = "Некорректный id";
            Debug.LogError(messageIncorrectId);
            return null;
        }
        
        /// <returns>Возвращает true, если объекты имеют одинаковый id, иначе возвращает false</returns>
        public override bool Equals(object other)
        {
            return other is Object otherObject && Id == otherObject.Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void Save(GameDataWriter writer)
        {
            writer.Write(transform.position);
            writer.Write(transform.rotation);
        }

        public virtual void Load(GameDataReader reader)
        {
            transform.position = reader.ReadPosition();
            transform.rotation = reader.ReadRotation();
        }

        /// <summary>
        /// Уничтожает объект
        /// </summary>
        public void Destroy()
        {
            gameObject.SetActive(false);
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            ObjectsPool.Push(this);
            IsDestroyed = true;
        }
        
        /// <summary>
        /// Уничтожает объект
        /// </summary>
        /// <param name="tweenTask">Анимация, которая должна проиграть перед уничтожением объекта</param>
        public void Destroy(Tween tweenTask)
        {
            tweenTask.OnComplete(Destroy);
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
            
            var message = 
                $"Попытка уничтожить объект {name} во время игры через UnityEngine.Object.Destroy. " +
                "Для уничтожения объекта используйте BaseDefense.Object.Destroy";
            throw new AttemptDestroyObjectException(message);
        }
    }
}
