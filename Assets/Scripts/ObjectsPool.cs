using UnityEngine;
using System.Collections.Generic;

///<summary>Реализует простой пул для объектов</summary>
///<remarks>Рекомендуется проверка пула методом IsEmpty() перед извлечением объекта</remarks>
///<typeparam name="T">Тип объекта, добавляемого в пул</typeparam>
public static class ObjectsPool<T> where T : class
{
    static Stack<T> pool = new Stack<T>();

    ///<summary>Добавляет объект в пул</summary>
    ///<remarks>
    ///Если объект является компонентом MonoBehaviour, 
    ///то при добавлении в пул автоматически отключает прикреплённый GameObject
    ///</remarks>
    ///<param name="value">Объект, который добавляется в пул</param>
    public static void Push(T value)
    {
        if (value is MonoBehaviour mono)
            mono.gameObject.SetActive(false);
        pool.Push(value);
    }

    ///<summary>Извлекает объект из пула</summary>
    ///<remarks>
    ///Если объект является компонентом MoboBehaviour, 
    ///то при извлечении из пула автоматически включает прикреплённый GameObject
    ///</remarks>
    ///<returns>Возвращает null, если пул пустой или объект из пула уничтожен</returns>
    public static T Pop()
    {
        if (IsEmpty())
            return null;
        T value = pool.Pop();
        if (value is MonoBehaviour mono)
            mono.gameObject.SetActive(true);
        return value;
    }

    ///<summary>Проверяет пул на наличие объектов</summary>
    ///<returns>Возвращает true, если пул пустой или объект из пула уничтожен</returns>
    public static bool IsEmpty()
    {
        if (pool.Count == 0)
            return true;
        if (pool.Peek() == null || (pool.Peek() is MonoBehaviour mono && mono == null))
        {
            pool.Clear();
            return true;
        }
        return false;
    }
}
