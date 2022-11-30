using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemCollecting : MonoBehaviour
{
    [SerializeField] Transform stackForMoneys;
    [SerializeField] float spaceBetweenItems = 0.15f;
    [SerializeField] int maxStackSize = 15;
    [SerializeField] int maxStasksCount = 2;
    [SerializeField] float forceScalar = 3;
    Vector3 firstPos;
    int stackSize;
    int stacksCount;
    Stack<Money> moneys;
    Inventory inventory;

    void OnEnable() => BroadcastMessages.AddListener(MessageType.DEATH_PLAYER, LossMoney);
    void OnDisable() => BroadcastMessages.RemoveListener(MessageType.DEATH_PLAYER, LossMoney);

    void Start()
    {
        firstPos = stackForMoneys.localPosition;
        stackSize = 0;
        stacksCount = 0;
        moneys = new Stack<Money>();
        inventory.Initialize();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gem"))
            inventory.PutItem(other.GetComponent<Gem>());
        if (other.CompareTag("Money") && stacksCount < maxStasksCount)
            StackMoney(other.GetComponent<Money>());
        if (other.CompareTag("PlayerBase"))
            StartCoroutine(DropMoney());
    }

    void StackMoney(Money money)
    {
        money.Collect();
        money.transform.SetParent(stackForMoneys.parent);
        money.transform.rotation = stackForMoneys.rotation;
        money.transform.localPosition = stackForMoneys.localPosition;
        moneys.Push(money);
        stackSize++;
        if (stackSize < maxStackSize)
        {
            Vector3 vector = stackForMoneys.localPosition;
            vector.y += spaceBetweenItems;
            stackForMoneys.localPosition = vector;
        }
        else
        {
            Vector3 vector = stackForMoneys.localPosition;
            vector.y = 0;
            vector.z += spaceBetweenItems * 2.5f;
            stackForMoneys.localPosition = vector;
            stackSize = 0;
            stacksCount++;
        }
    }

    IEnumerator DropMoney()
    {
        int moneysCount = moneys.Count;
        for (int i = 0; i < moneysCount; i++)
        {
            Money money = moneys.Pop();
            money.transform.parent = null;
            SceneManager.MoveGameObjectToScene(money.gameObject, Game.ItemsScene);
            Vector3 force = new Vector3(Random.Range(0f, 1f), 1, Random.Range(-1.5f, -0.5f)) * forceScalar;
            Vector3 torque = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            money.Drop(force, torque);
            inventory.PutItem(money);
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
            Money money = moneys.Pop();
            money.transform.parent = null;
            SceneManager.MoveGameObjectToScene(money.gameObject, Game.ItemsScene);
            Vector3 force = new Vector3(Random.Range(0f, 1f), 1, Random.Range(0f, 1f)) * forceScalar;
            money.Drop(force);
        }
        stackForMoneys.localPosition = firstPos;
        stackSize = 0;
        stacksCount = 0;
    }
}
