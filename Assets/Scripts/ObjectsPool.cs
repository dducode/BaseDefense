using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Zenject;

///<summary>Реализует простой пул для компонентов MonoBehaviour</summary>
///<typeparam name="T">Тип объекта, добавляемого в пул</typeparam>
public static class ObjectsPool<T> where T : MonoBehaviour
{
    static List<T> pool = new List<T>();
    static Scene scene;

    ///<summary>Добавляет объект в пул</summary>
    ///<remarks>
    ///При добавлении объекта в пул автоматически отключается его GameObject и сбрасываются значения Transform
    ///</remarks>
    ///<param name="value">Объект, который добавляется в пул</param>
    public static void Push(T value)
    {
        value.gameObject.SetActive(false);
        value.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        pool.Add(value);
    }

    ///<summary>Извлекает объект из пула при его наличии. В ином случае создаёт новый</summary>
    ///<param name="prefab">Префаб объекта, который нужно создать, если пул пустой</param>
    public static T Get(T prefab)
    {
        if (pool.Count == 0)
            return CreateAndMoveObject(prefab);
        else
        {
            T value = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            value.gameObject.SetActive(true);
            return value;
        }
    }

    ///<summary>Извлекает объект, идентичный передаваемому в метод. Если такого нет - создаёт новый из фабрики</summary>
    ///<remarks>
    ///Объекты сравниваются по их именам. Предполагается, что идентичные объекты также имеют идентичные имена
    ///</remarks>
    ///<param name="factory">Фабрика, из которой создаётся объект</param>
    ///<param name="prefab">Префаб объекта, который нужно создать, если идентичный объект не найден</param>
    ///<param name="param">Параметр, который передаётся создаваемому объекту</param>
    public static T GetEqual<K>(PlaceholderFactory<Object, K, T> factory, T prefab, K param)
    {
        T value;
        foreach (T o in pool)
            if (o.name == prefab.name)
            {
                value = o;
                pool.Remove(o);
                value.gameObject.SetActive(true);
                return value;
            }

        value = factory.Create(prefab, param);
        value.gameObject.name = prefab.name;
        MoveObjectToScene(value);
        return value;
    }

    ///<summary>Вспомогательный метод для сортировки игровых объектов в разные сцены</summary>
    ///<remarks>Для каждого типа "Т" объекта создаётся своя сцена</remarks>
    ///<param name="gameObject">Игровой объект, переносимый в сцену</param>
    public static void MoveObjectToScene(T obj)
    {
        if (scene == default)
            scene = SceneManager.CreateScene($"{obj.GetType()} Scene");
        SceneManager.MoveGameObjectToScene(obj.gameObject, scene);
    }

    static T CreateAndMoveObject(T obj)
    {
        T value = Object.Instantiate(obj);
        value.gameObject.name = obj.name;
        MoveObjectToScene(value);
        return value;
    }
}
