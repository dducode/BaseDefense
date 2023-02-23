using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Zenject;

namespace BaseDefense
{
    ///<summary>Реализует простой пул для компонентов Unity</summary>
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
            MoveObjectToScene(value);
        }

        /// <summary>Извлекает объект из пула по имени</summary>
        /// <param name="name">Имя объекта, который нужно найти в пуле</param>
        /// <param name="obj">Объект, передаваемый вызывающему методу</param>
        /// <returns> Возвращает true, если объект найден. Иначе возвращает false </returns>
        public static bool Get(in string name, out Object obj)
        {
            foreach (var o in Pool)
                if (o.name == name)
                {
                    obj = o;
                    Pool.Remove(o);
                    SceneManager.MoveGameObjectToScene(obj.gameObject, SceneManager.GetActiveScene());
                    return true;
                }

            obj = null;
            return false;
        }

        ///<summary>Вспомогательный метод для сортировки игровых объектов в разные сцены</summary>
        ///<param name="obj">Объект, переносимый в сцену</param>
        public static void MoveObjectToScene(in Object obj)
        {
            foreach (var scene in Scenes)
            {
                if (!scene.name.Contains(obj.name)) continue;
                SceneManager.MoveGameObjectToScene(obj.gameObject, scene);
                return;
            }

            var newScene = SceneManager.CreateScene($"{obj.name} Pool");
            SceneManager.MoveGameObjectToScene(obj.gameObject, newScene);
            Scenes.Add(newScene);
        }
    }
}


