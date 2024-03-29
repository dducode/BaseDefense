using UnityEngine;
using System.Collections.Generic;

namespace BaseDefense.Extensions {

    public static class GameExtensions {

        /// <summary>Форматирует целое число в строку и добавляет разделитель между числовыми классами</summary>
        /// <param name="target">Форматируемая строка</param>
        /// <param name="separator">Разделитель, добавляемый в строку</param>
        public static string ToStringWithSeparator (this int target, char separator = ' ') {
            var symbols = new List<char>(target.ToString());
            var initialCount = symbols.Count;
            for (var i = initialCount - 3; i > 0; i -= 3)
                symbols.Insert(i, separator);

            return string.Concat(symbols);
        }


        /// <summary>Задаёт линейную и угловую скорость тела</summary>
        /// <param name="rigidbody">Изменяемое физическое тело</param>
        /// <param name="velocity">Линейная скорость, устанавливаемая телу</param>
        /// <param name="angularVelocity">Угловая скорость, устанавливаемая телу</param>
        public static void SetVelocityAndAngularVelocity (
            this Rigidbody rigidbody, Vector3 velocity, Vector3 angularVelocity
        ) {
            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
        }

    }

}