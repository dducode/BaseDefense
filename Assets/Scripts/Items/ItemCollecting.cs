using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using BroadcastMessages;

public class ItemCollecting : MonoBehaviour
{
    ///<summary>Место, в котором находятся собранные деньги, пока игрок находится на вражеской базе</summary>
    [Tooltip("Место, в котором находятся собранные деньги, пока игрок находится на вражеской базе")] 
    [SerializeField] Transform stackForMoneys;

    ///<summary>Максимальное количество пачек денег в одной стопке</summary>
    ///<value>[1, infinity]</value>
    [Tooltip("Максимальное количество пачек денег в одной стопке. [1, infinity]")]
    [SerializeField, Min(1)] int maxStackSize = 15;

    ///<summary>Максимальное количество стеков</summary>
    ///<value>[1, infinity]</value>
    [Tooltip("Максимальное количество стеков. [1, infinity]")] 
    [SerializeField, Min(1)] int maxStasksCount = 2;

    ///<summary>Определяет, с какой силой сбросить все деньги</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Определяет, с какой силой сбросить все деньги. [0, infinity]")] 
    [SerializeField, Min(0)] float forceScalar = 3;

    ///<summary>Расстояние между пачками денег в стопке</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Расстояние между пачками денег в стопке. [0, infinity]")]
    [SerializeField, Min(0)] float spaceBetweenMoneys = 0.15f;

    ///<summary>
    ///Запоминает начальное положение преобразования stackForMoneys, 
    ///т.к. в процессе сбора преобразование стека перемещается
    ///</summary>
    Vector3 firstPosition;

    ///<summary>Хранит все собранные игроком деньги</summary>
    Stack<Money> moneys;

    int stackSize;
    int stacksCount;
    [Inject] Inventory inventory;

    void Start()
    {
        firstPosition = stackForMoneys.localPosition;
        stackSize = 0;
        stacksCount = 0;
        moneys = new Stack<Money>();
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

    ///<summary>Укладывает пачку денег на верх стека</summary>
    void StackMoney(Money money)
    {
        money.Collect();
        money.transform.SetParent(stackForMoneys.parent);
        money.transform.SetPositionAndRotation(stackForMoneys.position, stackForMoneys.rotation);
        moneys.Push(money);
        stackSize++;
        if (stackSize < maxStackSize) 
        // Перемещение преобразования стека наверх
        {
            Vector3 vector = stackForMoneys.localPosition;
            vector.y += spaceBetweenMoneys;
            stackForMoneys.localPosition = vector;
        }
        else 
        // Перемещение преобразования стека вниз и вперёд
        {
            Vector3 vector = stackForMoneys.localPosition;
            vector.y = 0;
            vector.z += spaceBetweenMoneys * 2.5f;
            stackForMoneys.localPosition = vector;
            stackSize = 0;
            stacksCount++;
        }
    }

    ///<summary>Реализует анимацию сброса денег, когда игрок доходит до своей базы</summary>
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
        stackForMoneys.localPosition = firstPosition;
        stackSize = 0;
        stacksCount = 0;
    }

    [Listener(MessageType.DEATH_PLAYER)]
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
        stackForMoneys.localPosition = firstPosition;
        stackSize = 0;
        stacksCount = 0;
    }
}
