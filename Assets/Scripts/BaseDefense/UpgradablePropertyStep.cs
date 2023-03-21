using UnityEngine;

namespace BaseDefense {

    [CreateAssetMenu(fileName = "Property Step", menuName = "Upgradable Property Step", order = 51)]
    public class UpgradablePropertyStep : ScriptableObject {

        [Tooltip("Значение текущего шага свойства")]
        public float value;

        [Tooltip("Текущий шаг свойства")]
        public int stepCount;

        [Tooltip("Цена за прокачку свойства до текущего значения")]
        public int price;

    }

}