using UnityEngine;
using System.Collections.Generic;

public static class GameExtensions
{
    ///<summary>Форматирует целое число в строку и добавляет разделитель между числовыми классами</summary>
    ///<param name="separator">Разделитель, добавляемый в строку</param>
    public static string ToStringWithSeparator(this int target, char separator = ' ')
    {
        List<char> symbols = new List<char>(target.ToString());
        int initialCount = symbols.Count;
        for (int i = initialCount - 3; i > 0; i -= 3)
            symbols.Insert(i, separator);
        return string.Concat(symbols);
    }
}
