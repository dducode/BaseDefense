using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using BroadcastMessages;

///<summary>Обеспечивает сбор предметов с игрового поля, сброшенных с врагов</summary>
public class ItemCollecting : MonoBehaviour
{
    ///<summary>Место, в котором находятся собранные деньги, пока игрок находится на вражеской базе</summary>
    [Tooltip("Место, в котором находятся собранные деньги, пока игрок находится на вражеской базе")] 
    [SerializeField] Transform stackForMoneys;

    ///<summary>Максимальное количество пачек денег в одной стопке</summary>
    ///<value>[1, infinity]</value>
    [Tooltip("Максимальное количество пачек денег в одной стопке. [1, infinity]")]
    [SerializeField, Min(1)] int maxStackSize = 15;

    ///<summary>Максимальное количество стопок</summary>
    ///<value>[1, infinity]</value>
    [Tooltip("Максимальное количество стопок. [1, infinity]")] 
    [SerializeField, Min(1)] int capacity = 2;

    ///<summary>Определяет, с какой силой сбросить все деньги</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Определяет, с какой силой сбросить все деньги. [0, infinity]")] 
    [SerializeField, Min(0)] float forceScalar = 3;

    ///<summary>Расстояние между пачками денег в стопке</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Расстояние между пачками денег в стопке. [0, infinity]")]
    [SerializeField, Min(0)] float spaceBetweenMoneys = 0.15f;

    ///<inheritdoc cref="capacity"/>
    public int Capacity => capacity;

    ///<summary>
    ///Запоминает начальное положение преобразования stackForMoneys, 
    ///т.к. в процессе сбора преобразование стека перемещается
    ///</summary>
    Vector3 firstPosition = new Vector3();

    ///<summary>Хранит все собранные игроком деньги</summary>
    Stack<Money> moneys = new Stack<Money>();

    ///<summary>Рекомендуется использовать это свойство перед запуском сопрограммы сброса денег</summary>
    public bool DropIsInProcess { get; private set; }

    int stackSize;
    int stacksCount;
    [Inject] Inventory inventory;

    void Awake()
    {
        firstPosition = stackForMoneys.localPosition;
        stackSize = 0;
        stacksCount = 0;
    }

    public void UpgradeCapacity(int step, Upgrades upgrades)
    {
        if (capacity < upgrades.Capacity.maxValue)
        {
            capacity += step;
            if (capacity > upgrades.Capacity.maxValue)
                capacity = upgrades.Capacity.maxValue;
        }
    }

    ///<summary>Кладёт кристалл в инвентарь</summary>
    public void PutGem(Gem gem)
    {
        inventory.PutItem(gem);
    }

    ///<summary>Укладывает пачку денег на верх стека</summary>
    public void StackMoney(Money money)
    {
        if (stacksCount == capacity)
            return;
        money.enabled = false;
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

    ///<summary>Реализует анимацию сброса денег и кладёт их в инвентарь</summary>
    public IEnumerator DropMoney()
    {
        DropIsInProcess = true;
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
        DropIsInProcess = false;
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
