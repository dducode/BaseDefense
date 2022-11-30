using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] Item[] items;
    [SerializeField] float forceScalar;
    [SerializeField] MinMaxSliderInt itemsCount = new MinMaxSliderInt(0, 100);

    public void DropItems()
    {
        int itemsCount = Random.Range(this.itemsCount.minValue, this.itemsCount.maxValue + 1);
        for (int i = 0; i < itemsCount; i++)
        {
            Item itemPrefab = items[Random.Range(0, items.Length)];
            Item item;
            if (itemPrefab is Money)
            {
                if (Pools.MoneysCount == 0)
                {
                    item = Instantiate(itemPrefab);
                    SceneManager.MoveGameObjectToScene(item.gameObject, Game.ItemsScene);
                }
                else
                    item = Pools.PopMoney();
            }
            else if (itemPrefab is Gem)
            {
                if (Pools.GemsCount == 0)
                {
                    item = Instantiate(itemPrefab);
                    SceneManager.MoveGameObjectToScene(item.gameObject, Game.ItemsScene);
                }
                else
                    item = Pools.PopGem();
            }
            else
            {
                Debug.LogError($"Unknow item prefab {itemPrefab}");
                return;
            }
            item.transform.localPosition = transform.position + Vector3.up;
            item.transform.localRotation = Random.rotation;
            Vector3 force = new Vector3(Random.Range(0f, 1f), 1, Random.Range(0f, 1f)) * forceScalar;
            item.Drop(force);
        }
    }
}
