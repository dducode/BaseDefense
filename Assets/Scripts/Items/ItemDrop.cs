using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] GameObject[] items;
    [SerializeField] float force;
    [SerializeField] ForceMode forceMode;
    [SerializeField] MinMaxSliderInt itemsCount = new MinMaxSliderInt(0, 100);

    public void Drop()
    {
        int rand = Random.Range(itemsCount.minValue, itemsCount.maxValue + 1);
        for (int i = 0; i < rand; i++)
        {
            GameObject item = Instantiate(
                items[Random.Range(0, items.Length)], transform.position + Vector3.up, Random.rotation
                );
            Rigidbody rb = item.GetComponent<Rigidbody>();
            rb.AddForce(new Vector3(Random.Range(0f, 1f), 1, Random.Range(0f, 1f)) * force, forceMode);
        }
    }
}
