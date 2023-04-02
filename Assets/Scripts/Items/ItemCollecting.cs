using System.Collections;
using System.Collections.Generic;
using BaseDefense.Messages;
using BaseDefense.SaveSystem;
using BroadcastMessages;
using UnityEngine;
using Zenject;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace BaseDefense.Items {

    ///<summary>Обеспечивает сбор предметов с игрового поля, сброшенных с врагов</summary>
    public class ItemCollecting : MonoBehaviour {

        ///<summary>Место, в котором находятся собранные деньги, пока игрок находится на вражеской базе</summary>
        [Tooltip("Место, в котором находятся собранные деньги, пока игрок находится на вражеской базе")]
        [SerializeField]
        private Transform stackForMoneys;

        ///<summary>Максимальное количество пачек денег в одной стопке</summary>
        ///<value>[1, infinity]</value>
        [Tooltip("Максимальное количество пачек денег в одной стопке. [1, infinity]")]
        [SerializeField, Min(1)]
        private int maxStackSize = 15;

        ///<summary>Максимальное количество стопок</summary>
        ///<value>[1, infinity]</value>
        [Tooltip("Максимальное количество стопок. [1, infinity]")]
        [SerializeField, Min(1)]
        private int capacity = 2;

        ///<summary>Определяет, с какой силой сбросить все деньги</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Определяет, с какой силой сбросить все деньги. [0, infinity]")]
        [SerializeField, Min(0)]
        private float forceScalar = 3;

        ///<summary>Расстояние между пачками денег в стопке</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Расстояние между пачками денег в стопке. [0, infinity]")]
        [SerializeField, Min(0)]
        private float spaceBetweenMoneys = 0.15f;

        ///<summary>
        ///Запоминает начальное положение преобразования stackForMoneys, 
        ///т.к. в процессе сбора преобразование стека перемещается
        ///</summary>
        private Vector3 m_firstPosition;

        ///<summary>Хранит все собранные игроком деньги</summary>
        private Stack<Money> m_moneys;

        ///<summary>Рекомендуется использовать это свойство перед запуском сопрограммы сброса денег</summary>
        public bool DropIsInProcess { get; private set; }

        private int m_stackSize;
        private int m_stacksCount;

        [Inject]
        private Inventory m_inventory;


        public void Save (GameDataWriter writer) {
            writer.Write(capacity);
        }


        public void Load (GameDataReader reader) {
            capacity = reader.ReadInteger();
        }


        public void UpgradeCapacity (UpgradablePropertyStep propertyStep) {
            capacity = (int) propertyStep.value;
        }


        ///<summary>Кладёт кристалл в инвентарь</summary>
        public void PutGem (Gem gem) {
            m_inventory.PutItem(gem);
        }


        ///<summary>Укладывает пачку денег на верх стека</summary>
        public void StackMoney (Money money) {
            if (m_stacksCount == capacity)
                return;
            const float animationDuration = 0.5f;
            money.Enabled = false;
            money.transform.SetParent(stackForMoneys.parent);
            var sequence = DOTween.Sequence();
            sequence.Join(money.transform.DOLocalMove(stackForMoneys.localPosition, animationDuration));
            sequence.Join(money.transform.DOLocalRotate(stackForMoneys.localRotation.eulerAngles, animationDuration));
            sequence.OnComplete(() => sequence.Kill());
            m_moneys.Push(money);
            m_stackSize++;

            if (m_stackSize < maxStackSize)
                // Перемещение преобразования стека наверх
            {
                var vector = stackForMoneys.localPosition;
                vector.y += spaceBetweenMoneys;
                stackForMoneys.localPosition = vector;
            }
            else
                // Перемещение преобразования стека вниз и вперёд
            {
                const float spaceBetweenStacks = 2.5f;
                var vector = stackForMoneys.localPosition;
                vector.y = 0;
                vector.z += spaceBetweenMoneys * spaceBetweenStacks;
                stackForMoneys.localPosition = vector;
                m_stackSize = 0;
                m_stacksCount++;
            }
        }


        ///<summary>Реализует анимацию сброса денег и кладёт их в инвентарь</summary>
        public IEnumerator DropMoney () {
            if (m_moneys.Count == 0)
                yield break;
            DropIsInProcess = true;
            var moneysCount = m_moneys.Count;

            for (var i = 0; i < moneysCount; i++) {
                const float torqueScalar = 0.5f;
                var money = m_moneys.Pop();
                money.transform.parent = null;
                ObjectsPool.MoveObjectToHisScene(money);
                var force = new Vector3(
                    Random.Range(0f, 1f),
                    1,
                    Random.Range(-1.5f, -0.5f)) * forceScalar;
                var torque = new Vector3(
                    Random.Range(-torqueScalar, torqueScalar),
                    Random.Range(-torqueScalar, torqueScalar),
                    0);
                money.Drop(force, torque);
                m_inventory.PutItem(money);

                yield return new WaitForFixedUpdate();
            }

            stackForMoneys.localPosition = m_firstPosition;
            m_stackSize = 0;
            m_stacksCount = 0;
            DropIsInProcess = false;
        }


        private void Awake () {
            m_firstPosition = stackForMoneys.localPosition;
            m_moneys = new Stack<Money>();
        }


        private void OnEnable () {
            Messenger.SubscribeTo<DeathPlayerMessage>(LossMoney);
        }


        private void OnDisable () {
            Messenger.UnsubscribeFrom<DeathPlayerMessage>(LossMoney);
        }


        private void LossMoney () {
            var moneysCount = m_moneys.Count;

            for (var i = 0; i < moneysCount; i++) {
                var money = m_moneys.Pop();
                money.transform.parent = null;
                ObjectsPool.MoveObjectToHisScene(money);
                var force = new Vector3(Random.Range(0f, 1f), 1, Random.Range(0f, 1f)) * forceScalar;
                money.Drop(force);
            }

            stackForMoneys.localPosition = m_firstPosition;
            m_stackSize = 0;
            m_stacksCount = 0;
        }

    }

}