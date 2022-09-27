using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] float collectionTime;

    public IEnumerator CollectThis()
    {
        yield return new WaitForSeconds(collectionTime);
        Destroy(gameObject);
    }
}
