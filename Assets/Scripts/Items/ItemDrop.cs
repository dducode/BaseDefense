using BaseDefense.Properties;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace BaseDefense.Items {

    public class ItemDrop : MonoBehaviour {

        ///<summary>Предмет, выпадаемый с объекта после уничтожения</summary>
        [Tooltip("Предмет, выпадаемый с объекта после уничтожения")]
        [SerializeField]
        private Item itemPrefab;

        ///<summary>Сила, с которой выпадают предметы</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Сила, с которой выпадают предметы. [0, infinity]")]
        [SerializeField, Min(0)]
        private float forceScalar;

        ///<summary>Количество выпадаемых предметов в определённом диапазоне</summary>
        [Tooltip("Количество выпадаемых предметов в определённом диапазоне")]
        [SerializeField]
        private MinMaxSliderInt itemsCountRange = new MinMaxSliderInt(0, 100);


        ///<summary>Вызывается для выброса предметов</summary>
        ///<remarks>Количество выпадаемых предметов выбирается случайным образом</remarks>
        public void DropItems () {
            var itemsCount = Random.Range(itemsCountRange.minValue, itemsCountRange.maxValue + 1);

            for (var i = 0; i < itemsCount; i++) {
                var position = transform.position + Vector3.up;
                var rotation = Random.rotation;
                var item = Object.Create(itemPrefab, null, position, rotation);
                var force = new Vector3(Random.Range(0f, 1f), 1, Random.Range(0f, 1f)) * forceScalar;

                item.Drop(force);
            }
        }

    }

}