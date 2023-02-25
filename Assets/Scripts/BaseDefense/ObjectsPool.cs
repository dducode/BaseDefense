using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Zenject;

namespace BaseDefense
{
    ///<summary>Реализует простой пул для объектов, наследуемых от BaseDefense.Object</summary>
    public static class ObjectsPool
    {
        private static readonly List<Object> Pool = new List<Object>();
        private static readonly List<Scene> Scenes = new List<Scene>();

        ///<summary>Добавляет объект в пул</summary>
        ///<param name="value">Объект, который добавляется в пул</param>
        public static void Push(in Object value)
        {
            Pool.Add(value);
            value.transform.SetParent(null);
            MoveObjectToHisScene(value);
        }

        /// <summary>Извлекает объект из пула</summary>
        /// <param name="targetObject">Искомый в пуле объект</param>
        /// <param name="foundObject">Найденный объект, передаваемый вызывающему методу</param>
        /// <returns> Возвращает true, если объект найден. Иначе возвращает false </returns>
        public static bool Get(in Object targetObject, out Object foundObject)
        {
            foreach (var obj in Pool)
                if (obj.Equals(targetObject))
                {
                    foundObject = obj;
                    Pool.Remove(obj);
                    SceneManager.MoveGameObjectToScene(foundObject.gameObject, SceneManager.GetActiveScene());
                    return true;
                }

            foundObject = null;
            return false;
        }

        ///<summary>Вспомогательный метод для сортировки игровых объектов в разные сцены</summary>
        ///<param name="obj">Объект, переносимый в сцену</param>
        public static void MoveObjectToHisScene(in Object obj)
        {
            foreach (var scene in Scenes)
            {
                if (scene.name.Contains(obj.name))
                {
                    SceneManager.MoveGameObjectToScene(obj.gameObject, scene);
                    return;
                }
            }

            var newScene = SceneManager.CreateScene($"{obj.name} Pool");
            SceneManager.MoveGameObjectToScene(obj.gameObject, newScene);
            Scenes.Add(newScene);
        }
    }
}


