using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    ///<summary>Предмет, выпадаемый с объекта после уничтожения</summary>
    [Tooltip("Предмет, выпадаемый с объекта после уничтожения")]
    [SerializeField] Item itemPrefab;

    ///<summary>Сила, с которой выпадают предметы</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Сила, с которой выпадают предметы. [0, infinity]")]
    [SerializeField, Min(0)] float forceScalar;

    ///<summary>Количество выпадаемых предметов в определённом диапазоне</summary>
    [Tooltip("Количество выпадаемых предметов в определённом диапазоне")]
    [SerializeField] MinMaxSliderInt itemsCount = new MinMaxSliderInt(0, 100);

    ///<summary>Вызывается для выброса предметов</summary>
    ///<remarks>Количество выпадаемых предметов выбирается случайным образом</remarks>
    public void DropItems()
    {
        int itemsCount = Random.Range(this.itemsCount.minValue, this.itemsCount.maxValue + 1);
        for (int i = 0; i < itemsCount; i++)
        {
            Item item;
            if (itemPrefab is Money)
                item = ObjectsPool<Money>.Get(itemPrefab as Money);
            else if (itemPrefab is Gem)
                item = ObjectsPool<Gem>.Get(itemPrefab as Gem);
            else
            {
                Debug.LogError($"Type {itemPrefab.GetType()} is not implemented");
                return;
            }
            item.transform.SetLocalPositionAndRotation(transform.position + Vector3.up, Random.rotation);
            Vector3 force = new Vector3(Random.Range(0f, 1f), 1, Random.Range(0f, 1f)) * forceScalar;
            item.Drop(force);
        }
    }
}
