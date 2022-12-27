using UnityEngine;
using System.Collections.Generic;

///<summary>
///Реализует простой пул для объектов, наследуемых от MonoBehaviour
///</summary>
///<remarks>
///Рекомендуется проверка стека методом IsEmpty() перед извлечением объекта
///</remarks>
public static class ObjectsPool<T> where T : MonoBehaviour
{
    static Stack<T> objects = new Stack<T>();

    ///<summary>
    ///При добавлении в стек автоматически отключает GameObject
    ///</summary>
    public static void Push(T value)
    {
        value.gameObject.SetActive(false);
        objects.Push(value);
    }

    ///<summary>
    ///При извлечении из стека автоматически включает GameObject
    ///</summary>
    ///<returns>Возвращает null, если стек пустой или объект из стека уничтожен</returns>
    public static T Pop()
    {
        if (IsEmpty())
            return null;
        T value = objects.Pop();
        value.gameObject.SetActive(true);
        return value;
    }

    ///<summary>
    ///Проверяет стек на наличие объектов
    ///</summary>
    ///<returns>Возвращает true, если стек пустой или объект из стека уничтожен</returns>
    public static bool IsEmpty()
    {
        if (objects.Count == 0 || objects.Peek() == null)
        {
            objects = new Stack<T>();
            return true;
        }
        return false;
    }
}
