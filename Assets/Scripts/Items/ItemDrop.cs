using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemDrop : MonoBehaviour
{
    ///<summary>Предметы, выпадаемые с персонажа после смерти</summary>
    [SerializeField, Tooltip("Предметы, выпадаемые с персонажа после смерти")] 
    Item[] items;

    ///<summary>
    ///Сила, с которой выпадают предметы. Не может быть меньше 0
    ///</summary>
    [SerializeField, Min(0),
    Tooltip("Сила, с которой выпадают предметы. Не может быть меньше 0")] 
    float forceScalar;

    ///<summary>Количество выпадаемых предметов в определённом диапазоне</summary>
    [SerializeField, Tooltip("Количество выпадаемых предметов в определённом диапазоне")] 
    MinMaxSliderInt itemsCount = new MinMaxSliderInt(0, 100);

    ///<summary>Вызывается для выброса предметов из персонажа</summary>
    ///<remarks>Выпадаемые предметы и их количество выбираются случайным образом</remarks>
    public void DropItems()
    {
        int itemsCount = Random.Range(this.itemsCount.minValue, this.itemsCount.maxValue + 1);
        for (int i = 0; i < itemsCount; i++)
        {
            Item itemPrefab = items[Random.Range(0, items.Length)];
            Item item;
            if (itemPrefab is Money)
            {
                if (ObjectsPool<Money>.IsEmpty())
                {
                    item = Instantiate(itemPrefab);
                    SceneManager.MoveGameObjectToScene(item.gameObject, Game.ItemsScene);
                }
                else
                    item = ObjectsPool<Money>.Pop();
            }
            else if (itemPrefab is Gem)
            {
                if (ObjectsPool<Gem>.IsEmpty())
                {
                    item = Instantiate(itemPrefab);
                    SceneManager.MoveGameObjectToScene(item.gameObject, Game.ItemsScene);
                }
                else
                    item = ObjectsPool<Gem>.Pop();
            }
            else
            {
                Debug.LogError($"Unknow item prefab {itemPrefab}");
                return;
            }
            item.transform.SetLocalPositionAndRotation(transform.position + Vector3.up, Random.rotation);
            Vector3 force = new Vector3(Random.Range(0f, 1f), 1, Random.Range(0f, 1f)) * forceScalar;
            item.Drop(force);
        }
    }
}
