using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollecting : MonoBehaviour
{
    [SerializeField] Transform stackForMoneys;
    [SerializeField] float spaceBetweenItems = 0.15f;
    [SerializeField] int maxStackSize = 15;
    [SerializeField] int maxStasksCount = 2;
    [SerializeField] float force = 3;
    [SerializeField] ForceMode forceMode = ForceMode.Impulse;
    Vector3 firstPos;
    int stackSize;
    int stacksCount;
    Stack<Transform> moneys;

    void OnEnable() => BroadcastMessages.AddListener(MessageType.DEATH_PLAYER, LossMoney);
    void OnDisable() => BroadcastMessages.RemoveListener(MessageType.DEATH_PLAYER, LossMoney);

    void Start()
    {
        firstPos = stackForMoneys.localPosition;
        stackSize = 0;
        stacksCount = 0;
        moneys = new Stack<Transform>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gem"))
            Inventory.getInstance.CollectItem(other.gameObject);
        if (other.CompareTag("Money") && stacksCount < maxStasksCount)
            StackMoney(other.transform);
        if (other.CompareTag("PlayerBase"))
            StartCoroutine(CollectMoney());
    }

    void StackMoney(Transform money)
    {
        money.GetComponent<Rigidbody>().isKinematic = true;
        money.GetComponent<SphereCollider>().enabled = false;
        money.SetParent(stackForMoneys.parent);
        money.rotation = stackForMoneys.rotation;
        money.localPosition = stackForMoneys.localPosition;
        moneys.Push(money);
        stackSize++;
        if (stackSize < maxStackSize)
        {
            Vector3 vector = stackForMoneys.localPosition;
            vector.x -= spaceBetweenItems;
            stackForMoneys.localPosition = vector;
        }
        else
        {
            Vector3 vector = stackForMoneys.localPosition;
            vector.x = 0;
            vector.y += spaceBetweenItems * 2.5f;
            stackForMoneys.localPosition = vector;
            stackSize = 0;
            stacksCount++;
        }
    }

    IEnumerator CollectMoney()
    {
        int moneysCount = moneys.Count;
        for (int i = 0; i < moneysCount; i++)
        {
            Transform money = moneys.Pop();
            money.parent = null;
            Rigidbody rb = money.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(new Vector3(Random.Range(0f, 1f), 1, Random.Range(-1.5f, -0.5f)) * force, forceMode);
            StartCoroutine(money.GetComponent<Money>().CollectThis());
            Inventory.getInstance.CollectItem(money.gameObject);
            yield return new WaitForFixedUpdate();
        }
        stackForMoneys.localPosition = firstPos;
        stackSize = 0;
        stacksCount = 0;
    }

    void LossMoney()
    {
        int moneysCount = moneys.Count;
        for (int i = 0; i < moneysCount; i++)
        {
            Transform money = moneys.Pop();
            money.parent = null;
            money.GetComponent<SphereCollider>().enabled = true;
            Rigidbody rb = money.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(new Vector3(Random.Range(0f, 1f), 1, Random.Range(0f, 1f)) * force, forceMode);
        }
        stackForMoneys.localPosition = firstPos;
        stackSize = 0;
        stacksCount = 0;
    }
}
